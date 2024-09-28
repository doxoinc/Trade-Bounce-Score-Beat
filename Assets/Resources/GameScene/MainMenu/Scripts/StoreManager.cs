using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    [System.Serializable]
    public class StoreItem
    {
        public string itemName;
        public int price;
        public bool isPurchased;
        public Button purchaseButton;
        public Image itemImage;
    }

    public StoreItem[] skins;
    public TextMeshProUGUI playerMoneyText;

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
        LoadPurchases();
        UpdateUI();
    }

    /// <summary>
    /// Покупка предмета по индексу.
    /// </summary>
    /// <param name="itemIndex">Индекс предмета в массиве skins.</param>
    public void PurchaseItem(int itemIndex)
    {
        // Проверка на корректность индекса
        if (itemIndex < 0 || itemIndex >= skins.Length)
        {
            Debug.LogError($"Invalid itemIndex: {itemIndex}");
            return;
        }

        StoreItem item = skins[itemIndex];

        if (!item.isPurchased)
        {
            if (CoinManager.Instance != null)
            {
                bool success = CoinManager.Instance.SpendCoins(item.price);
                if (success)
                {
                    item.isPurchased = true;

                    // Сохраняем покупку
                    PlayerPrefs.SetInt(item.itemName, 1);
                    PlayerPrefs.Save();

                    UpdateUI();
                    Debug.Log($"Предмет '{item.itemName}' куплен за {item.price} монет.");
                }
                else
                {
                    Debug.Log("Недостаточно монет для покупки.");
                }
            }
            else
            {
                Debug.LogError("CoinManager.Instance равен null. Убедитесь, что CoinManager присутствует в сцене.");
            }
        }
        else
        {
            Debug.Log("Товар уже куплен.");
        }
    }

    /// <summary>
    /// Загружает информацию о покупках из PlayerPrefs.
    /// </summary>
    private void LoadPurchases()
    {
        foreach (var item in skins)
        {
            item.isPurchased = PlayerPrefs.GetInt(item.itemName, 0) == 1;
        }
    }

    /// <summary>
    /// Обновляет интерфейс магазина.
    /// </summary>
    private void UpdateUI()
    {
        // Обновление состояния кнопок покупки
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i].isPurchased)
            {
                skins[i].purchaseButton.interactable = false;
                TextMeshProUGUI buttonText = skins[i].purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "Куплено";
                }
                else
                {
                    Debug.LogWarning($"TextMeshProUGUI не найден на кнопке покупки предмета '{skins[i].itemName}'.");
                }
            }
            else
            {
                skins[i].purchaseButton.interactable = true;
                TextMeshProUGUI buttonText = skins[i].purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "Купить";
                }
                else
                {
                    Debug.LogWarning($"TextMeshProUGUI не найден на кнопке покупки предмета '{skins[i].itemName}'.");
                }
            }
        }
    }

    /// <summary>
    /// Обновляет текст количества монет в UI.
    /// </summary>
    /// <param name="newCoinAmount">Новое количество монет.</param>
    private void UpdatePlayerMoneyText(int newCoinAmount)
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = "Монеты: " + newCoinAmount.ToString();
            Debug.Log($"Обновление UI: Монеты: {newCoinAmount}");
        }
        else
        {
            Debug.LogWarning("playerMoneyText не назначен в StoreManager.");
        }
    }

    /// <summary>
    /// Метод для добавления монет (например, из consumable покупки).
    /// </summary>
    /// <param name="amount">Количество монет для добавления.</param>
    public void AddMoney(int amount)
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(amount);
            // UpdateUI(); // Не нужно, так как обновление происходит через событие
        }
        else
        {
            Debug.LogError("CoinManager.Instance равен null в AddMoney.");
        }
    }

    /// <summary>
    /// Сбрасывает все покупки до состояния не куплено.
    /// </summary>
    public void ResetPurchases()
    {
        foreach (var item in skins)
        {
            item.isPurchased = false;
            PlayerPrefs.SetInt(item.itemName, 0);
        }
        PlayerPrefs.Save();
        UpdateUI();
        Debug.Log("Все покупки сброшены.");
    }
}
