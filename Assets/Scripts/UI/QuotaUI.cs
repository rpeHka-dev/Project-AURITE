using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuotaUI : MonoBehaviour
{
    [Header("UI элементы")]
    public TMP_Text valueText;
    public TMP_Text cooldownText;
    public Button sendButton;

    [Header("Панель отправки")]
    public GameObject sendPanel;
    public Slider sendSlider;
    public TMP_Text sliderValueText;
    public Button confirmSendButton;

    [Header("Текст ошибки")]
    public TMP_Text errorText; // <--- НОВОЕ ПОЛЕ

    void Start()
    {
        if (QuotaManager.Instance != null)
        {
            QuotaManager.Instance.OnQuotaChanged += UpdateUI;
            UpdateUI();
        }

        sendButton.onClick.AddListener(OpenSendPanel);
        confirmSendButton.onClick.AddListener(ConfirmSend);
        sendSlider.onValueChanged.AddListener(UpdateSliderText);

        sendPanel.SetActive(false);
        if (errorText != null) errorText.text = "";
    }

    void OnDestroy()
    {
        if (QuotaManager.Instance != null)
            QuotaManager.Instance.OnQuotaChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        var qm = QuotaManager.Instance;
        valueText.text = $"{qm.auriteSentThisMonth} / {qm.currentQuota}";

        if (qm.IsOnCooldown)
        {
            sendButton.interactable = false;
            cooldownText.text = $"Перезарядка: {qm.GetCooldownDaysLeft()} дн.";
        }
        else
        {
            sendButton.interactable = qm.CanSend();
            cooldownText.text = "Готово к отправке";
        }
    }

    void OpenSendPanel()
    {
        var qm = QuotaManager.Instance;
        if (!qm.CanSend()) return;

        sendPanel.SetActive(true);
        if (errorText != null) errorText.text = ""; // Очищаем ошибку при открытии

        int maxSend = qm.GetMaxSendAmount();
        int maxByResources = ResourceManager.Instance.aurite;
        sendSlider.maxValue = Mathf.Min(maxSend, maxByResources);
        sendSlider.value = qm.minSendAmount;

        UpdateSliderText(qm.minSendAmount);
    }

    void UpdateSliderText(float value)
    {
        sliderValueText.text = $"{Mathf.RoundToInt(value)} / {QuotaManager.Instance.GetMaxSendAmount()}";
        if (errorText != null) errorText.text = ""; // Очищаем ошибку при движении ползунка
    }

    void ConfirmSend()
    {
        int amount = Mathf.RoundToInt(sendSlider.value);

        // === ПРОВЕРКА НА НЕХВАТКУ АУРИТА ===
        if (ResourceManager.Instance.aurite < amount)
        {
            if (errorText != null)
                errorText.text = "Не хватает аурита!";
            return; // Не закрываем панель, чтобы игрок мог выбрать меньше
        }

        QuotaManager.Instance.SendAurite(amount);
        sendPanel.SetActive(false);
    }
}