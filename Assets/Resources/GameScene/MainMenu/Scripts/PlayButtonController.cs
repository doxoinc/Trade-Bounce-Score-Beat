using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonController : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        // Загрузка игровой сцены
        SceneManager.LoadScene("GameScene");
    }
}
