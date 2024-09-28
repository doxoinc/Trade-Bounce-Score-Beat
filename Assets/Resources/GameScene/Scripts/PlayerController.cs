using UnityEngine;
using UnityEngine.UI; // Для UI Text
using UnityEngine.SceneManagement; // Для управления сценами
using TMPro; // Импортируем TextMeshPro (если нужен для других целей)

public class PlayerController : MonoBehaviour
{
    public ControlManager controlManager;

    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private LevelManager levelManager; // Ссылка на LevelManager

    [Header("Счёт игрока")]
    public int score = 0;

    [Header("UI Счёта")]
    public Text scoreText; // Оставляем тип Text

    [Header("UI Жизней")]
    public Text livesText; // Оставляем тип Text

    [Header("Настройки окончания игры")]
    public float fallThreshold = -10f; // Y позиция ниже которой игра заканчивается
    public float gameOverDelay = 2f; // Задержка перед перезагрузкой сцены

    [Header("Жизни")]
    public int maxLives = 3; // Максимальное количество жизней
    private int currentLives;

    [Header("Start Ground")]
    public GameObject startGround; // Ссылка на стартовую линию

    [Header("Game Over Canvas")]
    public GameObject gameOverCanvas; // Ссылка на GameOverCanvas

    private bool hasJumpedOnce = false; // Флаг первого прыжка

    private Vector3 originalScale; // Исходный масштаб персонажа

    void Start()
    {
        // Найти LevelManager в сцене
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogWarning("LevelManager не найден в сцене.");
        }

        rb = GetComponent<Rigidbody2D>();
        if (controlManager == null)
        {
            controlManager = FindObjectOfType<ControlManager>();
        }

        currentLives = maxLives;
        UpdateLivesUI();
        UpdateScoreUI();

        // Убедитесь, что GameOverCanvas неактивен при старте
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameOverCanvas не назначен в PlayerController.");
        }

        // Сброс Time.timeScale на 1 при старте сцены
        Time.timeScale = 1f;

        // Сохранение исходного масштаба
        originalScale = transform.localScale;
    }

    void Update()
    {
        HandleMovement();
        if (controlManager.jump && isGrounded)
        {
            Jump();
        }

        // Проверка позиции игрока для окончания жизни
        if (transform.position.y < fallThreshold)
        {
            LoseLife();
        }
    }

    void HandleMovement()
    {
        float move = 0f;
        if (controlManager.moveLeft)
        {
            move -= 1f;
        }
        if (controlManager.moveRight)
        {
            move += 1f;
        }

        Vector2 velocity = rb.velocity;
        velocity.x = move * moveSpeed;
        rb.velocity = velocity;

        // Поворот персонажа с сохранением исходного масштаба
        if (move != 0)
        {
            Vector3 newScale = originalScale;
            newScale.x = Mathf.Sign(move) * Mathf.Abs(originalScale.x); // Изменение только направления по оси X
            transform.localScale = newScale;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        controlManager.jump = false;

        // Проверяем, был ли уже выполнен первый прыжок
        if (!hasJumpedOnce)
        {
            hasJumpedOnce = true;
            DisableStartGround();
        }
    }

    // Обнаружение столкновений с Ground или Platform
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// Добавляет очки игроку.
    /// </summary>
    /// <param name="points">Количество очков для добавления.</param>
    public void AddScore(int points)
    {
        score += points;
        score = Mathf.Clamp(score, 0, int.MaxValue); // Минимум 0
        Debug.Log("Добавлено " + points + " очков. Текущий счёт: " + score);
        UpdateScoreUI();
        CheckForLevelCompletion();
    }

    /// <summary>
    /// Отнимает очки у игрока.
    /// </summary>
    /// <param name="points">Количество очков для отнимания.</param>
    public void SubtractScore(int points)
    {
        score -= points;
        score = Mathf.Clamp(score, 0, int.MaxValue); // Минимум 0
        Debug.Log("Отнято " + points + " очков. Текущий счёт: " + score);
        UpdateScoreUI();
        CheckForLevelCompletion();
    }

    /// <summary>
    /// Обновляет UI отображение счёта.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        else
        {
            Debug.LogWarning("Score Text не назначен в PlayerController.");
        }
    }

    private void CheckForLevelCompletion()
    {
        if (levelManager != null)
        {
            levelManager.CheckLevelCompletion(score);
        }
        else
        {
            Debug.LogWarning("LevelManager не найден в сцене.");
        }
    }

    /// <summary>
    /// Обновляет UI отображение жизней.
    /// </summary>
    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = currentLives.ToString();
        }
        else
        {
            Debug.LogWarning("Lives Text не назначен в PlayerController.");
        }
    }

    /// <summary>
    /// Вызывает потерю жизни.
    /// </summary>
    void LoseLife()
    {
        currentLives--;
        Debug.Log("Потеряна жизнь. Оставшиеся жизни: " + currentLives);
        UpdateLivesUI();

        if (currentLives > 0)
        {
            Respawn();
        }
        else
        {
            GameOver();
        }
    }

    /// <summary>
    /// Перемещает игрока обратно на стартовую линию.
    /// </summary>
    void Respawn()
    {
        // Перемещаем игрока на позицию стартовой линии
        if (startGround != null)
        {
            // Находим позицию стартовой линии
            Vector3 startPosition = startGround.transform.position + new Vector3(0, startGround.GetComponent<BoxCollider2D>().bounds.size.y / 2 + GetComponent<CircleCollider2D>().bounds.size.y / 2, 0);
            transform.position = startPosition;

            // Восстанавливаем скорость игрока
            rb.velocity = Vector2.zero;

            // Включаем стартовую линию снова
            EnableStartGround();

            // Сбрасываем флаг первого прыжка
            hasJumpedOnce = false;
            Debug.Log("Игрок перезапущен на стартовую линию. Флаг hasJumpedOnce сброшен.");
        }
        else
        {
            Debug.LogWarning("Start Ground не назначен в PlayerController.");
        }
    }

    /// <summary>
    /// Отключает стартовую линию.
    /// </summary>
    void DisableStartGround()
    {
        if (startGround != null)
        {
            startGround.SetActive(false);
            Debug.Log("Стартовая линия отключена.");
        }
        else
        {
            Debug.LogWarning("Start Ground не назначен в PlayerController.");
        }
    }

    /// <summary>
    /// Включает стартовую линию.
    /// </summary>
    void EnableStartGround()
    {
        if (startGround != null)
        {
            startGround.SetActive(true);
            Debug.Log("Стартовая линия включена.");
        }
        else
        {
            Debug.LogWarning("Start Ground не назначен в PlayerController.");
        }
    }

    /// <summary>
    /// Обработка окончания игры.
    /// </summary>
    void GameOver()
    {
        Debug.Log("Игра окончена!");

        // Активируем панель Game Over
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOverCanvas не назначен в PlayerController.");
        }

        // Останавливаем время, чтобы приостановить игру
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Перезагружает текущую сцену.
    /// </summary>
    public void RestartGame()
    {
        // Восстанавливаем время
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    /// <summary>
    /// Переходит в главное меню.
    /// </summary>
    public void GoToMainMenu()
    {
        // Восстанавливаем время
        Time.timeScale = 1f;

        // Замените "SampleScene" на имя вашей сцены главного меню
        SceneManager.LoadScene("SampleScene");
    }

    void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
