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

    // Цвета земли: в тёплом центре грунт, дальше иней и снег
    static readonly Color ThawedGround = new Color(0.52f, 0.48f, 0.43f);
    static readonly Color Frost = new Color(0.72f, 0.78f, 0.84f);
    static readonly Color Snow = new Color(0.90f, 0.94f, 0.98f);
   
    // Температура гекса (-2, -1, 0, 1, 2)
    public int temperature;

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
        }

        buildingData = null;
        UpdateColor();
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
        if (temperature >= 2)
            color = ThawedGround;
        else if (temperature >= 0)
            color = Frost;
        else
            color = Snow;

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

        if (distance <= 2)
            temperature = 2;
        else if (distance == 3)
            temperature = 1;
        else
            temperature = -2;
    }

    // --- ЗАГЛУШКА ДЛЯ РАСПРЕДЕЛИТЕЛЯ ТЕПЛА ---
    // Этот метод будет вызываться при постройке распределителя тепла.
    // Пока он просто увеличивает температуру текущего гекса на 1.
    public void IncreaseTemperatureFromDistributor()
    {
        // TODO: Реализовать полноценную логику (увеличение температуры в радиусе, проверка на дубли и т.д.)
        temperature += 1;
       
        // Пока просто выводим в консоль для проверки
        Debug.Log($"Температура гекса ({Q},{R},{S}) увеличена распределителем до {temperature}");
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