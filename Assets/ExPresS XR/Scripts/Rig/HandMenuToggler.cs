using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExPresSXR.Rig
{
    public class HandMenuToggler : MonoBehaviour
    {
        [SerializeField]
        private Canvas _menuCanvas;

        [SerializeField]
        private float _autoToggleOffDelay = 5.0f;

        [SerializeField]
        private Transform _leftHandAttach;

        [SerializeField]
        private Transform _rightHandAttach;

        [SerializeField]
        private InputActionReference[] _menuLeftToggleActions;

        [SerializeField]
        private InputActionReference[] _menuRightToggleActions;


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

        [ContextMenu("Toggle Menu")]
        public void ToggleMenu() => ToggleMenu(false);

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