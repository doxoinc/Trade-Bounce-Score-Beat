using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonController : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
       if (AudioManager.Instance != null && AudioManager.Instance.inGameMusic != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.inGameMusic);
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance или inGameMusic не назначены.");
        }

        // Загрузка игровой сцены
        SceneManager.LoadScene("GameScene"); // Замените на имя вашей игровой сцены
    }
}
