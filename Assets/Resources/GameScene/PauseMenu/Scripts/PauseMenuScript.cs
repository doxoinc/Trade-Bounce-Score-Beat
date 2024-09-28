using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public Button resumeButton;   // Кнопка для закрытия панели
    public Button restartButton;  // Кнопка для перезагрузки сцены
    public Button mainMenuButton; // Кнопка для выхода в главное меню
    public GameObject pauseMenuPanel; // Сама панель

    private void Start()
    {
        // Проверяем, что панель выключена в начале игры
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Настраиваем слушатели для кнопок
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ClosePauseMenu);
        }
        else
        {
            Debug.LogWarning("ResumeButton не назначен в PauseMenuScript.");
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("RestartButton не назначен в PauseMenuScript.");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        else
        {
            Debug.LogWarning("MainMenuButton не назначен в PauseMenuScript.");
        }
    }

    // Закрытие панели и продолжение игры
    public void ClosePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false); // Скрыть панель
            Time.timeScale = 1f; // Возобновить время
        }
    }

    // Перезагрузка текущей сцены
    public void RestartGame()
    {
        Time.timeScale = 1f; // Возобновить время
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex); // Перезагрузить текущую сцену
    }

    // Переход в главное меню
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Возобновить время
        SceneManager.LoadScene("SampleScene"); // Перейти на сцену главного меню (замените "MainMenu" на название вашей сцены)
    }
}
