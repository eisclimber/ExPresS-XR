using ExPresSXR.Interaction;
using ExPresSXR.Interaction.ValueRangeInteractable;
using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Represents the color switching logic of a BaseButton and a Button-ValueRangeInteractable.
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

        /// <summary>
        /// The (legacy) BaseButton to which this ColorAnimatorSwitcher is linked.
        /// </summary>
        [SerializeField]
        [Tooltip("The (legacy) BaseButton to which this ColorAnimatorSwitcher is linked.")]
        private BaseButton _legacyButton;

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected virtual void OnEnable()
        {
            RegisterButtonEvents();
        }

        /// <summary>
        /// < inheritdoc />
        /// </summary>
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

            if (_legacyButton != null)
            {
                _legacyButton.OnPressed.AddListener(HandleLegacyButtonPressed);
                _legacyButton.OnReleased.AddListener(HandleLegacyButtonReleased);

                _legacyButton.OnTogglePressed.AddListener(HandleLegacyButtonPressed);
                _legacyButton.OnToggleReleased.AddListener(HandleLegacyButtonReleased);

                _legacyButton.OnInputEnabled.AddListener(HandleLegacyButtonEnabled);
                _legacyButton.OnInputDisabled.AddListener(HandleLegacyButtonDisabled);

                _legacyButton.OnButtonPressReset.AddListener(HandleLegacyButtonPressReset);
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

            if (_legacyButton != null)
            {
                _legacyButton.OnPressed.RemoveListener(HandleLegacyButtonPressed);
                _legacyButton.OnReleased.RemoveListener(HandleLegacyButtonReleased);

                _legacyButton.OnTogglePressed.RemoveListener(HandleLegacyButtonPressed);
                _legacyButton.OnToggleReleased.RemoveListener(HandleLegacyButtonReleased);

                _legacyButton.OnInputEnabled.RemoveListener(HandleLegacyButtonEnabled);
                _legacyButton.OnInputDisabled.RemoveListener(HandleLegacyButtonDisabled);

                _legacyButton.OnButtonPressReset.RemoveListener(HandleLegacyButtonPressReset);
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

        // Legacy BaseButton handlers
        private void HandleLegacyButtonPressed()
        {
            ChangeColorWithBool("IsPressed", true);
        }

        private void HandleLegacyButtonReleased()
        {
            ChangeColorWithBool("IsPressed", false);
        }

        private void HandleLegacyButtonEnabled()
        {
            ChangeColorWithBool("IsDisabled", false);
        }

        private void HandleLegacyButtonDisabled()
        {
            ChangeColorWithBool("IsDisabled", true);
        }

        private void HandleLegacyButtonPressReset()
        {
            // We need to update the pressed state, as it won't get set automatically
            ChangeColorWithBool("IsPressed", false);
        }
    }
}