using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource SFXSource;
    public AudioSource MusicSource;

    [Header("Sound Effects")]
    public AudioClip buttonClickClip;
    public AudioClip jumpClip;
    public AudioClip removeGlassesClip;
    public AudioClip collectCoinClip;
    public AudioClip loseHeartClip;
    public AudioClip gameOverClip;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip inGameMusic;
    public AudioClip missionCompleteMusic; // Добавлено для панели завершения уровня

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Ссылка на AudioMixer (опционально)

    void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Проверка на наличие AudioSources
        if (SFXSource == null)
        {
            SFXSource = gameObject.AddComponent<AudioSource>();
            SFXSource.playOnAwake = false;
            SFXSource.loop = false;
            SFXSource.spatialBlend = 0f; // 2D звук
            if (audioMixer != null)
            {
                SFXSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            }
        }

        if (MusicSource == null)
        {
            MusicSource = gameObject.AddComponent<AudioSource>();
            MusicSource.playOnAwake = false;
            MusicSource.loop = true;
            MusicSource.spatialBlend = 0f; // 2D звук
            if (audioMixer != null)
            {
                MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
            }
        }
    }

    /// <summary>
    /// Воспроизводит звуковой эффект.
    /// </summary>
    /// <param name="clip">Аудиоклип для воспроизведения.</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && SFXSource != null)
        {
            SFXSource.PlayOneShot(clip);
            Debug.Log($"Воспроизведён звук: {clip.name}");
        }
        else
        {
            Debug.LogWarning("PlaySFX: Аудиоклип или SFXSource не назначены.");
        }
    }

    /// <summary>
    /// Воспроизводит фоновую музыку.
    /// </summary>
    /// <param name="clip">Аудиоклип музыки для воспроизведения.</param>
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && MusicSource != null)
        {
            if (MusicSource.clip == clip && MusicSource.isPlaying)
            {
                // Музыка уже играет, ничего не делаем
                return;
            }

            MusicSource.clip = clip;
            MusicSource.Play();
            Debug.Log($"Воспроизведена музыка: {clip.name}");
        }
        else
        {
            Debug.LogWarning("PlayMusic: Аудиоклип или MusicSource не назначены.");
        }
    }

    /// <summary>
    /// Останавливает воспроизведение музыки.
    /// </summary>
    public void StopMusic()
    {
        if (MusicSource != null && MusicSource.isPlaying)
        {
            MusicSource.Stop();
            Debug.Log("Музыка остановлена.");
        }
    }

    /// <summary>
    /// Включает/выключает фоновую музыку.
    /// </summary>
    /// <param name="isOn">True для включения, False для выключения.</param>
    public void ToggleMusic(bool isOn)
    {
        if (MusicSource != null)
        {
            MusicSource.mute = !isOn;
            Debug.Log($"Музыка {(isOn ? "включена" : "выключена")}.");
        }
    }

    /// <summary>
    /// Включает/выключает звуковые эффекты.
    /// </summary>
    /// <param name="isOn">True для включения, False для выключения.</param>
    public void ToggleSFX(bool isOn)
    {
        if (SFXSource != null)
        {
            SFXSource.mute = !isOn;
            Debug.Log($"Звуковые эффекты {(isOn ? "включены" : "выключены")}.");
        }
    }
}
