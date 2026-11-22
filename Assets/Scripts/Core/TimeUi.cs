using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public TMP_Text timeText;

    void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged += UpdateUI;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (timeText != null && TimeManager.Instance != null)
            timeText.text = TimeManager.Instance.GetTimeString();
    }
}