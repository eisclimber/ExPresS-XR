using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    [RequireComponent(typeof(Button))]
    public class ButtonToggler : MonoBehaviour
    {
        [SerializeField]
        private bool _pressed = false;
        public bool Pressed
        {
            get => _pressed;
            set
            {
                _pressed = value;

                if (btn != null)
                {
                    ColorBlock colors = btn.colors;
                    colors.normalColor = Pressed ? pressedColor : normalColor;
                    colors.selectedColor = Pressed ? pressedColor : normalColor;
                    btn.colors = colors;
                }
            }
        }

        [Space]

        public ToggledChangedEvent OnToggleChanged;


        private Button btn;
        private Color normalColor;
        private Color pressedColor;


        private void OnEnable()
        {
            btn = gameObject.GetComponent<Button>();
            normalColor = btn.colors.normalColor;
            pressedColor = btn.colors.pressedColor;
            btn.onClick.AddListener(ToggleButton);
        }

        private void OnDisable()
        {
            btn.onClick.RemoveListener(ToggleButton);
        }

        private void ToggleButton()
        {
            Pressed = !_pressed;

            OnToggleChanged.Invoke(_pressed);
        }

        private void OnValidate()
        {
            Pressed = _pressed;
        }
    }

    // Make the toggle changed event serializable again 
    [System.Serializable]
    public class ToggledChangedEvent : UnityEvent<bool> { }
}