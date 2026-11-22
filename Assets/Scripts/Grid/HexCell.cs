using UnityEngine;
using UnityEngine.EventSystems;

public class HexCell : MonoBehaviour, IPointerClickHandler
{
    public int Q { get; private set; }
    public int R { get; private set; }
    public int S { get; private set; }
    public GameObject currentBuilding { get; set; }

    // Какое здание стоит на гексе (null, если пусто)
    public BuildingData buildingData { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Color terrainColor;
    private bool isHovered;

    // Цвета температуры
    static readonly Color TempMinus2 = new Color(0.0f, 0.2f, 0.4f);   // Тёмно-синий
    static readonly Color TempMinus1 = new Color(0.0f, 0.4f, 0.8f);   // Голубой
    static readonly Color TempZero = new Color(0.6f, 0.8f, 1.0f);  // Снежный
    static readonly Color TempPlus1 = new Color(1.0f, 0.8f, 0.6f);    // Светло-оранжевый
    static readonly Color TempPlus2 = new Color(1.0f, 0.576f, 0.2f);    // Оранжевый

    // Температура гекса (-2, -1, 0, 1, 2)
    public int temperature;

    private const int DistributorRadius = 2;

    public void PlaceBuilding(BuildingData building)
    {
        buildingData = building;
        UpdateColor();

        // Если это барак — увеличиваем число рабочих
        if (building.buildingName == "Барак")
        {
            ResourceManager.Instance.AddWorkers(30);
            Debug.Log("Построен барак. Рабочие +30");
        }

        // Склад увеличивает лимиты
        if (building.storageBonusPlastic > 0 || building.storageBonusAurite > 0 ||
            building.storageBonusBatteries > 0 || building.storageBonusCoolant > 0)
        {
            ResourceManager.Instance.AddStorageCapacity(
                building.storageBonusPlastic,
                building.storageBonusAurite,
                building.storageBonusBatteries,
                building.storageBonusCoolant
            );
        }

        // === РАСПРЕДЕЛИТЕЛЬ ТЕПЛА ===
        // Если построен распределитель — пересчитываем температуру у всех гексов
        if (building.buildingName == "Распределитель тепла" || building.buildingName == "HeatDistributor")
        {
            UpdateAllTemperatures();
        }
    }

    public void RemoveBuilding()
    {
        if (buildingData != null)
        {
            if (buildingData.buildingName == "Барак")
            {
                ResourceManager.Instance.AddWorkers(-30);
            }

            if (buildingData.storageBonusPlastic > 0 || buildingData.storageBonusAurite > 0 ||
                buildingData.storageBonusBatteries > 0 || buildingData.storageBonusCoolant > 0)
            {
                ResourceManager.Instance.RemoveStorageCapacity(
                    buildingData.storageBonusPlastic,
                    buildingData.storageBonusAurite,
                    buildingData.storageBonusBatteries,
                    buildingData.storageBonusCoolant
                );
            }

            // === РАСПРЕДЕЛИТЕЛЬ ТЕПЛА ===
            // Если сносим распределитель — пересчитываем температуру у всех гексов
            if (buildingData.buildingName == "Распределитель тепла" || buildingData.buildingName == "HeatDistributor")
            {
                buildingData = null;
                UpdateAllTemperatures();
                UpdateColor();
                return;
            }
        }

        buildingData = null;
        UpdateColor();
    }

    // Пересчитывает температуру для ВСЕХ гексов на сцене
    void UpdateAllTemperatures()
    {
        HexCell[] allCells = FindObjectsByType<HexCell>(FindObjectsSortMode.None);
        foreach (HexCell cell in allCells)
        {
            cell.CalculateTemperature();
            cell.ApplyDistributorBonus();
            cell.terrainColor = cell.GetTerrainColor();
            cell.UpdateColor();
        }
    }

    // Применяет бонус от всех распределителей на карте
    void ApplyDistributorBonus()
    {
        HexCell[] allCells = FindObjectsByType<HexCell>(FindObjectsSortMode.None);

        foreach (HexCell cell in allCells)
        {
            if (cell.buildingData == null) continue;

            if (cell.buildingData.buildingName == "Распределитель тепла" ||
                cell.buildingData.buildingName == "HeatDistributor")
            {
                int dist = (Mathf.Abs(Q - cell.Q) + Mathf.Abs(R - cell.R) + Mathf.Abs(S - cell.S)) / 2;

                if (dist == 1)
                {
                    temperature += 2; // В радиусе 1 — +2
                }
                else if (dist == 2)
                {
                    temperature += 1; // В радиусе 2 — +1
                }
            }
        }

        // Ограничиваем температуру от -2 до 2
        temperature = Mathf.Clamp(temperature, -2, 2);
    }

    public void Setup(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
        transform.position = HexMetrics.GetPositionFromCube(q, r, s);
        gameObject.name = $"HexCell_{q}_{r}_{s}";

        // Вызываем расчет температуры сразу при создании гекса
        CalculateTemperature();

        spriteRenderer = GetComponent<SpriteRenderer>();
        // Нижние ряды рисуем поверх верхних, чтобы была видна толщина гекса
        spriteRenderer.sortingOrder = -r;
        terrainColor = GetTerrainColor();
        UpdateColor();
    }

    Color GetTerrainColor()
    {
        Color color;
        switch (temperature)
        {
            case -2: color = TempMinus2; break;
            case -1: color = TempMinus1; break;
            case 0: color = TempZero; break;
            case 1: color = TempPlus1; break;
            case 2: color = TempPlus2; break;
            default: color = TempZero; break;
        }

        // Немного меняем яркость, чтобы гексы не были одинаковыми
        float noise = Mathf.PerlinNoise(Q * 0.7f + 10f, R * 0.7f + 10f) * 0.1f - 0.05f;
        return new Color(color.r + noise, color.g + noise, color.b + noise);
    }

    void UpdateColor()
    {
        Color color = buildingData != null ? buildingData.hexColor : terrainColor;
        if (isHovered)
            color = Color.Lerp(color, Color.white, 0.3f);

        spriteRenderer.color = color;
    }

    // Подсветка гекса под курсором
    private void OnMouseEnter()
    {
        isHovered = true;
        UpdateColor();
    }

    private void OnMouseExit()
    {
        isHovered = false;
        UpdateColor();
    }

    // Логика расчета температуры в зависимости от расстояния до центра
    public void CalculateTemperature()
    {
        int distance = (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;

        if (distance <= 1)
            temperature = 2;
        else if (distance == 2)
            temperature = 1;
        else
            temperature = -2;
    }

    // --- ЗАГЛУШКА ДЛЯ РАСПРЕДЕЛИТЕЛЯ ТЕПЛА ---
    // Оставлена для совместимости, но теперь использует новую логику
    public void IncreaseTemperatureFromDistributor()
    {
        UpdateAllTemperatures();
    }

    private void OnMouseDown()
    {
        Debug.Log($"Clicked hex: ({Q}, {R}, {S})");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick: ({Q}, {R}, {S})");
    }
}