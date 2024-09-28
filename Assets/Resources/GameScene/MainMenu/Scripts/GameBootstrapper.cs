using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    public CoinManager coinManagerPrefab;

    void Awake()
    {
        // Проверяем, существует ли уже CoinManager
        if (CoinManager.Instance == null && coinManagerPrefab != null)
        {
            Instantiate(coinManagerPrefab);
            Debug.Log("CoinManager инициализирован из префаба.");
        }
    }
}
