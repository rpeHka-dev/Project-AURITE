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

    [Header("Характеристики")]
    public int maxWorkers;
    public bool conductsHeat = true;
    public float productionPerWorker; // для шахт
    public ResourceType producedResource; // какой ресурс добывается каждый день

    [Header("Отображение")]
    public Color hexColor = Color.white; // цвет гекса с этим зданием


    [Header("Описание")]
    [TextArea(2, 4)]
    public string description;
}