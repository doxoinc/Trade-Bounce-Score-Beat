using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance; // Singleton для доступа из других скриптов

    [Header("Initial Settings")]
    public int startingCoins = 0; // Начальное количество монет (установлено на 0)

    private int playerCoins;

    // Событие, вызываемое при изменении количества монет
    public delegate void OnCoinsChanged(int newCoinAmount);
    public event OnCoinsChanged CoinsChanged;

    void Awake()
    {
        // Реализуем паттерн Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохранять объект при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadCoins();
        NotifyCoinsChanged();
    }

    /// <summary>
    /// Получает текущее количество монет игрока.
    /// </summary>
    /// <returns>Количество монет.</returns>
    public int GetCoins()
    {
        return playerCoins;
    }

    /// <summary>
    /// Добавляет монеты игроку.
    /// </summary>
    /// <param name="amount">Количество монет для добавления.</param>
    public void AddCoins(int amount)
    {
        playerCoins += amount;
        SaveCoins();
        NotifyCoinsChanged();
        Debug.Log($"Добавлено {amount} монет. Текущий баланс: {playerCoins}");
    }

    /// <summary>
    /// Отнимает монеты у игрока.
    /// </summary>
    /// <param name="amount">Количество монет для отнимания.</param>
    /// <returns>Успешно ли отнять монеты.</returns>
    public bool SpendCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            SaveCoins();
            NotifyCoinsChanged();
            Debug.Log($"Отнято {amount} монет. Текущий баланс: {playerCoins}");
            return true;
        }
        else
        {
            Debug.Log("Недостаточно монет.");
            return false;
        }
    }

    /// <summary>
    /// Сохраняет количество монет в PlayerPrefs.
    /// </summary>
    private void SaveCoins()
    {
        PlayerPrefs.SetInt("PlayerCoins", playerCoins);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Загружает количество монет из PlayerPrefs.
    /// </summary>
    private void LoadCoins()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", startingCoins);
    }

    /// <summary>
    /// Вызывает событие изменения количества монет.
    /// </summary>
    private void NotifyCoinsChanged()
    {
        CoinsChanged?.Invoke(playerCoins);
    }

    /// <summary>
    /// Сбрасывает все сохранённые данные игрока.
    /// </summary>
    [ContextMenu("Reset Player Data")]
    public void ResetPlayerData()
    {
        // Очищаем все сохранённые данные
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Все PlayerPrefs очищены.");

        // Сброс количества монет до начального значения
        playerCoins = startingCoins;
        SaveCoins();
        NotifyCoinsChanged();
        Debug.Log($"Монеты сброшены до начального значения: {playerCoins}");

        // Найдите и сбросьте состояние покупок в StoreManager
        StoreManager storeManager = FindObjectOfType<StoreManager>();
        if (storeManager != null)
        {
            storeManager.ResetPurchases();
            Debug.Log("Состояние покупок сброшено.");
        }
        else
        {
            Debug.LogWarning("StoreManager не найден в сцене. Покупки не сброшены.");
        }
    }
}
