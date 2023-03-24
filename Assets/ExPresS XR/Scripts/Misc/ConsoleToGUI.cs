using UnityEngine;
using TMPro;

// Credits to bboysil : https://answers.unity.com/questions/125049/is-there-any-way-to-view-the-console-in-a-build.html
namespace ExPresSXR.Misc
{
    public class ConsoleToGUI : MonoBehaviour
    {
        static string myLog = "";
        private string output;
        private string stack;

        private TextMeshProUGUI _textDisplay;


        private void Start()
        {
            _textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        }


        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;

            string[] stackLines = stackTrace.Split("\n");
            string stackBeginning = stackLines.Length > 0 ? stackLines[0] : "";

            myLog = myLog + "\n" + stackBeginning + " : " + output;

            // Reduce Size
            if (myLog.Length > 2400)
            {
                myLog = myLog[^2000..];
            }
        }

        private void Update()
        {
            if (_textDisplay != null)
            {
                _textDisplay.text = myLog;
            }
        }
    }
}