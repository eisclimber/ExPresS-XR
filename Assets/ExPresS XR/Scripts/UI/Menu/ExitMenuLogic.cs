using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.UI.Menu
{
    public class ExitMenuLogic : BasicMenuLogic
    {
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}