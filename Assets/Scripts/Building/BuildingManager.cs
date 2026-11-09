using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [SerializeField] private BuildingData[] availableBuildings;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanBuild(HexCell cell, BuildingData building)
    {
        // Гекс должен быть пустым
        if (cell.currentBuilding != null) return false;

        // Должен быть сосед с зданием (кроме центрального реактора)
        bool hasNeighbor = false;
        // Проверка соседей будет добавлена позже

        return ResourceManager.Instance.HasResources(building);
    }

    public void Build(HexCell cell, BuildingData building)
    {
        if (!CanBuild(cell, building)) return;

        ResourceManager.Instance.SpendResources(building);

        GameObject buildingObj = Instantiate(building.prefab, cell.transform);
        buildingObj.transform.localPosition = Vector3.zero;
        cell.currentBuilding = buildingObj;

        Debug.Log($"Built {building.buildingName} at ({cell.Q}, {cell.R}, {cell.S})");
    }

    public void DeleteBuilding(HexCell cell)
    {
        if (cell.currentBuilding == null) return;

        // Найти BuildingData по префабу (упрощённо)
        // В реальности нужно хранить ссылку на BuildingData в компоненте здания

        Destroy(cell.currentBuilding);
        cell.currentBuilding = null;

        Debug.Log($"Deleted building at ({cell.Q}, {cell.R}, {cell.S})");
    }
}