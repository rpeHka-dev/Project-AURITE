using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Настройки времени")]
    [Tooltip("Сколько реальных секунд длится 1 игровой день")]
    public float secondsPerDay = 5f;

    [Header("Текущее время")]
    public int currentDay = 1;

    [Header("Управление")]
    public bool IsPaused { get; private set; } = false;
    public float GameSpeed { get; private set; } = 1f;

    [Header("События")]
    public System.Action OnDayPassed;
    public System.Action OnTimeChanged;

    private float dayTimer = 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (IsPaused) return;

        dayTimer += Time.deltaTime * GameSpeed;

        if (dayTimer >= secondsPerDay)
        {
            dayTimer -= secondsPerDay;
            PassDay();
        }
    }

    void PassDay()
    {
        currentDay++;

        OnDayPassed?.Invoke();
        OnTimeChanged?.Invoke();

        Debug.Log($"День {currentDay}");
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Debug.Log(IsPaused ? "Пауза" : "Игра продолжается");
    }

    public void SetSpeed(float speed)
    {
        GameSpeed = speed;
        Debug.Log($"Скорость: {speed}x");
    }

    public void SetSpeedNormal() => SetSpeed(1f);
    public void SetSpeedFast() => SetSpeed(2f);

    public string GetTimeString()
    {
        return $"День {currentDay}";
    }
}