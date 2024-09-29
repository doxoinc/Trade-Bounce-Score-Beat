using UnityEngine;
using TMPro; // Импортируем TextMeshPro
using System.Collections;

public class Platform : MonoBehaviour
{
    public enum PlatformType { Green, Red }

    [Header("Тип платформы")]
    public PlatformType platformType;

    [Header("Возможные значения очков")]
    public int[] possiblePoints = new int[] { 10, 25, 75, 90, 140 }; // Массив возможных очков

    [Header("Максимальные очки")]
    public int maxPoints;

    [Header("Скорость движения платформы вниз")]
    public float moveSpeed = 2f;

    [Header("UI Текст очков")]
    public TextMeshPro pointText; // Используем TextMeshPro для отображения текста

    [Header("Префаб Floating Text")]
    public GameObject floatingTextPrefab; // Ссылка на префаб FloatingText

    [Header("Canvas для Floating Text")]
    public Canvas floatingTextCanvas; // Ссылка на Canvas для размещения FloatingText

    [Header("Цвет платформы при исчерпании очков")]
    public Color depletedColor = Color.gray; // Настраиваемый цвет платформы после исчерпания очков

    private bool playerOnPlatform = false;
    private bool pointsDepleted = false; // Флаг для проверки, что очки исчерпаны
    private Coroutine pointCoroutine;
    private PlayerController playerController;

    void Start()
    {
        // Поиск Canvas в сцене, если он не назначен
        if (floatingTextCanvas == null)
        {
            floatingTextCanvas = FindObjectOfType<Canvas>();
            if (floatingTextCanvas == null)
            {
                Debug.LogWarning("Canvas не найден на сцене. Убедитесь, что Canvas присутствует.");
            }
        }

        // Присваиваем случайное значение из возможных очков
        maxPoints = possiblePoints[Random.Range(0, possiblePoints.Length)];

        // Инициализация Rigidbody2D для движения платформы
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        rb.velocity = Vector2.down * moveSpeed;

        // Инициализация UI Текста
        if (pointText != null)
        {
            pointText.text = maxPoints.ToString();
        }
        else
        {
            Debug.LogWarning("PointText не назначен в Platform.cs.");
        }

        // Убедимся, что цвет платформы соответствует её типу
        SetPlatformColor();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !pointsDepleted) // Проверяем, что очки не обнулены
        {
            playerOnPlatform = true;
            playerController = collision.collider.GetComponent<PlayerController>();
            if (playerController != null && maxPoints > 0)
            {
                //Debug.Log("Игрок сел на платформу: " + platformType);
                pointCoroutine = StartCoroutine(AccumulatePoints());
            }
            else
            {
                Debug.LogWarning("PlayerController не найден или maxPoints <= 0.");
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = false;
            Debug.Log("Игрок покинул платформу: " + platformType);
            if (pointCoroutine != null)
            {
                StopCoroutine(pointCoroutine);
                pointCoroutine = null;
            }
            playerController = null;
        }
    }

    IEnumerator AccumulatePoints()
    {
        float pointInterval = 0.1f; // Интервал в секундах для начисления 1 очка
        Debug.Log("Coroutine AccumulatePoints запущен для " + platformType);

        while (playerOnPlatform && playerController != null && maxPoints > 0)
        {
            // Ждём интервал перед начислением очков
            yield return new WaitForSeconds(pointInterval);

            if (!playerOnPlatform || playerController == null)
            {
                Debug.Log("Coroutine AccumulatePoints прерван для " + platformType);
                break;
            }

            int pointsThisFrame = 1; // Начисляем 1 очко за интервал

            if (platformType == PlatformType.Green)
            {
                // Добавляем очки игроку
                playerController.AddScore(pointsThisFrame);
                // Воспроизведение звука получения очков
                if (AudioManager.Instance != null && AudioManager.Instance.collectCoinClip != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.collectCoinClip);
                }
                else
                {
                    Debug.LogWarning("AudioManager.Instance или collectCoinClip не назначены.");
                }

                // Отображение визуального эффекта
                CreateFloatingText(pointsThisFrame, true); // true для добавления

                // Уменьшаем очки на платформе
                maxPoints -= pointsThisFrame;
                if (maxPoints < 0) maxPoints = 0;

                // Обновляем UI Текста
                if (pointText != null)
                {
                    pointText.text = maxPoints.ToString();
                }

                // Проверяем, если очки платформы закончились
                if (maxPoints == 0)
                {
                    // Меняем цвет платформы на серый и устанавливаем флаг
                    pointsDepleted = true;
                    ChangePlatformColorToGray();

                    // Воспроизведение звука исчерпания очков
                    if (AudioManager.Instance != null && AudioManager.Instance.removeGlassesClip != null)
                    {
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.removeGlassesClip);
                    }
                    else
                    {
                        Debug.LogWarning("AudioManager.Instance или removeGlassesClip не назначены.");
                    }

                    break;
                }
            }
            else if (platformType == PlatformType.Red)
            {
                // Отнимаем очки у игрока
                playerController.SubtractScore(pointsThisFrame);
                // Воспроизведение звука потери очков
                if (AudioManager.Instance != null && AudioManager.Instance.loseHeartClip != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.loseHeartClip);
                }
                else
                {
                    Debug.LogWarning("AudioManager.Instance или loseHeartClip не назначены.");
                }

                // Отображение визуального эффекта
                CreateFloatingText(pointsThisFrame, false); // false для отнимания

                // Уменьшаем очки на платформе
                maxPoints -= pointsThisFrame;
                if (maxPoints < 0) maxPoints = 0;

                // Обновляем UI Текста
                if (pointText != null)
                {
                    pointText.text = maxPoints.ToString();
                }

                // Проверяем, если очки платформы закончились
                if (maxPoints == 0)
                {
                    // Меняем цвет платформы на серый и устанавливаем флаг
                    pointsDepleted = true;
                    ChangePlatformColorToGray();

                    // Воспроизведение звука исчерпания очков
                    if (AudioManager.Instance != null && AudioManager.Instance.removeGlassesClip != null)
                    {
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.removeGlassesClip);
                    }
                    else
                    {
                        Debug.LogWarning("AudioManager.Instance или removeGlassesClip не назначены.");
                    }

                    break;
                }
            }
        }
    }

    void ChangePlatformColorToGray()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); // Используем SpriteRenderer для изменения цвета
        if (spriteRenderer != null)
        {
            spriteRenderer.color = depletedColor; // Меняем цвет спрайта на заданный
        }
        else
        {
            Debug.LogWarning("SpriteRenderer не найден на платформе.");
        }
    }

    void CreateFloatingText(int points, bool isAdding)
    {
        // Убираем проверку на точки для красных платформ, чтобы текст всегда отображался
        if (floatingTextPrefab != null && floatingTextCanvas != null)
        {
            // Определяем позицию над игроком с добавлением рандомного смещения
            float randomOffsetX = Random.Range(-0.5f, 0.5f); // Случайное смещение по X
            float randomOffsetY = Random.Range(0.5f, 1.5f);  // Случайное смещение по Y

            Vector3 spawnPosition = transform.position + new Vector3(randomOffsetX, randomOffsetY, 0); // Рандомная позиция рядом с игроком

            // Преобразуем мировую позицию в экранную
            Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPosition);

            // Инстанцируем префаб FloatingText
            GameObject floatingTextObj = Instantiate(floatingTextPrefab, floatingTextCanvas.transform);

            // Устанавливаем позицию
            floatingTextObj.transform.position = screenPos;

            // Настройка текста
            TextMeshPro ftText = floatingTextObj.GetComponent<TextMeshPro>();
            if (ftText != null)
            {
                if (isAdding)
                {
                    ftText.text = "+" + points.ToString();
                    ftText.color = Color.green; // Зеленый цвет для добавления
                }
                else
                {
                    ftText.text = "-" + points.ToString();
                    ftText.color = Color.red; // Красный цвет для отнимания
                }
            }
            else
            {
                Debug.LogWarning("TextMeshPro компонент не найден на FloatingText префабе.");
            }

            // Добавьте здесь анимацию или логику исчезновения текста, если необходимо
        }
        else
        {
            Debug.LogWarning("FloatingTextPrefab или FloatingTextCanvas не назначены в Platform.cs.");
        }
    }

    void SetPlatformColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (platformType == PlatformType.Green)
            {
                spriteRenderer.color = Color.green; // Зеленый цвет для зеленых платформ
            }
            else if (platformType == PlatformType.Red)
            {
                spriteRenderer.color = Color.red; // Красный цвет для красных платформ
            }
        }
        else
        {
            Debug.LogWarning("SpriteRenderer не найден на платформе.");
        }
    }

    void OnBecameInvisible()
    {
        // Уничтожаем платформу, когда она выходит за пределы экрана
        Destroy(gameObject);
    }
}
