using UnityEngine;

public class ProductionManager : MonoBehaviour
{
    void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed += OnDayPassed;
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed -= OnDayPassed;
    }

    // Каждый день здания приносят ресурсы
    void OnDayPassed()
    {
        HexCell[] cells = FindObjectsByType<HexCell>(FindObjectsSortMode.None);

        foreach (HexCell cell in cells)
        {
            BuildingData building = cell.buildingData;
            if (building == null || building.producedResource == ResourceType.None)
                continue;

            int amount = Mathf.RoundToInt(building.productionPerWorker);
            if (amount <= 0)
                continue;

            AddResource(building.producedResource, amount);
            Debug.Log($"{building.buildingName} принесла +{amount} {GetResourceName(building.producedResource)}");
        }
    }

    void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Plastic: ResourceManager.Instance.AddResources(amount, 0, 0, 0); break;
            case ResourceType.Aurite: ResourceManager.Instance.AddResources(0, amount, 0, 0); break;
            case ResourceType.Batteries: ResourceManager.Instance.AddResources(0, 0, amount, 0); break;
            case ResourceType.Coolant: ResourceManager.Instance.AddResources(0, 0, 0, amount); break;
        }
    }

    // Для сообщения в консоль: "принесла +10 аурита"
    string GetResourceName(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Plastic: return "пластика";
            case ResourceType.Aurite: return "аурита";
            case ResourceType.Batteries: return "батарей";
            case ResourceType.Coolant: return "теплоносителя";
            default: return "";
        }
    }
}
