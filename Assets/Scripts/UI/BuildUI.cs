using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    [Header("Кнопки")]
    public Button btnMine;
    public Button btnBarrack;
    public Button btnAerodrome;
    public Button btnComms;
    public Button btnRoad;

    [Header("Building Data")]
    public BuildingData mineData;
    public BuildingData barrackData;
    public BuildingData aerodromeData;
    public BuildingData commsData;
    public BuildingData roadData;

    private HexCell selectedCell;

    void Start()
    {
        // Назначаем обработчики кнопок
        btnMine.onClick.AddListener(() => TryBuild(mineData));
        btnBarrack.onClick.AddListener(() => TryBuild(barrackData));
        btnAerodrome.onClick.AddListener(() => TryBuild(aerodromeData));
        btnComms.onClick.AddListener(() => TryBuild(commsData));
        btnRoad.onClick.AddListener(() => TryBuild(roadData));
    }

    void Update()
    {
        // Клик по гексу для выбора
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                HexCell cell = hit.collider.GetComponent<HexCell>();
                if (cell != null)
                {
                    selectedCell = cell;
                    Debug.Log($"Selected hex: ({cell.Q}, {cell.R}, {cell.S})");
                }
            }
        }

        // Правая кнопка мыши для удаления здания
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                HexCell cell = hit.collider.GetComponent<HexCell>();
                if (cell != null && cell.currentBuilding != null)
                {
                    DeleteBuilding(cell);
                }
            }
        }
    }

    void DeleteBuilding(HexCell cell)
    {
        // Находим BuildingData по имени здания (упрощённо)
        BuildingData buildingData = GetBuildingDataByName(cell.currentBuilding.name);

        if (buildingData != null)
        {
            ResourceManager.Instance.RefundResources(buildingData);
        }

        Destroy(cell.currentBuilding);
        cell.currentBuilding = null;

        Debug.Log($"Deleted building at ({cell.Q}, {cell.R}, {cell.S})");
    }

    BuildingData GetBuildingDataByName(string name)
    {
        // Сопоставляем имя здания с BuildingData
        switch (name)
        {
            case "AuriteMine": return mineData;
            case "Barrack": return barrackData;
            case "Aerodrome": return aerodromeData;
            case "CommsStation": return commsData;
            case "Road": return roadData;
            default: return null;
        }
    }

    void TryBuild(BuildingData building)
    {
        if (selectedCell == null)
        {
            Debug.Log("Сначала выберите гекс (кликните на него)");
            return;
        }

        if (selectedCell.currentBuilding != null)
        {
            Debug.Log("На этом гексе уже есть здание!");
            return;
        }

        if (ResourceManager.Instance.HasResources(building))
        {
            ResourceManager.Instance.SpendResources(building);
            CreateBuildingPlaceholder(selectedCell, building);
            Debug.Log($"Построено: {building.buildingName}");
        }
        else
        {
            Debug.Log("Не хватает ресурсов!");
        }
    }



    void CreateBuildingPlaceholder(HexCell cell, BuildingData building)
    {
        // Имя ассета (AuriteMine), а не buildingName (Aurite Mine): по нему GetBuildingDataByName находит здание при удалении
        GameObject buildingObj = new GameObject(building.name);
        buildingObj.transform.SetParent(cell.transform);
        buildingObj.transform.localPosition = new Vector3(0, 0, -0.1f);

        SpriteRenderer cellRenderer = cell.GetComponent<SpriteRenderer>();
        SpriteRenderer buildingRenderer = buildingObj.AddComponent<SpriteRenderer>();

        buildingRenderer.sprite = cellRenderer.sprite;
        buildingRenderer.color = new Color(0f, 1f, 0f, 0.7f); // Полупрозрачный зелёный
        buildingRenderer.sortingOrder = 10;

        // Размер здания = 80% от размера гекса
        // Объект — дочерний к гексу и уже наследует его масштаб, поэтому достаточно 0.8
        buildingObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        cell.currentBuilding = buildingObj;
    }
}