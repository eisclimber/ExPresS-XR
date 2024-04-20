using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.UI.Menu
{
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// Container for all menus that should be handled.
        /// </summary>
        [Tooltip("Container for all menus that should be handled.")]
        [SerializeField]
        private GameObject _menuContainer;

        /// <summary>
        /// Menu that is shown initially.
        /// </summary>
        [Tooltip("Menu that is shown initially.")]
        [SerializeField]
        private Transform _startMenu;


        private void Start()
        {
            ActivateStartMenu();
        }

        /// <summary>
        /// Activates the 'startMenu' while disabling all other GameObjects that are children of '_menuContainer.
        /// </summary>
        public void ActivateStartMenu()
        {
            if (_menuContainer == null || _startMenu == null)
            {
                Debug.LogError("No '_menuContainer' or '_startMenu' provided, cannot activate the first menu.");
                return;
            }

            // Disable all but the start menu
            foreach (Transform menu in _menuContainer.transform)
            {
                menu.gameObject.SetActive(menu == _startMenu);
            }
        }
    }
}
