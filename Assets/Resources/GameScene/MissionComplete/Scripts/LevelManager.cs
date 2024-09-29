using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Для работы с TextMeshPro

public class LevelManager : MonoBehaviour
{
    [Header("Mission Complete Panel")]
    public GameObject missionCompletePanel; // Панель MissionComplete
    public Animator missionCompleteAnimator; // Animator панели MissionComplete
    public TextMeshProUGUI coinsEarnedText; // Текст для отображения заработанных монет

    [Header("Level Settings")]
    public int targetScore = 200; // Цель по очкам для завершения уровня
    public int coinsPerLevel = 100; // Количество монет за прохождение уровня

    private bool levelCompleted = false;

    void Start()
    {
        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(false); // Скрыть панель при старте
            Debug.Log("MissionCompletePanel инициализирован и скрыт.");
        }
        else
        {
            Debug.LogWarning("MissionCompletePanel не назначен в LevelManager.");
        }

        if (missionCompleteAnimator == null && missionCompletePanel != null)
        {
            missionCompleteAnimator = missionCompletePanel.GetComponent<Animator>();
            if (missionCompleteAnimator != null)
            {
                Debug.Log("Animator найден и назначен.");
            }
            else
            {
                Debug.LogWarning("Animator не найден на MissionCompletePanel.");
            }
        }

        if (coinsEarnedText == null && missionCompletePanel != null)
        {
            coinsEarnedText = missionCompletePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (coinsEarnedText != null)
            {
                Debug.Log("TextMeshProUGUI для монет найден и назначен.");
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI для монет не найден на MissionCompletePanel.");
            }
        }
    }

    public void CheckLevelCompletion(int currentScore)
    {
        Debug.Log($"Проверка завершения уровня: текущий счёт = {currentScore}, целевой счёт = {targetScore}");
        if (!levelCompleted && currentScore >= targetScore)
        {
            Debug.Log("Цель по очкам достигнута. Завершение уровня.");
            levelCompleted = true;
            StartCoroutine(CompleteLevel());
        }
    }

    IEnumerator CompleteLevel()
    {
        Debug.Log("Начало завершения уровня.");
        if (missionCompletePanel != null)
        {
            if (CoinManager.Instance != null)
            {
                // Добавляем монеты за прохождение уровня
                CoinManager.Instance.AddCoins(coinsPerLevel);
                Debug.Log($"Добавлено {coinsPerLevel} монет за прохождение уровня.");

                // Воспроизведение звука получения монет
                if (AudioManager.Instance != null && AudioManager.Instance.collectCoinClip != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.collectCoinClip);
                }
                else
                {
                    Debug.LogWarning("AudioManager.Instance или collectCoinClip не назначены.");
                }
            }
            else
            {
                Debug.LogError("CoinManager.Instance равен null. Убедитесь, что CoinManager присутствует в сцене.");
                yield break; // Прерываем корутину, чтобы избежать дальнейших ошибок
            }

            // Обновляем текст с количеством заработанных монет
            if (coinsEarnedText != null)
            {
                coinsEarnedText.text = coinsPerLevel.ToString();
            }
            else
            {
                Debug.LogError("coinsEarnedText равен null. Убедитесь, что TextMeshProUGUI назначен в LevelManager.");
            }

            // Включаем панель Mission Complete
            missionCompletePanel.SetActive(true);
            Debug.Log($"Панель активирована: {missionCompletePanel.activeSelf}");

            // Воспроизведение анимации открытия панели
            if (missionCompleteAnimator != null)
            {
                missionCompleteAnimator.SetTrigger("Open"); // Запустить анимацию открытия
                Debug.Log("Триггер 'Open' активирован.");

                // Воспроизведение звука завершения уровня (например, звук аплодисментов)
                if (AudioManager.Instance != null && AudioManager.Instance.gameOverClip != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverClip);
                }
                else
                {
                    Debug.LogWarning("AudioManager.Instance или gameOverClip не назначены.");
                }
            }
            else
            {
                Debug.LogWarning("Animator не назначен.");
            }

            // Воспроизведение фоновой музыки Mission Complete (если требуется)
            if (AudioManager.Instance != null && AudioManager.Instance.missionCompleteMusic != null)
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.missionCompleteMusic);
            }

            // Ждём 1 секунду перед изменением timeScale
            yield return new WaitForSecondsRealtime(1f);

            float duration = 1f;
            float elapsed = 0f;
            float initialTimeScale = 1f;
            float targetTimeScale = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float newTimeScale = Mathf.Lerp(initialTimeScale, targetTimeScale, elapsed / duration);
                Time.timeScale = newTimeScale;
                yield return null;
            }

            Time.timeScale = 0f; // Убедимся, что timeScale точно равен 0
            Debug.Log("Уровень завершён, Time.timeScale установлено на 0.");
        }
        else
        {
            Debug.LogWarning("MissionCompletePanel не назначен.");
        }
    }

    public void ExitToMainMenu()
    {
        Debug.Log("Выход в главное меню.");
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    public void PlayAgain()
    {
        Debug.Log("Перезапуск текущей сцены.");
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void NextLevel()
    {
        Debug.Log("Переход на следующий уровень.");
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Нет следующей сцены для загрузки.");
        }
    }
}
