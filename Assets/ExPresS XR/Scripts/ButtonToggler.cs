using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("ExPresS XR/Button Toggler")]
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
                    colors.normalColor = (pressed ? pressedColor : normalColor);
                    colors.selectedColor = (pressed ? pressedColor : normalColor);
                    btn.colors = colors;
                }
            }
        }

        [Space]

        public ToggledChangedEvent OnToggleChanged;


        private Button btn;
        private Color normalColor;
        private Color pressedColor;


        private void Awake()
        {
            btn = gameObject.GetComponent<Button>();
            normalColor = btn.colors.normalColor;
            pressedColor = btn.colors.pressedColor;
            btn.onClick.AddListener(ToggleButton);
        }

        private void ToggleButton()
        {
            pressed = !pressed;

            OnToggleChanged.Invoke(pressed);
        }

        private void OnValidate()
        {
            pressed = _pressed;
        }
    }

    // Make the toggle changed event serializable again 
    [System.Serializable]
    public class ToggledChangedEvent : UnityEvent<bool> { }
}