using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;


namespace ExPresSXR.Experimentation.DataGathering
{
    /// <summary>
    /// The Data Gatherer is a component to extract and save data in a Unity Scene.
    /// All values that are specified in the Data Gatherer will be stored in a CSV-formatted file which will be saved at the path and/or be send via http POST to a server.
    /// 
    /// A more detailed description of the capabilities of the `Data Gatherer` can be found in the [DataGathering-Tutorial](Data-Gathering) which is also available in the editor under "ExPresS XR > Data Gathering".
    /// 
    /// Some important things to note:  
    /// 
    /// - The DataGatherer can automatically export values but any script can export new values at any time calling`ExportNewCsvLine()`.
    /// - When played in the editor the Values will be stored at `Application.dataPath`. The Build will store it at the apps data-path (e.g. `%APPDATA%` on Windows)
    /// - While `includeTimestamp` is optional it is recommended to include it as the export times might differ by a few milliseconds.
    /// - The shortest somewhat stable value for `periodicExportTime` was about `0.01s`. 
    /// - Using `exportDuringUpdate` the exports were around 0.02 on a Valve Index (aprox. `Time.DeltaTime` with 60FPs).
    /// </summary>
    public class DataGatherer : MonoBehaviour
    {
        /// <summary>
        /// Default file ending used if a path does not provide a file extension considered valid.
        /// </summary>
        public static readonly string DEFAULT_EXPORT_FILE_ENDING = ".csv";

        /// <summary>
        /// Default path to the export file created to safe the data.
        /// </summary>
        public const string DEFAULT_EXPORT_FILE_NAME = "Data/DataGathererValues.csv";

        /// <summary>
        /// Name of the automatically generated timestamp (number) column.
        /// </summary>
        public const string HUMAN_READABLE_TIME_COLUMN_NAME = "time";

        /// <summary>
        /// Name of the automatically generated unix timestamp (date + time in UTC) column.
        /// </summary>
        public const string UNIX_TIME_COLUMN_NAME = "unix_time";

        /// <summary>
        /// Name of the automatically generated unitx time (seconds since app start) column.
        /// </summary>
        public const string UNITY_TIME_COLUMN_NAME = "unity_time";

        /// <summary>
        /// Name of the automatically generated delta time (duration of current frame) column.
        /// </summary>
        public const string DELTA_TIME_COLUMN_NAME = "delta_time";

        /// <summary>
        /// File endings considered valid log file paths. If the files do not match one of these endings, s
        /// </summary>
        public static readonly string[] EXPORT_FILE_ENDINGS = new string[] { DEFAULT_EXPORT_FILE_ENDING, ".log", ".txt" };

        /// <summary>
        /// Format for a pretty timestamp with a space separating date and time.
        /// </summary>
        public static readonly string timestampPretty = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Format for a safe timestamp with a underscore separating date and time.
        /// </summary>
        public static readonly string timestampSafe = DateTimeOffset.Now.ToString("yyyy-MM-dd_HH-mm-ss");


        [SerializeField]
        [Tooltip("How the export should be performed. Either saving to a file or posting as http request.")]
        private ExportType _dataExportType;
        /// <summary>
        /// How the export should be performed. Either saving to a file or posting as http request.
        /// </summary>
        public ExportType DataExportType
        {
            get => _dataExportType;
            set => _dataExportType = value;
        }

        [SerializeField]
        [Tooltip("Separator type used for the columns.")]
        private SeparatorType _separator;
        /// <summary>
        /// Separator type used for the columns.
        /// </summary>
        public SeparatorType Separator
        {
            get => _separator;
            set
            {
                _separator = value;

                if (_separator == SeparatorType.Comma)
                {
                    ColumnSeparator = CsvUtility.COMMA_COLUMN_SEPARATOR;
                }
                else if (_separator == SeparatorType.Semicolon)
                {
                    ColumnSeparator = CsvUtility.SEMICOLON_COLUMN_SEPARATOR;
                }
            }
        }

        [SerializeField]
        [Tooltip("Actual separator character used for the columns. Defined by the Separator if not set to 'Custom'.")]
        private char _columnSeparator = CsvUtility.DEFAULT_COLUMN_SEPARATOR;
        /// <summary>
        /// Actual separator character used for the columns. Defined by the Separator if not set to 'Custom'.
        /// </summary>
        public char ColumnSeparator
        {
            get => _columnSeparator;
            set
            {
                _columnSeparator = value;

                foreach (DataGatheringBinding binding in DataBindings)
                {
                    binding.HeaderSeparator = _columnSeparator;
                }
            }
        }

        [SerializeField]
        [Tooltip("If all columns should be escaped to prevent format issues.")]
        private bool _escapeColumns = true;
        /// <summary>
        /// If all columns should be escaped to prevent format issues.
        /// </summary>
        public bool EscapeColumns
        {
            get => _escapeColumns;
            set => _escapeColumns = value;
        }

        [SerializeField]
        [Tooltip("Path to the local export directory relative to the apps data directory (Application.persistentDataPath).")]
        private string _localExportPath = DEFAULT_EXPORT_FILE_NAME;
        /// <summary>
        /// Path to the local export directory relative to the apps data directory (Application.persistentDataPath).
        /// </summary>
        public string LocalExportPath
        {
            get => _localExportPath;
            set
            {
                _localExportPath = value;

                if (Application.isPlaying)
                {
                    // Redo setup as the file stream needs to be reopened
                    SetupExport();
                }
            }
        }

        [SerializeField]
        [Tooltip("Url to post data to when exports should be performed via http.")]
        private string _httpExportPath;
        /// <summary>
        /// Url to post data to when exports should be performed via http.
        /// </summary>
        public string HttpExportPath
        {
            get => _httpExportPath;
            set => _httpExportPath = value;
        }


        [SerializeField]
        [Tooltip("When enabled a timestamped export file is created each time the app is started.")]
        private bool _newExportFilePerPlaythrough = true;
        /// <summary>
        /// When enabled a timestamped export file is created each time the app is started.
        /// </summary>
        public bool NewExportFilePerPlaythrough
        {
            get => _newExportFilePerPlaythrough;
            set => _newExportFilePerPlaythrough = value;
        }


        [SerializeField]
        [Tooltip("Will automatically export data each frame in the Update() function.")]
        private bool _exportDuringUpdateEnabled;
        /// <summary>
        /// Will automatically export data each frame in the Update() function.
        /// </summary>
        public bool ExportDuringUpdateEnabled
        {
            get => _exportDuringUpdateEnabled;
            set => _exportDuringUpdateEnabled = value;
        }

        [SerializeField]
        [Tooltip("Input actions that trigger an export when performed.")]
        private InputActionReference[] _inputActionTrigger;
        /// <summary>
        /// Input actions that trigger an export when performed.
        /// </summary>
        public InputActionReference[] InputActionTrigger
        {
            get => _inputActionTrigger;
            set => _inputActionTrigger = value;
        }

        [SerializeField]
        [Tooltip("When enabled, updates are performed periodically after `_periodicExportTime` seconds.")]
        private bool _periodicExportEnabled = false;
        /// <summary>
        /// When enabled, updates are performed periodically after `_periodicExportTime` seconds.
        /// </summary>
        public bool PeriodicExportEnabled
        {
            get => _periodicExportEnabled;
            set
            {
                _periodicExportEnabled = value;

                if (_periodicExportEnabled)
                {
                    StopCoroutine(_periodicExportCoroutine);
                    _periodicExportCoroutine = null;
                }
                else
                {
                    TryStartPeriodicCoroutine();
                }
            }
        }

        [SerializeField]
        [Tooltip("Frequency in seconds after which an automatic export is triggered, when enabled.")]
        private float _periodicExportTime = 1.0f;
        /// <summary>
        /// Frequency in seconds after which an automatic export is triggered, when enabled.
        /// </summary>
        public float PeriodicExportTime
        {
            get => _periodicExportTime;
            set => _periodicExportTime = value;
        }


        [SerializeField]
        [Tooltip("Includes a timestamp in a human-readable format ('yyyy-MM-dd HH:mm:ss'). "
                    + "Its value is relative to the computers local timezone.")]
        private bool _includeHumanReadableTimestamp = true;
        /// <summary>
        /// Includes a timestamp in a human-readable format ('yyyy-MM-dd HH:mm:ss'). Its value is relative to the computers local timezone.
        /// </summary>
        public bool IncludeHumanReadableTimestamp
        {
            get => _includeHumanReadableTimestamp;
            set => _includeHumanReadableTimestamp = value;
        }

        [SerializeField]
        [Tooltip("Includes a unix (numeric) timestamp.")]
        private bool _includeUnixTimestamp = true;
        /// <summary>
        /// Includes a unix (numeric) timestamp.
        /// </summary>
        public bool IncludeUnixTimestamp
        {
            get => _includeUnixTimestamp;
            set => _includeUnixTimestamp = value;
        }

        [SerializeField]
        [Tooltip("Includes the unity time (time since the app was started).")]
        private bool _includeUnityTime = true;
        /// <summary>
        /// Includes the unity time (time since the app was started).
        /// </summary>
        public bool IncludeUnityTime
        {
            get => _includeUnityTime;
            set => _includeUnityTime = value;
        }

        [SerializeField]
        [Tooltip("Includes the unity delta time.")]
        private bool _includeDeltaTime = true;
        /// <summary>
        /// Includes the unity delta time.
        /// </summary>
        public bool IncludeDeltaTime
        {
            get => _includeDeltaTime;
            set => _includeDeltaTime = value;
        }

        [SerializeField]
        [Tooltip("Values and function return values to be exported.")]
        private DataGatheringBinding[] _dataBindings = new DataGatheringBinding[0];
        /// <summary>
        /// Values and function return values to be exported.
        /// </summary>
        public DataGatheringBinding[] DataBindings
        {
            get => _dataBindings;
            set => _dataBindings = value;
        }


        [SerializeField]
        [Tooltip("InputAction values to be exported.")]
        private InputActionReference[] _inputActionDataBindings = new InputActionReference[0];
        /// <summary>
        /// InputAction values values to be exported.
        /// </summary>
        public InputActionReference[] InputActionDataBindings
        {
            get => _inputActionDataBindings;
            set => _inputActionDataBindings = value;
        }

        private Coroutine _periodicExportCoroutine;

        private StreamWriter _outputWriter;

        private void OnEnable()
        {
            TryStartPeriodicCoroutine();
            ConnectInputActions();
            ValidateBindings(false);
            SetupExport();
        }

        private void OnDisable()
        {
            DisconnectInputActions();
            CloseFileWriter();
        }

        private void OnDestroy() => CloseFileWriter();

        private void FixedUpdate()
        {
            if (ExportDuringUpdateEnabled)
            {
                ExportNewCSVLine();
            }
        }

        #region Export
        /// <summary>
        /// Exports a new line with the current values.
        /// </summary>
        public void ExportNewCSVLine()
        {
            if (!isActiveAndEnabled)
            {
                Debug.LogError("Trying to Export a new CSV line of a disabled DataGatherer. This is not allowed.");
                return;
            }

            string data = GetExportCSVLine();
            if (DataExportType == ExportType.Http || DataExportType == ExportType.Both)
            {
                StartCoroutine(PostHttpData(HttpExportPath, data));
            }

            if (DataExportType == ExportType.Local || DataExportType == ExportType.Both)
            {
                _outputWriter.WriteLine(data);
            }
        }

        /// <summary>
        /// Calculates the CSV header for the current configuration and bindings. 
        /// </summary>
        /// <returns>CSV header string.</returns>
        public string GetExportCSVHeader()
        {
            string[] prependedHeaders = {
                _includeHumanReadableTimestamp ? HUMAN_READABLE_TIME_COLUMN_NAME : "",
                _includeUnixTimestamp ? UNIX_TIME_COLUMN_NAME : "",
                _includeUnityTime ? UNITY_TIME_COLUMN_NAME : "",
                _includeDeltaTime ? DELTA_TIME_COLUMN_NAME : ""
            };
            // Add prepended headers
            List<string> bindingHeaders = new(prependedHeaders.Where(s => !string.IsNullOrEmpty(s)));
            List<bool> escapeIndividual = new(Enumerable.Repeat(_escapeColumns, bindingHeaders.Count));
            // Add data bindings
            bindingHeaders.AddRange(_dataBindings.Select(v => v != null ? v.ExportColumnName : ""));
            escapeIndividual.AddRange(_dataBindings.Select(v => !(v?.IsBoundToMultiColumnValue() ?? false) && _escapeColumns));
            // Add InputAction bindings
            bindingHeaders.AddRange(_inputActionDataBindings.Select(v => v != null ? v.name : ""));
            escapeIndividual.AddRange(Enumerable.Repeat(_escapeColumns, bindingHeaders.Count - escapeIndividual.Count));

            // Convert to string
            if (EscapeColumns)
            {
                return CsvUtility.JoinAsCsv(bindingHeaders, escapeIndividual, ColumnSeparator);
            }
            return CsvUtility.JoinAsCsv(bindingHeaders.ToArray(), ColumnSeparator, false);
        }


        /// <summary>
        /// Reads the current values, exporting them as CSV string.
        /// </summary>
        /// <returns>CSV value (escaped) string</returns>
        public string GetExportCSVLine()
        {
            string[] prependedValues = {
                _includeHumanReadableTimestamp ? timestampPretty : "",
                _includeUnixTimestamp ? DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() : "",
                _includeUnityTime ? Time.time.ToString() : "",
                _includeDeltaTime ? Time.deltaTime.ToString() : ""
            };
            // Add prepended values
            List<string> bindingValues = new(prependedValues.Where(s => !string.IsNullOrEmpty(s)));
            List<bool> escapeIndividual = new(Enumerable.Repeat(_escapeColumns, bindingValues.Count));
            // Add DataGatheringBindings
            bindingValues.AddRange(_dataBindings.Select(v => v?.GetBindingValue() ?? ""));
            escapeIndividual.AddRange(_dataBindings.Select(v => !(v?.IsBoundToMultiColumnValue() ?? false) && _escapeColumns));
            // Add InputActionBindings
            bindingValues.AddRange(_inputActionDataBindings.Select(v => CsvUtility.GetInputActionAsSafeString(v)));
            escapeIndividual.AddRange(Enumerable.Repeat(false, bindingValues.Count - escapeIndividual.Count));

            // Convert to string
            if (EscapeColumns)
            {
                return CsvUtility.JoinAsCsv(bindingValues, escapeIndividual, ColumnSeparator);
            }
            return CsvUtility.JoinAsCsv(bindingValues.ToArray(), ColumnSeparator, false);
        }

        private IEnumerator PostHttpData(string url, string data)
        {
            string actualUrl = _newExportFilePerPlaythrough ? $"{url}_{timestampSafe}" : url;
            UnityWebRequest request = new(actualUrl, UnityWebRequest.kHttpVerbPOST);

            if (data != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application-json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ProtocolError
                || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"Failed to send data to server: '{request.error}'.");
            }
        }
        #endregion

        #region Setup & Teardown
        private void SetupExport()
        {
            // Clean up old Output writer if exits
            if (_outputWriter != null)
            {
                _outputWriter.Flush();
                _outputWriter.Close();
            }

            if (DataExportType == ExportType.Http || DataExportType == ExportType.Both)
            {
                StartCoroutine(PostHttpData(HttpExportPath, GetExportCSVHeader()));
            }

            if (DataExportType == ExportType.Local || DataExportType == ExportType.Both)
            {
                if (!HasExportableFileEnding(LocalExportPath))
                {
                    string availableEndings = CsvUtility.ArrayToString(EXPORT_FILE_ENDINGS);
                    _localExportPath += DEFAULT_EXPORT_FILE_ENDING;
                    Debug.LogWarning($"File did not end on one of ${availableEndings}."
                            + $"Appending '${DEFAULT_EXPORT_FILE_ENDING}' and creating a new file if necessary. "
                            + $"New path is: '{LocalExportPath}'.");
                }

                string path = GetLocalSavePath();

                try
                {
                    // Throws an error if the path format is invalid
                    string fullPath = Path.GetFullPath(path);

                    // Create folder if not exists
                    CreateDirectoryIfNotExist(fullPath);

                    _outputWriter = new StreamWriter(fullPath);

                    // If empty append csv header
                    if (new FileInfo(fullPath).Length == 0)
                    {
                        _outputWriter.WriteLine(GetExportCSVHeader());
                        _outputWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        /// <summary>
        /// Utility function called by the editor internally to validate the bindings. 
        /// </summary>
        /// <param name="warnInvalid">Prints a wraning message per invalid binding.</param>
        public void ValidateBindings(bool warnInvalid = true)
        {
            foreach (DataGatheringBinding binding in _dataBindings)
            {
                if (binding == null || !binding.ValidateBinding())
                {
                    if (warnInvalid)
                    {
                        // Check separately to update data upon validation!!
                        Debug.LogWarning("The following binding is invalid and will always be empty: "
                            + $"{binding.GetBindingDescription()}", this);
                    }
                }
            }
        }


        /// <summary>
        /// Adds a DataGatheringBinding to the end of the exported Data Bindings.
        /// Use this function carefully as this rather expensive and it will not add a new column the header, if the file is already open.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public void AddNewBinding(DataGatheringBinding binding)
        {
            _dataBindings = _dataBindings.Concat(new DataGatheringBinding[] { binding }).ToArray();
        }


        private void CloseFileWriter()
        {
            if (_outputWriter != null)
            {
                // Write everything that might not be written & close writer
                _outputWriter.Close();
            }
        }

        private void ConnectInputActions()
        {
            foreach (InputActionReference actionRef in InputActionTrigger)
            {
                if (actionRef != null)
                {
                    actionRef.action.performed += OnInputActionExportRequested;
                }
            }
        }

        private void DisconnectInputActions()
        {
            foreach (InputActionReference actionRef in InputActionTrigger)
            {
                if (actionRef != null)
                {
                    actionRef.action.performed -= OnInputActionExportRequested;
                }
            }
        }
        #endregion

        #region Coroutines & Callback
        private void TryStartPeriodicCoroutine()
        {
            if (PeriodicExportEnabled && Application.isPlaying)
            {
                if (_periodicExportTime > 0)
                {
                    if (_periodicExportCoroutine != null)
                    {
                        StopCoroutine(_periodicExportCoroutine);
                        _periodicExportCoroutine = null;
                    }
                    _periodicExportCoroutine = StartCoroutine(TimeTriggerCoroutine());
                }
                else
                {
                    Debug.LogError("PeriodicExportTime must be greater than zero.");
                }
            }
        }

        private IEnumerator TimeTriggerCoroutine()
        {
            while (_periodicExportEnabled)
            {
                yield return new WaitForSeconds(_periodicExportTime);
                ExportNewCSVLine();
            }
        }

        private void OnInputActionExportRequested(InputAction.CallbackContext _) => ExportNewCSVLine();
        #endregion

        #region Utility
        /// <summary>
        /// Returns the local safe path either in appdata or in the editor.
        /// </summary>
        /// <returns>A safe path to write files to.</returns>
        public string GetLocalSavePath()
        {
            string path = _newExportFilePerPlaythrough ? InsertBeforeExportPostfixes(LocalExportPath, $"_{timestampSafe}") : LocalExportPath;

#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, path);
#else
            return Path.Combine(Application.persistentDataPath, path);
#endif
        }

        private void CreateDirectoryIfNotExist(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);

            if (dirPath == "")
            {
                throw new IOException($"Could not verify Directory existence for filePath {filePath}. Directory path was {dirPath}.");
            }
            else if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private static bool HasExportableFileEnding(string path) => EXPORT_FILE_ENDINGS.Any(ending => path.EndsWith(ending));

        private static string InsertBeforeExportPostfixes(string path, string toInsert)
        {
            string pattern = $"(\\.{string.Join("|", EXPORT_FILE_ENDINGS)})$";

            if (Regex.IsMatch(path, pattern))
            {
                // String ends with one of the postfixes, insert text before the postfix
                return Regex.Replace(path, pattern, @$"{toInsert}$1");
            }

            // String does not end with any of the postfixes, simply append the text
            return path + toInsert;
        }


        private void OnValidate()
        {
            Separator = _separator;
            ColumnSeparator = _columnSeparator;
            ValidateBindings(false);
        }
        #endregion

        #region Enums
        /// <summary>
        /// Type of separating csv columns.
        /// </summary>
        public enum SeparatorType
        {
            /// <summary> Separate columns using a semicolon `;`. </summary>
            Semicolon,
            /// <summary> Separate columns using a comma `,`. </summary>
            Comma,
            /// <summary> Separate columns using the provided char. </summary>
            Custom
        }

        /// <summary>
        /// How data is exported.
        /// </summary>
        public enum ExportType
        {
            /// <summary> Write data to disk. </summary>
            Local,
            /// <summary> Send data via http. </summary>
            Http,
            /// <summary> Write both the file to disk and sent via http. </summary>
            Both
        }
        #endregion
    }
}