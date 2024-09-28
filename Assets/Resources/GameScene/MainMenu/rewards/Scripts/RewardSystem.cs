using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance; // Singleton для доступа из других скриптов

    [Header("UI Elements")]
    public GameObject rewardPanel; // Панель наград
    public Image rewardButtonImage; // Изображение кнопки наград
    public TextMeshProUGUI timerText; // Текст для отображения таймера
    public TextMeshProUGUI rewardAmountText; // Текст для отображения количества монет
    public Button rewardButton; // Кнопка, открывающая панель наград
    public Sprite rewardAvailableSprite; // Спрайт кнопки, когда награда доступна
    public Sprite rewardUnavailableSprite; // Спрайт кнопки, когда награда недоступна

    private RewardDataManager dataManager;
    private TimeSpan rewardInterval;

    void Awake()
    {
        // Реализуем паттерн Singleton
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Убедитесь, что RewardSystem только в главном меню
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        dataManager = RewardDataManager.Instance;
        if (dataManager == null)
        {
            Debug.LogError("RewardDataManager не найден в сцене.");
            return;
        }

        rewardInterval = dataManager.GetRewardInterval();
        UpdateRewardButton();
    }

    void Update()
    {
        if (!IsRewardAvailable())
        {
            TimeSpan timeRemaining = GetTimeUntilNextReward();
            // Убедимся, что время не отрицательное
            timeRemaining = timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                Math.Max(timeRemaining.Hours, 0),
                Math.Max(timeRemaining.Minutes, 0),
                Math.Max(timeRemaining.Seconds, 0));
        }
        else
        {
            timerText.text = "00:00:00";
        }
    }

    /// <summary>
    /// Открывает панель наград.
    /// </summary>
    public void OpenRewardPanel()
    {
        if (IsRewardAvailable())
        {
            rewardPanel.SetActive(true);
            UpdateRewardButton();

            // Определяем, какую награду получит игрок
            int rewardToShow = dataManager.hasCollectedFirstReward ? dataManager.regularRewardAmount : dataManager.initialRewardAmount;
            UpdateRewardAmountText(rewardToShow, true); // true указывает, что это предварительное отображение награды

            Debug.Log("RewardPanel открыта.");
        }
        else
        {
            Debug.Log("Награда ещё недоступна. RewardPanel не открыта.");
        }
    }

    /// <summary>
    /// Закрывает панель наград.
    /// </summary>
    public void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);
        Debug.Log("RewardPanel закрыта.");
    }

    /// <summary>
    /// Получает награду, добавляет монеты и обновляет время последней награды.
    /// </summary>
    public void CollectReward()
    {
        if (IsRewardAvailable())
        {
            int rewardAmount = 0;
            if (!dataManager.hasCollectedFirstReward)
            {
                // Первоначальная награда
                rewardAmount = dataManager.initialRewardAmount;
            }
            else
            {
                // Регулярная награда
                rewardAmount = dataManager.regularRewardAmount;
            }

            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoins(rewardAmount);
                Debug.Log($"Собрана награда: {rewardAmount} монет.");
            }
            else
            {
                Debug.LogError("CoinManager.Instance равен null. Награда не добавлена.");
            }

            if (!dataManager.hasCollectedFirstReward)
            {
                dataManager.hasCollectedFirstReward = true;
            }

            dataManager.lastRewardTime = DateTime.UtcNow;
            dataManager.SaveData();
            UpdateRewardButton();

            // Обновляем текст количества монет в панели наград
            UpdateRewardAmountText(rewardAmount, false); // false указывает, что это окончательное отображение награды

            Debug.Log("Награда успешно собрана.");
        }
        else
        {
            Debug.Log("Награда ещё недоступна.");
        }
    }

    /// <summary>
    /// Проверяет, доступна ли награда.
    /// </summary>
    /// <returns>True, если награда доступна.</returns>
    public bool IsRewardAvailable()
    {
        if (!dataManager.hasCollectedFirstReward)
        {
            return true;
        }

        DateTime nextRewardTime = dataManager.lastRewardTime.Add(rewardInterval);
        return DateTime.UtcNow >= nextRewardTime;
    }

    /// <summary>
    /// Получает время до следующей награды.
    /// </summary>
    /// <returns>TimeSpan до следующей награды.</returns>
    public TimeSpan GetTimeUntilNextReward()
    {
        if (!dataManager.hasCollectedFirstReward)
        {
            return TimeSpan.Zero;
        }

        DateTime nextRewardTime = dataManager.lastRewardTime.Add(rewardInterval);
        return nextRewardTime - DateTime.UtcNow;
    }

    /// <summary>
    /// Обновляет состояние кнопки наград.
    /// </summary>
    private void UpdateRewardButton()
    {
        bool rewardAvailable = IsRewardAvailable();

        if (rewardAvailable)
        {
            rewardButtonImage.sprite = rewardAvailableSprite;
            timerText.gameObject.SetActive(false);
            rewardButton.interactable = true;
            Debug.Log("RewardButton активен и отображает доступную награду.");
        }
        else
        {
            rewardButtonImage.sprite = rewardUnavailableSprite;
            timerText.gameObject.SetActive(true);
            rewardButton.interactable = false;
            Debug.Log("RewardButton неактивен и отображает таймер.");
        }

        // При доступной награде показываем, сколько монет будет получено
        if (rewardAvailable)
        {
            int rewardToShow = dataManager.hasCollectedFirstReward ? dataManager.regularRewardAmount : dataManager.initialRewardAmount;
            UpdateRewardAmountText(rewardToShow, true); // true указывает, что это предварительное отображение награды
        }
        else
        {
            // При недоступной награде показываем количество монет, которое получит игрок после окончания таймера
            int rewardToShow = dataManager.hasCollectedFirstReward ? dataManager.regularRewardAmount : dataManager.initialRewardAmount;
            UpdateRewardAmountText(rewardToShow, true); // true указывает, что это предварительное отображение награды
        }
    }

    /// <summary>
    /// Обновляет текст с количеством монет в панели наград.
    /// </summary>
    /// <param name="amount">Количество монет для отображения.</param>
    /// <param name="isPreliminary">Указывает, показывать ли предварительную информацию о награде.</param>
    private void UpdateRewardAmountText(int amount, bool isPreliminary)
    {
        if (rewardAmountText != null)
        {
            if (isPreliminary)
            {
                rewardAmountText.text = $"Вы получите {amount} монет!";
            }
            else
            {
                rewardAmountText.text = $"Вы заработали {amount} монет!";
            }
            Debug.Log($"Обновление текста награды: {rewardAmountText.text}");
        }
        else
        {
            Debug.LogWarning("rewardAmountText не назначен в RewardSystem.");
        }
    }

    /// <summary>
    /// Сбрасывает таймер наград и флаг первой награды (для разработчика).
    /// </summary>
    public void ResetRewardTimer()
    {
        dataManager.ResetReward();
        UpdateRewardButton();
        Debug.Log("Таймер наград и флаг первой награды сброшены.");
    }

    /// <summary>
    /// Добавляет [ContextMenu] для сброса таймера из инспектора.
    /// </summary>
    [ContextMenu("Reset Reward Timer")]
    public void ResetRewardTimerFromContextMenu()
    {
        ResetRewardTimer();
    }
}
