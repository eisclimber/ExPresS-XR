using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
[AddComponentMenu("AutoXR/ButtonToggler")]
public class ButtonToggler : MonoBehaviour
{
    [SerializeField]
    private bool _pressed = false;
    public bool pressed
    {
        get => _pressed;
        set
        {
            _pressed = value;

            if (btn != null)
            {
                ColorBlock colors = btn.colors;
                colors.normalColor = (pressed? pressedColor : normalColor);
                colors.selectedColor = (pressed? pressedColor : normalColor);
                btn.colors = colors;
            }
        }
    }
    
    [Space]

    public ToggledChangedEvent onToggleChanged;


    private Button btn;
    private Color normalColor;
    private Color pressedColor;

    
    void Awake()
    {
        btn = gameObject.GetComponent<Button>();
        normalColor = btn.colors.normalColor;
        pressedColor = btn.colors.pressedColor;
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        pressed = !pressed;

        onToggleChanged.Invoke(pressed);
    }

    void OnValidate()
    {
        pressed = _pressed;
    }
}

// Make the toggle changed event serializable again 
[System.Serializable]
public class ToggledChangedEvent : UnityEvent<bool> { }
