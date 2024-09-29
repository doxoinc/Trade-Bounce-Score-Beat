using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [System.Serializable]
    public class StoreItem
    {
        public string itemName;        // Название предмета
        public int coinAmount;         // Количество монет для добавления
        public Button purchaseButton;  // Кнопка покупки/получения
        public Image itemImage;        // Изображение предмета
    }

    public StoreItem[] coinItems;           // Массив предметов в магазине
    public TextMeshProUGUI playerMoneyText; // UI элемент для отображения монет игрока

    private void OnEnable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged += UpdatePlayerMoneyText;
            UpdatePlayerMoneyText(CoinManager.Instance.GetCoins());
        }
        else
        {
            Debug.LogError("CoinManager.Instance равен null в OnEnable.");
        }
    }

    private void OnDisable()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged -= UpdatePlayerMoneyText;
        }
    }

    private void Start()
    {
        InitializeStoreItems();
        UpdateUI();
    }

    /// <summary>
    /// Инициализирует предметы магазина, добавляя слушатели на кнопки.
    /// </summary>
    private void InitializeStoreItems()
    {
        for (int i = 0; i < coinItems.Length; i++)
        {
            if (coinItems[i].purchaseButton != null)
            {
                int index = i; // Локальная копия индекса для использования в лямбда-функции
                coinItems[i].purchaseButton.onClick.AddListener(() => AddCoins(index));
            }
            else
            {
                Debug.LogWarning($"purchaseButton не назначен для предмета '{coinItems[i].itemName}'.");
            }
        }
    }

    /// <summary>
    /// Добавляет монеты игроку при нажатии на предмет.
    /// </summary>
    /// <param name="itemIndex">Индекс предмета в массиве coinItems.</param>
    private void AddCoins(int itemIndex)
    {
        // Проверка на корректность индекса
        if (itemIndex < 0 || itemIndex >= coinItems.Length)
        {
            Debug.LogError($"Invalid itemIndex: {itemIndex}");
            return;
        }

        StoreItem item = coinItems[itemIndex];

        if (item.coinAmount > 0)
        {
            // Добавляем монеты игроку
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoins(item.coinAmount);
                Debug.Log($"Добавлено {item.coinAmount} монет игроку через магазин '{item.itemName}'.");
            }
            else
            {
                Debug.LogError("CoinManager.Instance равен null. Убедитесь, что CoinManager присутствует в сцене.");
                return;
            }

            // Воспроизведение звука получения монет
            if (AudioManager.Instance != null && AudioManager.Instance.collectCoinClip != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.collectCoinClip);
            }
            else
            {
                Debug.LogWarning("AudioManager.Instance или collectCoinClip не назначены.");
            }

            // (Опционально) Добавьте визуальные эффекты, уведомления или анимации здесь

            // Обновляем UI (монеты обновятся автоматически через событие)
        }
        else
        {
            Debug.LogWarning($"Предмет '{item.itemName}' имеет нулевое количество монет.");
        }
    }

    /// <summary>
    /// Обновляет интерфейс магазина.
    /// </summary>
    private void UpdateUI()
    {
        // Обновление текста количества монет происходит через событие CoinsChanged
        // Здесь можно добавить дополнительную логику, если необходимо
    }

    /// <summary>
    /// Обновляет текст UI с количеством монет.
    /// </summary>
    /// <param name="newCoinAmount">Новое количество монет.</param>
    private void UpdatePlayerMoneyText(int newCoinAmount)
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = newCoinAmount.ToString();
            Debug.Log($"Обновление UI: Монеты: {newCoinAmount}");
        }
        else
        {
            Debug.LogWarning("playerMoneyText не назначен в StoreManager.");
        }
    }

    /// <summary>
    /// Сбрасывает все покупки до состояния не куплено.
    /// </summary>
    public void ResetPurchases()
    {
        foreach (var item in coinItems)
        {
            // В вашей текущей реализации покупки не требуется, так как предметы дают монеты бесплатно
            // Однако, если вы хотите, чтобы предметы могли быть использованы только один раз, можно сбросить состояния
            // Например, сделать кнопки снова активными, если они были деактивированы
            // В текущей реализации это не требуется, поэтому метод может быть пустым или содержать соответствующую логику

            // Пример сброса состояния кнопок:
            // item.isPurchased = false; // Если вы используете подобную логику
            // item.purchaseButton.interactable = true;
        }
        // Сохранение изменений, если необходимо
        PlayerPrefs.Save();
        Debug.Log("Все покупки сброшены.");
    }
}
