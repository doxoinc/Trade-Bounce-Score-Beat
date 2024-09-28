using UnityEngine;
using TMPro;

public class CoinUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI coinText; // Текст для отображения количества монет

    void OnEnable()
    {
        // Подписываемся на событие изменения монет
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged += UpdateCoinText;
            // Обновляем текст сразу при активации
            UpdateCoinText(CoinManager.Instance.GetCoins());
        }
        else
        {
            Debug.LogError("CoinManager.Instance равен null. Убедитесь, что CoinManager присутствует в сцене.");
        }
    }

    void OnDisable()
    {
        // Отписываемся от события при деактивации
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CoinsChanged -= UpdateCoinText;
        }
    }

    /// <summary>
    /// Обновляет текст с количеством монет.
    /// </summary>
    /// <param name="newCoinAmount">Новое количество монет.</param>
    private void UpdateCoinText(int newCoinAmount)
    {
        if (coinText != null)
        {
            coinText.text = newCoinAmount.ToString();
            Debug.Log($"Обновление UI: Монеты: {newCoinAmount}");
        }
        else
        {
            Debug.LogWarning("coinText не назначен в CoinUIManager.");
        }
    }
}
