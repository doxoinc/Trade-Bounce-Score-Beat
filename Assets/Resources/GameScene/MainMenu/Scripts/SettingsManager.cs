using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button musicToggleButton;
    public Button sfxToggleButton;

    [Header("Sprites")]
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private Image musicButtonImage;
    private Image sfxButtonImage;

    private bool isMusicOn = true;
    private bool isSFXOn = true;

    void Start()
    {
        // Получаем компоненты Image кнопок
        if (musicToggleButton != null)
        {
            musicButtonImage = musicToggleButton.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Music Toggle Button не назначен в SettingsManager.");
        }

        if (sfxToggleButton != null)
        {
            sfxButtonImage = sfxToggleButton.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("SFX Toggle Button не назначен в SettingsManager.");
        }

        // Загружаем настройки из PlayerPrefs
        LoadSettings();

        // Обновляем спрайты кнопок в соответствии с текущими настройками
        UpdateMusicButtonSprite();
        UpdateSFXButtonSprite();

        // Добавляем слушатели нажатий на кнопки
        if (musicToggleButton != null)
        {
            musicToggleButton.onClick.AddListener(ToggleMusic);
        }

        if (sfxToggleButton != null)
        {
            sfxToggleButton.onClick.AddListener(ToggleSFX);
        }
    }

    /// <summary>
    /// Переключает состояние фоновой музыки.
    /// </summary>
    void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        AudioManager.Instance.ToggleMusic(isMusicOn);
        UpdateMusicButtonSprite();
        SaveSettings();
    }

    /// <summary>
    /// Переключает состояние звуковых эффектов.
    /// </summary>
    void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        AudioManager.Instance.ToggleSFX(isSFXOn);
        UpdateSFXButtonSprite();
        SaveSettings();
    }

    /// <summary>
    /// Обновляет спрайт кнопки управления музыкой.
    /// </summary>
    void UpdateMusicButtonSprite()
    {
        if (musicButtonImage != null)
        {
            musicButtonImage.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
        }
    }

    /// <summary>
    /// Обновляет спрайт кнопки управления звуковыми эффектами.
    /// </summary>
    void UpdateSFXButtonSprite()
    {
        if (sfxButtonImage != null)
        {
            sfxButtonImage.sprite = isSFXOn ? sfxOnSprite : sfxOffSprite;
        }
    }

    /// <summary>
    /// Сохраняет настройки звука в PlayerPrefs.
    /// </summary>
    void SaveSettings()
    {
        PlayerPrefs.SetInt("MusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.SetInt("SFXOn", isSFXOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Настройки звука сохранены.");
    }

    /// <summary>
    /// Загружает настройки звука из PlayerPrefs.
    /// </summary>
    void LoadSettings()
    {
        isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        isSFXOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        // Применяем настройки к AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusic(isMusicOn);
            AudioManager.Instance.ToggleSFX(isSFXOn);
        }
    }
}
