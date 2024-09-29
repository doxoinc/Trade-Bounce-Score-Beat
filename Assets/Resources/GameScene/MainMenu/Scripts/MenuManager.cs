using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    // Панели
    public GameObject[] panels;

    // Кнопки, соответствующие панелям
    public Button[] panelButtons;

    // Спрайты для активного и неактивного состояния кнопок
    public Sprite[] activeButtonSprites;
    public Sprite[] defaultButtonSprites;

    // Индекс текущей панели
    private int currentPanelIndex = -1;

    // Масштаб для активной кнопки (увеличение на 0.2)
    public Vector3 activeButtonScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 defaultButtonScale = Vector3.one;

    void Start()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.mainMenuMusic != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenuMusic);
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance или mainMenuMusic не назначены.");
        }
        // Устанавливаем спрайты кнопок в состояние по умолчанию при старте
        for (int i = 0; i < panelButtons.Length; i++)
        {
            UpdateButtonState(i, false);
            int index = i; // Для использования внутри лямбда-функции
            panelButtons[i].onClick.AddListener(() => OnPanelButtonClicked(index));
        }
    }

    // Метод для открытия панели по индексу
    public void OpenPanel(int panelIndex)
    {
        if (currentPanelIndex == panelIndex)
        {
            // Панель уже открыта, ничего не делаем
            return;
        }

        // Закрываем текущую панель, если она открыта
        if (currentPanelIndex != -1)
        {
            ClosePanel(currentPanelIndex);
        }

        // Открываем новую панель
        currentPanelIndex = panelIndex;
        GameObject panel = panels[panelIndex];
        panel.SetActive(true);

        Animator animator = panel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("PanelOpen");
        }

        // Обновляем состояние кнопки
        UpdateButtonState(panelIndex, true);
    }

    // Метод для закрытия панели по индексу
    public void ClosePanel(int panelIndex)
    {
        GameObject panel = panels[panelIndex];

        // Обновляем состояние кнопки
        UpdateButtonState(panelIndex, false);

        Animator animator = panel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("PanelClose");
            StartCoroutine(DisablePanelAfterAnimation(panel, animator.GetCurrentAnimatorStateInfo(0).length));
        }
        else
        {
            panel.SetActive(false);
        }

        if (currentPanelIndex == panelIndex)
            currentPanelIndex = -1;
    }

    private IEnumerator DisablePanelAfterAnimation(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(false);
    }

    // Метод для закрытия всех панелей
    public void CloseAllPanels()
    {
        if (currentPanelIndex != -1)
        {
            ClosePanel(currentPanelIndex);
        }
        else
        {
            // Если текущая панель не отслеживается, закрываем все активные панели
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].activeSelf)
                {
                    ClosePanel(i);
                }
            }
        }
        currentPanelIndex = -1;
    }

    // Метод для обновления состояния кнопки
    private void UpdateButtonState(int buttonIndex, bool isActive)
    {
        if (panelButtons.Length > buttonIndex)
        {
            Button button = panelButtons[buttonIndex];
            Image buttonImage = button.GetComponent<Image>();
            RectTransform buttonRectTransform = button.GetComponent<RectTransform>();

            if (buttonImage != null)
            {
                if (isActive)
                {
                    // Устанавливаем активный спрайт и увеличиваем кнопку
                    if (activeButtonSprites.Length > buttonIndex && activeButtonSprites[buttonIndex] != null)
                    {
                        buttonImage.sprite = activeButtonSprites[buttonIndex];
                    }
                    // Увеличиваем размер кнопки
                    buttonRectTransform.localScale = activeButtonScale;
                }
                else
                {
                    // Устанавливаем спрайт по умолчанию и возвращаем кнопку к исходному размеру
                    if (defaultButtonSprites.Length > buttonIndex && defaultButtonSprites[buttonIndex] != null)
                    {
                        buttonImage.sprite = defaultButtonSprites[buttonIndex];
                    }
                    // Возвращаем размер кнопки к нормальному
                    buttonRectTransform.localScale = defaultButtonScale;
                }
            }
        }
    }

    // Методы для кнопок
    public void OnPanelButtonClicked(int panelIndex)
    {
        // Воспроизведение звука при нажатии на кнопку
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClickClip != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickClip);
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance или buttonClickClip не назначены.");
        }

        OpenPanel(panelIndex);
    }

    public void OnCloseButtonClicked()
    {
        if (currentPanelIndex != -1)
        {
            // Воспроизведение звука при закрытии панели
            if (AudioManager.Instance != null && AudioManager.Instance.buttonClickClip != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickClip);
            }
            else
            {
                Debug.LogWarning("AudioManager.Instance или buttonClickClip не назначены.");
            }

            ClosePanel(currentPanelIndex);
        }
    }

    // Метод для кнопки Home
    public void OnHomeButtonClicked()
    {
        // Воспроизведение звука при нажатии на кнопку Home
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClickClip != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickClip);
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance или buttonClickClip не назначены.");
        }

        CloseAllPanels();
    }
}
