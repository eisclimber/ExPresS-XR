using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.UI.Menu
{
    public class BasicMenuLogic : MonoBehaviour
    {
        [SerializeField]
        protected BasicMenuLogic backMenu;

        public void GoToMenu(BasicMenuLogic menu)
        {
            menu.gameObject.SetActive(true);

            // Disable current Menu
            gameObject.SetActive(false);
        }

        public void GoBack()
        {
            if (backMenu != null)
            {
                GoToMenu(backMenu);
            }
            else
            {
                Debug.LogError("No menu provided to go back to, nothing will happen.");
            }
        }
    }
}
