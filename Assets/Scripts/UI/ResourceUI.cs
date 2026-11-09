using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public TMP_Text plasticText;
    public TMP_Text auriteText;
    public TMP_Text batteriesText;
    public TMP_Text coolantText;

    void Start()
    {
        UpdateUI();
        ResourceManager.Instance.OnResourcesChanged += UpdateUI;
    }

    void UpdateUI()
    {
        plasticText.text = $"Пластик: {ResourceManager.Instance.plastic}";
        auriteText.text = $"Аурит: {ResourceManager.Instance.aurite}";
        batteriesText.text = $"Батареи: {ResourceManager.Instance.batteries}";
        coolantText.text = $"Теплоноситель: {ResourceManager.Instance.coolant}";
    }
}