using UnityEngine;
using System.Collections;

public class SettingsButtonScript : MonoBehaviour
{
    public GameObject pauseMenuPanel; // Панель PauseMenu, которую нужно открыть
    public Animator pauseMenuAnimator; // Animator для панели паузы

    // Длительность анимации открытия и закрытия (в секундах)
    public float animationDuration = 1f;

    public void OpenPauseMenu()
    {
        if (pauseMenuPanel != null && pauseMenuAnimator != null)
        {
            // Включить панель (если она была скрыта)
            pauseMenuPanel.SetActive(true);

            // Запустить анимацию открытия
            pauseMenuAnimator.SetTrigger("Open");

            // Запустить Coroutine для приостановки игры после завершения анимации
            StartCoroutine(PauseGame());
        }
        else
        {
            Debug.LogWarning("PauseMenuPanel или PauseMenuAnimator не назначены в SettingsButtonScript.");
        }
    }

    IEnumerator PauseGame()
    {
        // Ждем длительность анимации открытия
        yield return new WaitForSecondsRealtime(animationDuration);

        // Ставим игру на паузу
        Time.timeScale = 0f;
    }

    public void ClosePauseMenu()
    {
        if (pauseMenuPanel != null && pauseMenuAnimator != null)
        {
            // Запустить анимацию закрытия
            pauseMenuAnimator.SetTrigger("Close");

            // Запустить Coroutine для возобновления игры после завершения анимации
            StartCoroutine(ResumeGame());
        }
        else
        {
            Debug.LogWarning("PauseMenuPanel или PauseMenuAnimator не назначены в SettingsButtonScript.");
        }
    }

    IEnumerator ResumeGame()
    {
        // Ждем длительность анимации закрытия
        yield return new WaitForSecondsRealtime(animationDuration);

        // Возобновить игру
        Time.timeScale = 1f;

        // Скрыть панель паузы
        pauseMenuPanel.SetActive(false);
    }

    // Методы для анимационных событий (опционально)
    public void OnPauseMenuOpened()
    {
        Time.timeScale = 0f; // Остановить игру после завершения анимации
    }

    public void OnPauseMenuClosed()
    {
        Time.timeScale = 1f; // Возобновить игру после завершения анимации
        pauseMenuPanel.SetActive(false); // Скрыть панель паузы
    }
}
