using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ExPresSXR.Misc;


namespace ExPresSXR.UI
{
    public class WorldSpaceKeyboard : MonoBehaviour
    {
        [SerializeField]
        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;

                if (_inputField != null)
                {
                    string prefix = _inputText.StartsWith(_textPrefix) ? "" : _textPrefix;
                    string suffix = _inputText.EndsWith(_textSuffix) ? "" : _textSuffix;
                    _inputField.text = prefix + _inputText + suffix;
                }

                if (_confirmButton != null && _confirmNotEmpty)
                {
                    _confirmButton.interactable = _inputText != "" && !_inputDisabled;
                }
            }
        }

        [SerializeField]
        private string _textPrefix = "";
        public string TextPrefix
        {
            get => _textPrefix;
            set => _textPrefix = value;
        }

        [SerializeField]
        private string _textSuffix = "​";
        public string TextSuffix
        {
            get => _textSuffix;
            set => _textSuffix = value;
        }

        [SerializeField]
        private CapsMode _capsMode = CapsMode.Toggle;
        public CapsMode CapsMode
        {
            get => _capsMode;
            set
            {
                _capsMode = value;

                // Always start with tabs of if not always upper
                CapsActive = _capsMode == CapsMode.AlwaysUpper;

                if (_capsButton != null)
                {
                    // Update the model if forced always upper
                    if (_capsButton.gameObject.TryGetComponent(out ButtonToggler toggler))
                    {
                        toggler.Pressed = CapsMode == CapsMode.AlwaysUpper;
                    }

                    // Can't interact if one mode is forced
                    _capsButton.interactable = CapsMode != CapsMode.AlwaysUpper && CapsMode != CapsMode.AlwaysLower;
                }
            }
        }

        [SerializeField]
        private bool _capsActive = false;
        public bool CapsActive
        {
            get => _capsActive;
            set
            {
                _capsActive = value;
            }
        }

        [SerializeField]
        private bool _inputDisabled;
        public bool InputDisabled
        {
            get => _inputDisabled;
            set
            {
                _inputDisabled = value;

                foreach (Button btn in GetComponentsInChildren<Button>())
                {
                    if (_confirmButton != null && btn == _confirmButton)
                    {
                        _confirmButton.interactable = _inputText != "" && !_inputDisabled;
                    }
                    else
                    {
                        btn.interactable = !_inputDisabled;
                    }
                }

                if (_inputField != null)
                {
                    _inputField.interactable = !_inputDisabled;
                }
            }
        }

        [SerializeField]
        private bool _disableOnConfirm;

        [SerializeField]
        private bool _confirmNotEmpty;

        [SerializeField]
        private TMP_InputField _inputField;

        [SerializeField]
        private Button _capsButton;

        [SerializeField]
        private Button _confirmButton;

        [Space]

        public UnityEvent<string> OnTextEntered;
        public UnityEvent<string> OnTextChanged;


        private void Awake()
        {
            if (_inputField != null)
            {
                _inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            }
        }

        /// <summary>
        /// Confirms the text input.
        /// </summary>
        [ContextMenu("Confirm Text")]
        public void ConfirmText()
        {
            if (_disableOnConfirm)
            {
                InputDisabled = true;
            }

            OnTextEntered.Invoke(InputText);
        }

        /// <summary>
        /// Appends the string to the displayed text.
        /// </summary>
        /// <param name="stringToAppend">String literal to append.</param>
        public void AppendToText(string stringToAppend)
        {
            // This will update the text displayed via the setter function
            string rawValue = CapsActive ? stringToAppend.ToUpper() : stringToAppend.ToLower();
            if (_textSuffix != "" && _inputText.EndsWith(_textSuffix)) // Remove suffix if present
            {
                _inputText = _inputText[..^_textSuffix.Length];
            }
            InputText += rawValue + _textSuffix;

            if (CapsActive && _capsMode == CapsMode.OneCharUpper)
            {
                CapsActive = !CapsActive;

                if (_capsButton != null && _capsButton.gameObject.TryGetComponent(out ButtonToggler toggler))
                {
                    toggler.Pressed = CapsActive;
                }
            }

            OnTextChanged.Invoke(_inputText);
        }

        /// <summary>
        /// Adds a linebreak "\n" to the text.
        /// </summary>
        [ContextMenu("Add Line Break to Text")]
        public void AppendLineBreak()
        {
            if (_disableOnConfirm)
            {
                InputDisabled = true;
            }

            OnTextEntered.Invoke(InputText);
        }

        /// <summary>
        /// Removes the last character from the text, but keeping possible pre- and suffixes.
        /// </summary>
        [ContextMenu("Remove Last from Text")]
        public void RemoveLastFromText()
        {
            if (_inputText.Length > 0)
            {
                int numToStrip = _inputText.EndsWith(_textSuffix) ? _textSuffix.Length + 1 : 1;
                string newText = _inputText[..^numToStrip];
                newText = newText != _textPrefix ? newText + _textSuffix : "";
                InputText = newText;

                OnTextChanged.Invoke(_inputText);
            }
        }

        /// <summary>
        /// Clears text displayed in the keyboard.
        /// </summary>
        [ContextMenu("Clear Text")]
        public void ClearText()
        {
            InputText = "";
            OnTextChanged.Invoke(_inputText);
        }


        /// <summary>
        /// Explicit setter function for setting caps active so we can connect use a named reference. 
        /// </summary>
        /// <param name="newCaps"></param>
        public void ChangeCapsActive(bool newCaps)
        {
            CapsActive = newCaps;
        }

        /// <summary>
        /// Toggles the caps active state.
        /// </summary>
        public void ToggleCapsActive()
        {
            CapsActive = !CapsActive;
        }


        /// <summary>
        /// Allow text input via mouse and keyboard while playing.
        /// </summary>
        /// <param name="newValue">New text value.</param>
        private void OnInputFieldValueChanged(string newValue)
        {
            if (_inputText != newValue)
            {
                _inputText = newValue;
            }
        }

        /// <summary>
        /// Allows in-editor changes.
        /// </summary>
        private void OnValidate()
        {
            InputText = _inputText;
            CapsMode = _capsMode;
            InputDisabled = _inputDisabled;
        }
    }

    /// <summary>
    /// Defines possible behaviors for toggle behaviors.
    /// </summary>
    public enum CapsMode
    {
        Toggle,
        OneCharUpper,
        AlwaysUpper,
        AlwaysLower
    }
}