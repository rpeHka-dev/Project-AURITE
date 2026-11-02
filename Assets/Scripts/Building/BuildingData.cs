using UnityEngine;

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


    [Header("Описание")]
    [TextArea(2, 4)]
    public string description;
}