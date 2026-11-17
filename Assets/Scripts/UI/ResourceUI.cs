using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [Header("Тексты ресурсов")]
    public TMP_Text plasticText;
    public TMP_Text auriteText;
    public TMP_Text batteriesText;
    public TMP_Text coolantText;
    public TMP_Text workersText;

    void Start()
    {
        UpdateUI();
        ResourceManager.Instance.OnResourcesChanged += UpdateUI;
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourcesChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        var rm = ResourceManager.Instance;

        // Считаем суммарную добычу за день
        int plasticPerDay = GetTotalProduction(ResourceType.Plastic);
        int auritePerDay = GetTotalProduction(ResourceType.Aurite);
        int batteriesPerDay = GetTotalProduction(ResourceType.Batteries);
        int coolantPerDay = GetTotalProduction(ResourceType.Coolant);

        // Формируем строки
        plasticText.text = $"Пластик: {rm.plastic} / {rm.maxPlastic} | {GetBonusString(plasticPerDay)}";
        auriteText.text = $"Аурит: {rm.aurite} / {rm.maxAurite} | {GetBonusString(auritePerDay)}";
        batteriesText.text = $"Батареи: {rm.batteries} / {rm.maxBatteries} | {GetBonusString(batteriesPerDay)}";
        coolantText.text = $"Теплоноситель: {rm.coolant} / {rm.maxCoolant} | {GetBonusString(coolantPerDay)}";

        if (workersText != null)
            workersText.text = $"Рабочие: {rm.workers}";
    }

    // Считаем, сколько всего ресурсов добывается за день (по всем зданиям)
    int GetTotalProduction(ResourceType type)
    {
        int total = 0;
        HexCell[] cells = FindObjectsByType<HexCell>(FindObjectsSortMode.None);

        foreach (HexCell cell in cells)
        {
            if (cell.buildingData != null && cell.buildingData.producedResource == type)
            {
                total += Mathf.RoundToInt(cell.buildingData.productionPerWorker);
            }
        }
        return total;
    }

    // Форматируем строку
    string GetBonusString(int amount)
    {
        if (amount > 0)
            return $"+{amount}";
        else
            return "0";
    }
}