using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

        /// <summary>
        /// If enabled, will attempt to connect to the 'onClick' event of the button. Do not call ToggleButton in this case!
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled, will attempt to connect to the 'onClick' event of the button. Do not call ToggleButton in this case!")]
        private bool _connectToClick = false;
        public bool ConnectToClick
        {
            get => _connectToClick;
            set => _connectToClick = value;
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

            if (_connectToClick)
            {
                btn.onClick.AddListener(ToggleButton);
            }
        }

        private void OnDisable()
        {
            if (_connectToClick)
            {
                btn.onClick.RemoveListener(ToggleButton);
            }
        }

        /// <summary>
        /// Toggles the buttons pressed state. Fails if `_connectToClick` is disabled, to prevent multiple toggles.
        /// </summary>
        public void ToggleButton()
        {
            if (_connectToClick)
            {
                Debug.LogWarning("Calling Toggle Button if `_connectToClick` is true, is not allowed!");
                return;
            }

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