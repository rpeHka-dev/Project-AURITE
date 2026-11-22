using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Текущие ресурсы")]
    public int plastic = 200;
    public int aurite = 300;
    public int batteries = 50;
    public int coolant = 5;
    public int workers = 200;

    [Header("Базовые лимиты (стартовые)")]
    public int basePlasticLimit = 300;
    public int baseAuriteLimit = 400;
    public int baseBatteriesLimit = 100;
    public int baseCoolantLimit = 10;

    [Header("Текущие лимиты (увеличиваются складами)")]
    public int maxPlastic;
    public int maxAurite;
    public int maxBatteries;
    public int maxCoolant;

    [Header("События")]
    public System.Action OnResourcesChanged;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Инициализируем лимиты базовыми значениями
        maxPlastic = basePlasticLimit;
        maxAurite = baseAuriteLimit;
        maxBatteries = baseBatteriesLimit;
        maxCoolant = baseCoolantLimit;
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

    // === Изменение лимитов (для складов) ===

    public void AddStorageCapacity(int plasticAmt, int auriteAmt, int batteriesAmt, int coolantAmt)
    {
        maxPlastic += plasticAmt;
        maxAurite += auriteAmt;
        maxBatteries += batteriesAmt;
        maxCoolant += coolantAmt;

        OnResourcesChanged?.Invoke();
        Debug.Log($"Лимиты увеличены: Пластик +{plasticAmt}, Аурит +{auriteAmt}, Батареи +{batteriesAmt}, Теплоноситель +{coolantAmt}");
    }

    public void RemoveStorageCapacity(int plasticAmt, int auriteAmt, int batteriesAmt, int coolantAmt)
    {
        maxPlastic = Mathf.Max(basePlasticLimit, maxPlastic - plasticAmt);
        maxAurite = Mathf.Max(baseAuriteLimit, maxAurite - auriteAmt);
        maxBatteries = Mathf.Max(baseBatteriesLimit, maxBatteries - batteriesAmt);
        maxCoolant = Mathf.Max(baseCoolantLimit, maxCoolant - coolantAmt);

        OnResourcesChanged?.Invoke();
        Debug.Log($"Лимиты уменьшены (склад снесен)");
    }

    // === Проверка и списание для зданий ===

    public bool HasResources(BuildingData building)
    {
        return plastic >= building.plasticCost &&
               aurite >= building.auriteCost &&
               batteries >= building.batteryCost &&
               coolant >= building.coolantCost &&
               workers >= building.workerCost;
    }

    public void SpendResources(BuildingData building)
    {
        plastic -= building.plasticCost;
        aurite -= building.auriteCost;
        batteries -= building.batteryCost;
        coolant -= building.coolantCost;
        workers -= building.workerCost;
        OnResourcesChanged?.Invoke();
    }

    public void RefundResources(BuildingData building)
    {
        plastic += building.plasticCost;
        aurite += building.auriteCost;
        batteries += building.batteryCost;
        coolant += building.coolantCost;
        workers += building.workerCost;
        OnResourcesChanged?.Invoke();
    }

    // === Общее добавление ресурсов (с учетом лимитов) ===

    public void AddResources(int plasticAmt, int auriteAmt, int batteriesAmt, int coolantAmt)
    {
        plastic = Mathf.Min(plastic + plasticAmt, maxPlastic);
        aurite = Mathf.Min(aurite + auriteAmt, maxAurite);
        batteries = Mathf.Min(batteries + batteriesAmt, maxBatteries);
        coolant = Mathf.Min(coolant + coolantAmt, maxCoolant);

        OnResourcesChanged?.Invoke();
    }

    // Добавление рабочих
    public void AddWorkers(int amount)
    {
        workers += amount;
        OnResourcesChanged?.Invoke();
        Debug.Log($"Рабочие: {workers} ({amount:+0;-0})");
    }
}