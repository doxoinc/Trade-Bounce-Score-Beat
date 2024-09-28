using UnityEngine;
using TMPro; // Импортируем TextMeshPro

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeDuration = 1f;

    private TextMeshProUGUI floatingText;
    private Color originalColor;
    private float timer = 0f;

    void Awake()
    {
        floatingText = GetComponent<TextMeshProUGUI>(); // Изменили на TextMeshProUGUI
        if (floatingText != null)
        {
            originalColor = floatingText.color;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI компонент не найден на FloatingText.");
        }
    }

    void Update()
    {
        // Поднимаем текст вверх
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Затухание текста
        if (floatingText != null)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, timer / fadeDuration);
            floatingText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Уничтожаем объект после завершения затухания
            if (timer >= fadeDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}
