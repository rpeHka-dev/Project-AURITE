using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Текущие ресурсы")]
    public int plastic = 200;
    public int aurite = 300;
    public int batteries = 50;
    public int coolant = 3;

    [Header("События")]
    public System.Action OnResourcesChanged;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayPassed += OnDayPassed;
        }
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayPassed -= OnDayPassed;
        }
    }

    void OnDayPassed()
    {
        // Здесь будет добыча ресурсов каждый день
        Debug.Log("Прошёл день — добыча ресурсов");
    }

    // === Проверка и списание для зданий ===

    public bool HasResources(BuildingData building)
    {
        return plastic >= building.plasticCost &&
               aurite >= building.auriteCost &&
               batteries >= building.batteryCost &&
               coolant >= building.coolantCost;
    }

    public void SpendResources(BuildingData building)
    {
        plastic -= building.plasticCost;
        aurite -= building.auriteCost;
        batteries -= building.batteryCost;
        coolant -= building.coolantCost;
        OnResourcesChanged?.Invoke();
    }

    public void RefundResources(BuildingData building)
    {
        plastic += building.plasticCost;
        aurite += building.auriteCost;
        batteries += building.batteryCost;
        coolant += building.coolantCost;
        OnResourcesChanged?.Invoke();
    }

    // === Общее добавление ресурсов ===

    public void AddResources(int plasticAmt, int auriteAmt, int batteriesAmt, int coolantAmt)
    {
        plastic += plasticAmt;
        aurite += auriteAmt;
        batteries += batteriesAmt;
        coolant += coolantAmt;
        OnResourcesChanged?.Invoke();
    }
}