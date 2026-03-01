using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ExPresSXR.Misc;


namespace ExPresSXR.UI
{
    /// <summary>
    /// A world space UI element representing keyboard with an input field for XR.
    /// It can be configured to match any set of characters or even longer string sequences.
    /// Keyboards of the German and English key layouts, and a numpad can be created directly from the GameObject-Menu.
    /// 
    /// The keyboard can have a Caps-key which is interpreted determined by `capsMode`.  
    /// It can either be:
    /// 
    /// - Toggle: Pressing Caps will change between upper to lower and keep it until pressed again.
    /// - OneCharUpper: Pressing Caps will change the next character to upper, then go back to lower
    /// - AlwaysUpper: The caps does not have an effect. All characters are in *upper* case.
    /// - AlwaysLower: The caps does not have an effect. All characters are in *lower* case.
    /// 
    /// To make a new custom Keyboard create a Canvas Object and add a `WorldSpaceKeyboards`-Component.
    /// Then place all Buttons and connect the Buttons with their `OnPressed`-Event with
    /// the `WorldSpaceKeyboard.appendToText()`-function passing the text of the button as parameter.
    /// For the deletion of the last character, clearing or confirming the text simply connect their respective functions instead.
    /// If the current text should be shown, add a `TMP_InputField` and drag it into the property of the `WorldSpaceKeyboards`.
    /// </summary>
    public class WorldSpaceKeyboard : MonoBehaviour
    {
        [SerializeField]
        private string _inputText = "";
        /// <summary>
        /// The current text input of the keyboard.
        /// </summary>
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
        /// <summary>
        /// A prefix that is always added to the displayed text but is not considered part of the input.
        /// Can be used to e.g. display currency symbols like `EUR 100`.
        /// </summary>
        public string TextPrefix
        {
            get => _textPrefix;
            set => _textPrefix = value;
        }

        [SerializeField]
        private string _textSuffix = "​";
        /// <summary>
        /// A suffix that is always added to the displayed text but is not considered part of the input.
        /// Can be used to e.g. display currency symbols like `100 €`.
        /// </summary>
        public string TextSuffix
        {
            get => _textSuffix;
            set => _textSuffix = value;
        }

        [SerializeField]
        private CapsMode _capsMode = CapsMode.Toggle;
        /// <summary>
        /// Defines how the caps/shift button behaves.
        /// </summary>
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
        /// <summary>
        /// Whether or not the caps/shift button is currently active. The behavior depends on the selected `CapsMode`.
        /// </summary>
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
        /// <summary>
        /// Whether or not the keyboard input is currently disabled. This will make all buttons non-interactable and also disable the input field.
        /// </summary>
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

        /// <summary>
        /// If the keyboard should be disabled after confirming the input text.
        /// </summary>
        [SerializeField]
        private bool _disableOnConfirm;

        /// <summary>
        /// If an empty input is allowed or not. The confirm button will be interactable accordingly.
        /// </summary>
        [SerializeField]
        private bool _confirmNotEmpty;

        /// <summary>
        /// The input field used to display the current text.
        /// </summary>
        [SerializeField]
        private TMP_InputField _inputField;

        /// <summary>
        /// The caps button of the keyboard.
        /// </summary>
        [SerializeField]
        private Button _capsButton;

        /// <summary>
        /// The confirm button of the keyboard.
        /// </summary>
        [SerializeField]
        private Button _confirmButton;

        /// <summary>
        /// Event invoked when the text was confirmed, providing the final text.
        /// </summary>
        [Space]
        public UnityEvent<string> OnTextEntered;

        /// <summary>
        /// Event invoked when the text was changed, providing the new text.
        /// </summary>
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
        /// <summary> The caps button is toggled on and off until pressed again. </summary>
        Toggle,
        /// <summary> The caps button will only affect the next character and then automatically turns off.</summary>
        OneCharUpper,
        /// <summary> Text will be always upper case. </summary>
        AlwaysUpper,
        /// <summary> Text will be always lower case. </summary>
        AlwaysLower
    }
}