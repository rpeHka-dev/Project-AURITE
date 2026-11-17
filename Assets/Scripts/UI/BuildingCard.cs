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

        // Получаем информацию о производстве и рабочих
        string productionInfo = GetProductionString(building);
        string workerInfo = "";

        // Если это барак — добавляем строку про рабочих
        if (building.buildingName == "Барак" || building.buildingName == "Barrack")
        {
            workerInfo = "+30 рабочих";
        }

        // Собираем текст: если есть и добыча, и рабочие — две строки
        if (!string.IsNullOrEmpty(productionInfo) && !string.IsNullOrEmpty(workerInfo))
            productionText.text = $"{productionInfo}\n{workerInfo}";
        else if (!string.IsNullOrEmpty(productionInfo))
            productionText.text = productionInfo;
        else if (!string.IsNullOrEmpty(workerInfo))
            productionText.text = workerInfo;
        else
            productionText.text = "";

        colorStrip.color = building.hexColor;

        // === ПРОВЕРКА РЕСУРСОВ (ВСТАВИТЬ СЮДА) ===
        bool canBuild = ResourceManager.Instance.HasResources(building);
        selectButton.interactable = canBuild;

        if (!canBuild)
        {
            // Красим кнопку в красный цвет, если ресурсов не хватает
            selectButton.GetComponent<Image>().color = new Color(0.5f, 0.2f, 0.2f);
        }
        else
        {
            // Если ресурсов хватает — возвращаем стандартный цвет (или оставляем как есть)
            selectButton.GetComponent<Image>().color = Color.white; // или стандартный цвет кнопки
        }
        // ===========================================

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
        if (building.workerCost > 0) parts.Add($"Рабочие {building.workerCost}");

        string cost = "";
        for (int i = 0; i < parts.Count; i++)
        {
            if (i > 0)
                cost += i == 2 ? "\n" : "  ·  ";
            cost += parts[i];
        }
        return cost;
    }

    // Вывод добычи ресурсов (если она есть)
    string GetProductionString(BuildingData building)
    {
        int amount = Mathf.RoundToInt(building.productionPerWorker);
        if (amount <= 0) return ""; // Если добычи нет — возвращаем пустую строку

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