using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Карточка здания в панели строительства
public class BuildingCard : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text costText;
    public TMP_Text productionText;
    public Image colorStrip;
    public Button selectButton;

    public void Setup(BuildingData building, System.Action<BuildingData> onSelect)
    {
        titleText.text = building.buildingName.ToUpper();
        costText.text = GetCostString(building);
        productionText.text = GetProductionString(building);
        colorStrip.color = building.hexColor;

        selectButton.onClick.AddListener(() => onSelect(building));
    }

    // Цена: по два ресурса в строке
    string GetCostString(BuildingData building)
    {
        var parts = new System.Collections.Generic.List<string>();
        if (building.plasticCost > 0) parts.Add($"Пластик {building.plasticCost}");
        if (building.auriteCost > 0) parts.Add($"Аурит {building.auriteCost}");
        if (building.batteryCost > 0) parts.Add($"Батареи {building.batteryCost}");
        if (building.coolantCost > 0) parts.Add($"Теплонос. {building.coolantCost}");

        string cost = "";
        for (int i = 0; i < parts.Count; i++)
        {
            if (i > 0)
                cost += i == 2 ? "\n" : "  ·  ";
            cost += parts[i];
        }
        return cost;
    }

    string GetProductionString(BuildingData building)
    {
        int amount = Mathf.RoundToInt(building.productionPerWorker);
        switch (building.producedResource)
        {
            case ResourceType.Plastic: return $"+{amount} пластика / день";
            case ResourceType.Aurite: return $"+{amount} аурита / день";
            case ResourceType.Batteries: return $"+{amount} батарей / день";
            case ResourceType.Coolant: return $"+{amount} теплонос. / день";
            default: return "";
        }
    }
}
