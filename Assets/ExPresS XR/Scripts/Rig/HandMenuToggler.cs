using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExPresSXR.Rig
{
    public class HandMenuToggler : MonoBehaviour
    {
        /// <summary>
        /// Menu canvas to be shown, i.e. the HandMenu.
        /// </summary>
        [SerializeField]
        [Tooltip("Menu canvas to be shown, i.e. the HandMenu.")]
        private Canvas _menuCanvas;

        /// <summary>
        /// How long the menu stays open. Values less or equal to zero will keep it open until toggled off.
        /// </summary>
        [SerializeField]
        [Tooltip("How long the menu stays open. Values less or equal to zero will keep it open until toggled off.")]
        private float _autoToggleOffDelay = 5.0f;

        /// <summary>
        /// Attach transform for the left hand side.
        /// </summary>
        [SerializeField]
        [Tooltip("Attach transform for the left hand side.")]
        private Transform _leftHandAttach;

        /// <summary>
        /// Attach transform for the right hand side.
        /// </summary>
        [SerializeField]
        [Tooltip("Attach transform for the right hand side.")]
        private Transform _rightHandAttach;

        /// <summary>
        /// InputActions that toggle the menu for the left side.
        /// </summary>
        [SerializeField]
        [Tooltip("InputActions that toggle the menu for the left side.")]
        private InputActionReference[] _menuLeftToggleActions;

        /// <summary>
        /// InputActions that toggle the menu for the right side.
        /// </summary>
        [SerializeField]
        [Tooltip("InputActions that toggle the menu for the right side.")]
        private InputActionReference[] _menuRightToggleActions;

        /// <summary>
        /// Whether or not the menu is attached on the left side or the right. Changing it will only affect the attach, but keep the hand menus active status.
        /// </summary>
        private bool _leftHandSide;
        public bool LeftHandSide
        {
            get => _leftHandSide;
            set
            {
                _leftHandSide = value;
                _menuCanvas.transform.SetParent(_leftHandSide ? _leftHandAttach : _rightHandAttach, false);
            }
        }

        /// <summary>
        /// Whether or not the menu is shown.
        /// </summary>
        private bool _showMenu;
        public bool ShowMenu
        {
            get => _showMenu;
            set
            {
                _showMenu = value;
                _menuCanvas.gameObject.SetActive(_showMenu);

                if (_autoToggleOffDelay > 0.0f)
                {
                    if (_toggleOffCoroutine != null)
                    {
                        StopCoroutine(_toggleOffCoroutine);
                    }
                    _toggleOffCoroutine = StartCoroutine(ToggleOffDelayed());
                }
            }
        }

        private Coroutine _toggleOffCoroutine;

        private void OnEnable()
        {
            foreach (InputActionReference actionRef in _menuLeftToggleActions)
            {
                actionRef.action.performed += ToggleLeftMenuCallback;
            }
            foreach (InputActionReference actionRef in _menuRightToggleActions)
            {
                actionRef.action.performed += ToggleRightMenuCallback;
            }
        }

        private void OnDisable()
        {
            foreach (InputActionReference actionRef in _menuLeftToggleActions)
            {
                actionRef.action.performed -= ToggleLeftMenuCallback;
            }
            foreach (InputActionReference actionRef in _menuRightToggleActions)
            {
                actionRef.action.performed -= ToggleRightMenuCallback;
            }
        }

        /// <summary>
        /// Toggles the hand menu on and off on the right hand.
        /// </summary>
        [ContextMenu("Toggle Menu")]
        [Tooltip("Toggles the hand menu on and off.")]
        public void ToggleMenu() => ToggleMenu(false);

        /// <summary>
        /// Toggles the hand menu on and off on either the left or right hand.
        /// </summary>
        /// <param name="leftHandSide">Use the left hand or the right.</param>
        public void ToggleMenu(bool leftHandSide)
        {
            if (!_showMenu)
            {
                // Only need to update parent when showing
                LeftHandSide = leftHandSide;
            }
            ShowMenu = !_showMenu;
        }

        private void ToggleLeftMenuCallback(InputAction.CallbackContext _) => ToggleMenu(true);

        private void ToggleRightMenuCallback(InputAction.CallbackContext _) => ToggleMenu(false);

        private IEnumerator ToggleOffDelayed()
        {
            yield return new WaitForSeconds(_autoToggleOffDelay);
            ShowMenu = false;
        }
    }
}