using System;
using ExPresSXR.Interaction;
using UnityEngine;

namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Represents the color switching logic of a Legacy BaseButton.
    /// </summary>
    [Obsolete("The button functionality was reimplemented as ValueRangeInteractable.\nUse an `ExPresSXR.Interaction.Button` instead.")]
    [RequireComponent(typeof(Animator))]
    public class LegacyButtonColorSwitcher : ColorAnimatorSwitcher
    {
        /// <summary>
        /// The (legacy) BaseButton to which this ColorAnimatorSwitcher is linked.
        /// </summary>
        [SerializeField]
        [Tooltip("The (legacy) BaseButton to which this ColorAnimatorSwitcher is linked.")]
        private BaseButton _legacyButton;

        /// <inheritdoc />
        protected virtual void OnEnable()
        {
            RegisterButtonEvents();

            if (_legacyButton == null)
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