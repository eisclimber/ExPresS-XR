using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.UI.Menu
{
    /// <summary>
    /// Implements a basic menu navigation for switching menus (to a fixe "back"-menu or to an arbitrary one).
    /// </summary>
    public class BasicMenuLogic : MonoBehaviour
    {
        /// <summary>
        /// Menu to switch to when `GoBack` is called.
        /// </summary>
        [SerializeField]
        protected BasicMenuLogic _backMenu;
        
        /// <summary>
        /// Disables the gameObject this component is attached to and activates the provided menu.
        /// </summary>
        /// <param name="menu">Menu to switch to.</param>
        public void GoToMenu(BasicMenuLogic menu)
        {
            menu.gameObject.SetActive(true);

            // Disable current Menu
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Switches to the menu set as `backMenu` unless it is `null`.
        /// </summary>
        public void GoBack()
        {
            if (_backMenu != null)
            {
                GoToMenu(_backMenu);
            }
            else
            {
                Debug.LogError("No menu provided to go back to, nothing will happen.");
            }
        }
    }
}
