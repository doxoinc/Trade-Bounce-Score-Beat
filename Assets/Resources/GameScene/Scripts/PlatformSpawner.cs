using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Префабы платформ")]
    public GameObject greenPlatformPrefab;
    public GameObject redPlatformPrefab;

    [Header("Настройки генерации")]
    public float spawnInterval = 2f; // Интервал между спавнами платформ
    public float spawnYPosition = 10f; // Y позиция спавна (вверху экрана)
    public float minX = -4f; // Минимальная X позиция
    public float maxX = 4f; // Максимальная X позиция

    [Header("Максимальное количество платформ на экране")]
    public int maxPlatforms = 30;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && FindObjectsOfType<Platform>().Length < maxPlatforms)
        {
            SpawnPlatform();
            timer = 0f;
        }
    }

    void SpawnPlatform()
    {
        // Выбор типа платформы случайным образом
        int platformType = Random.Range(0, 2); // 0 - Green, 1 - Red
        GameObject platformToSpawn = (platformType == 0) ? greenPlatformPrefab : redPlatformPrefab;

        // Генерация случайной X позиции
        float randomX = Random.Range(minX, maxX);

        // Создание платформы
        Instantiate(platformToSpawn, new Vector3(randomX, spawnYPosition, 0), Quaternion.identity);
        //Debug.Log("Платформа " + (platformType == 0 ? "Green" : "Red") + " спавнена на позиции: " + new Vector3(randomX, spawnYPosition, 0));
    }
}
