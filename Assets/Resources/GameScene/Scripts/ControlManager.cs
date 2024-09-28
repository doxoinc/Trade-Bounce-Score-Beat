using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlManager : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public Button jumpButton;

    [HideInInspector]
    public bool moveLeft = false;
    [HideInInspector]
    public bool moveRight = false;
    [HideInInspector]
    public bool jump = false;

    void Start()
    {
        // Настройка событий для кнопок
        AddButtonEvents(leftButton, true, () => moveLeft = true, () => moveLeft = false);
        AddButtonEvents(rightButton, true, () => moveRight = true, () => moveRight = false);
        AddButtonEvents(jumpButton, false, () => jump = true, () => jump = false);
    }

    void AddButtonEvents(Button button, bool isHold, System.Action onPress, System.Action onRelease)
    {
        if (button == null) return;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // Добавляем событие PointerDown
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { 
            onPress?.Invoke(); 
            Debug.Log(button.name + " нажата");
        });
        trigger.triggers.Add(pointerDown);

        // Добавляем событие PointerUp
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { 
            onRelease?.Invoke(); 
            Debug.Log(button.name + " отпущена");
        });
        trigger.triggers.Add(pointerUp);
    }
}
