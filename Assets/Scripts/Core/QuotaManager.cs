using UnityEngine;

public class QuotaManager : MonoBehaviour
{
    public static QuotaManager Instance { get; private set; }

    [Header("Настройки квоты")]
    public int startQuota = 1000;
    public int growthMultiplier = 2;
    public int maxQuotaLimit = 0;

    [Header("Настройки поставки")]
    public int minSendAmount = 100;        // Минимум за раз
    public float sendCooldownDays = 7f;    // Перезарядка в днях

    [Header("Текущее состояние")]
    public int currentMonth = 1;
    public int currentQuota;
    public int auriteSentThisMonth = 0;

    // Кулдаун (в днях)
    private float cooldownTimer = 0f;
    public bool IsOnCooldown => cooldownTimer > 0f;

    [Header("События")]
    public System.Action OnQuotaChanged;
    public System.Action OnQuotaFailed;
    public System.Action OnQuotaCompleted;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentQuota = startQuota;

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayPassed += OnDayPassed;
            TimeManager.Instance.OnMonthPassed += OnMonthPassed;
        }

        OnQuotaChanged?.Invoke();
    }

    void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed -= OnDayPassed;
    }

    // Каждый день уменьшаем кулдаун
    void OnDayPassed()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= 1f;
            if (cooldownTimer < 0f) cooldownTimer = 0f;
            OnQuotaChanged?.Invoke();
        }
    }

    // Максимально возможная сумма для отправки (половина квоты)
    public int GetMaxSendAmount()
    {
        return Mathf.Max(minSendAmount, currentQuota / 2);
    }

    // Можно ли отправить
    public bool CanSend()
    {
        return !IsOnCooldown && auriteSentThisMonth < currentQuota;
    }

    // Сколько дней осталось до конца перезарядки
    public int GetCooldownDaysLeft()
    {
        return Mathf.CeilToInt(cooldownTimer);
    }

    public void SendAurite(int amount)
    {
        if (IsOnCooldown)
        {
            Debug.Log("Поставка на перезарядке!");
            return;
        }

        if (amount < minSendAmount)
        {
            Debug.Log($"Минимальная сумма отправки: {minSendAmount}");
            return;
        }

        if (amount > GetMaxSendAmount())
        {
            Debug.Log($"Максимальная сумма отправки: {GetMaxSendAmount()}");
            return;
        }

        if (ResourceManager.Instance.aurite < amount)
        {
            Debug.Log("Недостаточно аурита!");
            return;
        }

        ResourceManager.Instance.AddResources(0, -amount, 0, 0);
        auriteSentThisMonth += amount;

        // Запускаем перезарядку
        cooldownTimer = sendCooldownDays;

        Debug.Log($"Отправлено {amount} аурита. Всего: {auriteSentThisMonth}/{currentQuota}. Перезарядка: {sendCooldownDays} дней");

        if (auriteSentThisMonth >= currentQuota)
        {
            OnQuotaCompleted?.Invoke();
        }

        OnQuotaChanged?.Invoke();
    }

    void OnMonthPassed()
    {
        if (auriteSentThisMonth < currentQuota)
            OnQuotaFailed?.Invoke();
        else
            OnQuotaCompleted?.Invoke();

        currentMonth++;
        auriteSentThisMonth = 0;
        cooldownTimer = 0f; // Сбрасываем кулдаун при новом месяце

        int newQuota = currentQuota * growthMultiplier;
        if (maxQuotaLimit > 0 && newQuota > maxQuotaLimit)
            newQuota = maxQuotaLimit;

        currentQuota = newQuota;
        Debug.Log($"Месяц {currentMonth}. Новая квота: {currentQuota}");
        OnQuotaChanged?.Invoke();
    }

    // Подписка на месяцы (если нужно)
    public void Init()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnMonthPassed += OnMonthPassed;
    }

    public int GetRemainingQuota() => Mathf.Max(0, currentQuota - auriteSentThisMonth);
    public bool IsQuotaCompleted() => auriteSentThisMonth >= currentQuota;
}