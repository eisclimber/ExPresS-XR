using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ExPresSXR.Misc
{
    /// <summary>
    /// Allows toggling the state of a ui button for supporting on the fly toggleMode.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonToggler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If the button should be considered pressed (toggledDown) or not (toggledUp).")]
        private bool _pressed = false;
        /// <summary>
        /// If the button should be considered pressed (toggledDown) or not (toggledUp).
        /// </summary>
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

        [SerializeField]
        [Tooltip("If enabled, will attempt to connect to the 'onClick' event of the button. Do not call ToggleButton in this case!")]
        private bool _connectToClick = false;
        /// <summary>
        /// If enabled, will attempt to connect to the 'onClick' event of the button. Do not call ToggleButton in this case!
        /// </summary>
        public bool ConnectToClick
        {
            get => _connectToClick;
            set => _connectToClick = value;
        }

        /// <summary>
        /// Emitted when the toggle state changes, providing the toggle pressed state.
        /// </summary>
        [Space]
        public ToggledChangedEvent OnToggleChanged;

        /// <summary>
        /// Emitted when the toggle state changes, providing the *Negated* toggle pressed state.
        /// </summary>
        [Space]
        public ToggledChangedEvent OnToggleChangedNegated;

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
            OnToggleChangedNegated.Invoke(!_pressed);
        }

        private void OnValidate()
        {
            Pressed = _pressed;
        }
    }

    /// <summary>
    /// Wrapper class for a bool event emitted when the toggle state changes.
    /// </summary>
    [Serializable]
    public class ToggledChangedEvent : UnityEvent<bool> { }
}