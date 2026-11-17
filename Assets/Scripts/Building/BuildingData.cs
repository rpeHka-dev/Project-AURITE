using UnityEngine;

public enum ResourceType
{
    None,
    Plastic,
    Aurite,
    Batteries,
    Coolant
}

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Aurite/BuildingData")]
public class BuildingData : ScriptableObject
{
    [Header("Основное")]
    public string buildingName;
    public GameObject prefab;
    public Sprite icon;

    [Header("Стоимость")]
    public int plasticCost;
    public int auriteCost;
    public int batteryCost;
    public int coolantCost;
    public int workerCost;

    [Header("Характеристики")]
    public int maxWorkers;
    public bool conductsHeat = true;
    public float productionPerWorker; // для шахт
    public ResourceType producedResource; // какой ресурс добывается каждый день

    
    [Header("Бонус склада (только для склада)")]
    public int storageBonusPlastic = 50;
    public int storageBonusAurite = 100;
    public int storageBonusBatteries = 20;
    public int storageBonusCoolant = 10;
    

    [Header("Отображение")]
    public Color hexColor = Color.white; // цвет гекса с этим зданием

    [Header("Описание")]
    [TextArea(2, 4)]
    public string description;
}