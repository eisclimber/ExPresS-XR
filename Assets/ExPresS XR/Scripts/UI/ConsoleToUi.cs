using System;
using UnityEngine;
using TMPro;


/*
 * This component that can be added to a TMP_Text to display the console log inside the application.
 * The first element of the Stack Trace will be logged too.
 *
 * Note that you can connect your device to your your computer and select it as "Connected Player" in the editor.
 * Use this component instead if this is not possible.
 *
 * A discussion and credits to bboysil on this topic: https://answers.unity.com/questions/125049/is-there-any-way-to-view-the-console-in-a-build.html
 */
namespace ExPresSXR.UI
{
    public class ConsoleToGUI : MonoBehaviour
    {
        /// <summary>
        /// Number of max lines after which the oldest lines get removed. No restriction if less or equal to 0.
        /// </summary>
        [SerializeField]
        [Tooltip("Number of max lines after which the oldest lines get removed. No restriction if less or equal to 0.")]
        private int _maxLines = -1;

        /// <summary>
        /// Which types of logs should be displayed.
        /// </summary>
        [SerializeField]
        [Tooltip("Which types of logs should be displayed.")]
        private LogTypeFilter _logTypeFilter = LogTypeFilter.Error | LogTypeFilter.Assert | LogTypeFilter.Warning 
                                                | LogTypeFilter.Log | LogTypeFilter.Exception; // Default: Everything
        
        /// <summary>
        /// Reference to the TMP_Text that should display the console log.
        /// </summary>
        [SerializeField]
        private TMP_Text _textDisplay;

        public string currentLog { get; private set; }


        private void Start()
        {
            if (_textDisplay == null && !TryGetComponent(out _textDisplay))
            {
                Debug.LogError("Could not find a TMP_Text to display the console output.", this);
            }
        }


        private void OnEnable()
        {
            Application.logMessageReceived += AppendToLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= AppendToLog;
        }

        /// <summary>
        /// Adds an entry to the logs displayed. 
        /// Will be automatically be called when something is logged to the console.
        /// </summary>
        /// <param name="logString">String to be logged.</param>
        /// <param name="stackTrace">Stack trace of that log entry.</param>
        /// <param name="type">Type/Severity of the log.</param>
        public void AppendToLog(string logString, string stackTrace, LogType type)
        {
            if (!IsLogTypeMatch(type))
            {
                return;
            }

            string[] stackLines = stackTrace.Split("\n");
            string stackBeginning = stackLines.Length > 0 ? "\t" + stackLines[0] : "";
            string logColor = ColorForLogType(type);

            currentLog += $"<color={logColor}>{ logString }</color>\n<color={ logColor }> - { stackBeginning }</color>\n";

            // Truncate text
            string[] logLines = currentLog.Split("\n");
            
            if (_maxLines > 0 && logLines.Length > _maxLines)
            {
                // Remove lines that are not fitting anymore
                currentLog = string.Join("\n", logLines[^(_maxLines + 1)..]);
            }
        }

        /// <summary>
        /// Helper functions to log a short string via the Components context menu
        /// </summary>
        [ContextMenu("Append Test Entry (Short)")]
        public void AppendTestEntryShort()
                        => AppendToLog("Lorem ipsum dolor sit amet, consetetur",
                                        "Stack Trace Lorem Ipsum", GetRandomLogType());

        /// <summary>
        /// Helper functions to log a long string via the Components context menu
        /// </summary>
        [ContextMenu("Append Test Entry (Long)")]
        public void AppendTestEntryLong() 
                        => AppendToLog("Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy "
                                        + "eirmod tempor invidunt ut labore et dolore magna aliquyam", 
                                        "Stack Trace Lorem Ipsum", GetRandomLogType());

        private void Update()
        {
            if (_textDisplay != null)
            {
                _textDisplay.text = currentLog;
            }
        }


        private bool IsLogTypeMatch(LogType type) => _logTypeFilter.HasFlag((LogTypeFilter)type);

        private string ColorForLogType(LogType type)
        {
            return type switch
            {
                LogType.Error or LogType.Exception or LogType.Assert => "red",
                LogType.Warning => "yellow",
                _ => "white",
            };
        }

        private LogType GetRandomLogType() => (LogType) UnityEngine.Random.Range(0, Enum.GetValues(typeof(LogType)).Length);


        // Reflects UnityEngine.LogType but as flags.
        [Flags]
        public enum LogTypeFilter
        {
            Error, // LogType used for Errors.
            Assert, // LogType used for Asserts (These could also indicate an Unity internal error).
            Warning, // LogType used for Warnings.
            Log, // LogType used for regular log messages.
            Exception// LogType used for Exceptions.
        }
    }
}