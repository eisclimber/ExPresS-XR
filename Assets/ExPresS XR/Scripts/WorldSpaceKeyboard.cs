using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using ExPresSXR.Misc;


namespace ExPresSXR.UI
{
    public class WorldSpaceKeyboard : MonoBehaviour
    {
        private string _inputText = "";
        public string inputText
        {
            get => _inputText;
            set
            {
                _inputText = value;

                if (_inputField != null)
                {
                    _inputField.text = _inputText;
                }
            }
        }

        [SerializeField]
        private CapsMode _capsMode = CapsMode.Toggle;
        public CapsMode capsMode
        {
            get => _capsMode;
            set
            {
                _capsMode = value;

                // Always start with tabs of if not always upper
                capsActive = _capsMode == CapsMode.AlwaysUpper;

                if (_capsButton != null)
                {
                    // Update the model if forced always upper
                    if (_capsButton.gameObject.GetComponent<ButtonToggler>())
                    {
                        _capsButton.gameObject.GetComponent<ButtonToggler>().pressed = (capsMode == CapsMode.AlwaysUpper);
                    }

                    // Can't interact if one mode is forced
                    _capsButton.interactable = (capsMode != CapsMode.AlwaysUpper && capsMode != CapsMode.AlwaysLower);
                }
            }
        }

        [SerializeField]
        private bool _capsActive = false;
        public bool capsActive
        {
            get => _capsActive;
            set
            {
                _capsActive = value;
            }
        }

        [SerializeField]
        private TMP_InputField _inputField;

        [SerializeField]
        private Button _capsButton;

        [Space]

        public UnityEvent<string> OnTextEntered;
        public UnityEvent<string> OnTextChanged;


        private void Awake()
        {
            if (_inputField != null)
            {
                _inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
            }

            if (_capsButton != null)
            {
                if (_capsButton != null && _capsButton.gameObject.GetComponent<ButtonToggler>())
                {
                    _capsButton.gameObject.GetComponent<ButtonToggler>().OnToggleChanged.AddListener(ChangeCapsActive);
                }
            }
        }


        public void ConfirmText()
        {
            OnTextEntered.Invoke(inputText);
        }

        public void AppendToText(string stringToAppend)
        {
            inputText += capsActive ? stringToAppend.ToUpper() : stringToAppend.ToLower();

            if (capsActive && _capsMode == CapsMode.OneCharUpper)
            {
                capsActive = !capsActive;

                if (_capsButton != null && _capsButton.gameObject.GetComponent<ButtonToggler>())
                {
                    _capsButton.gameObject.GetComponent<ButtonToggler>().pressed = capsActive;
                }
            }

            OnTextChanged.Invoke(inputText);
        }

        public void RemoveLastFromText()
        {
            if (inputText.Length > 0)
            {
                inputText = inputText.Substring(0, inputText.Length - 1);
                OnTextChanged.Invoke(inputText);
            }
        }

        public void ClearText()
        {
            inputText = "";
            OnTextChanged.Invoke(inputText);
        }


        public void ChangeCapsActive(bool newCaps)
        {
            capsActive = newCaps;
        }


        // Allow text input via keyboard
        private void OnInputFieldValueChanged(string newValue)
        {
            if (inputText != newValue)
            {
                inputText = newValue;
            }
        }

        // Allows in-editor changes
        private void OnValidate()
        {
            inputText = _inputText;
            capsMode = _capsMode;
        }
    }


    public enum CapsMode
    {
        Toggle,
        OneCharUpper,
        AlwaysUpper,
        AlwaysLower
    }
}