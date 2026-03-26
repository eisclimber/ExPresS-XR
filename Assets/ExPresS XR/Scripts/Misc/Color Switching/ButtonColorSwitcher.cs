using ExPresSXR.Interaction;
using ExPresSXR.Interaction.ValueRangeInteractable;
using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Represents the color switching logic of a Button-ValueRangeInteractable.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ButtonColorSwitcher : ColorAnimatorSwitcher
    {
        /// <summary>
        /// The Button to which this ColorAnimatorSwitcher is linked.
        /// </summary>
        [SerializeField]
        [Tooltip("The Button to which this ColorAnimatorSwitcher is linked.")]
        private Button _button;

        /// <inheritdoc />
        protected virtual void OnEnable()
        {
            RegisterButtonEvents();

            if (_button == null)
            {
                Debug.LogError("No button reference set for this color switcher.", this);
            }
        }

        /// <inheritdoc />
        protected virtual void OnDisable()
        {
            UnregisterButtonEvents();
        }

        private void RegisterButtonEvents()
        {
            if (_button != null)
            {
                _button.OnPressed.AddListener(HandleButtonPressed);
                _button.OnReleased.AddListener(HandleButtonReleased);

                _button.OnTogglePressed.AddListener(HandleButtonPressed);
                _button.OnToggleReleased.AddListener(HandleButtonReleased);

                _button.OnInputEnabled.AddListener(HandleButtonEnabled);
                _button.OnInputDisabled.AddListener(HandleButtonDisabled);
            }
        }

        private void UnregisterButtonEvents()
        {
            if (_button != null)
            {
                _button.OnPressed.RemoveListener(HandleButtonPressed);
                _button.OnReleased.RemoveListener(HandleButtonReleased);

                _button.OnTogglePressed.AddListener(HandleButtonPressed);
                _button.OnToggleReleased.AddListener(HandleButtonReleased);

                _button.OnInputEnabled.RemoveListener(HandleButtonEnabled);
                _button.OnInputDisabled.RemoveListener(HandleButtonDisabled);
            }
        }


        private void HandleButtonPressed()
        {
            ChangeColorWithBool("IsPressed", true);
        }

        private void HandleButtonReleased()
        {
            ChangeColorWithBool("IsPressed", false);
        }

        private void HandleButtonEnabled()
        {
            ChangeColorWithBool("IsDisabled", false);
        }

        private void HandleButtonDisabled()
        {
            ChangeColorWithBool("IsDisabled", true);
        }
    }
}