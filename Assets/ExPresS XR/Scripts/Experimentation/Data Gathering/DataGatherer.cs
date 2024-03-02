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
    public class DataGatherer : MonoBehaviour
    {
        public const string DEFAULT_EXPORT_FILE_NAME = "Data/DataGathererValues.csv";
        public const string HUMAN_READABLE_TIME_COLUMN_NAME = "time";
        public const string UNIX_TIME_COLUMN_NAME = "unix_time";
        public const string UNITY_TIME_COLUMN_NAME = "unity_time";
        public const string DELTA_TIME_COLUMN_NAME = "delta_time";

        public static readonly string[] EXPORT_FILE_ENDINGS = { "csv", "log", "txt" };

        public static readonly string timestampPretty = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public static readonly string timestampSafe = DateTimeOffset.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        [SerializeField]
        private ExportType _dataExportType;
        public ExportType dataExportType
        {
            get => _dataExportType;
            set => _dataExportType = value;
        }


        [SerializeField]
        private SeparatorType _separatorType;
        public SeparatorType separatorType
        {
            get => _separatorType;
            set
            {
                _separatorType = value;

                if (_separatorType == SeparatorType.Comma)
                {
                    columnSeparator = CsvUtility.COMMA_COLUMN_SEPARATOR;
                }
                else if (_separatorType == SeparatorType.Semicolon)
                {
                    columnSeparator = CsvUtility.SEMICOLON_COLUMN_SEPARATOR;
                }
            }
        }

        [SerializeField]
        private bool _escapeColumns = true;
        public bool escapeColumns
        {
            get => _escapeColumns;
            set => _escapeColumns = value;
        }


        [SerializeField]
        private char _columnSeparator = CsvUtility.DEFAULT_COLUMN_SEPARATOR;
        public char columnSeparator
        {
            get => _columnSeparator;
            set
            {
                _columnSeparator = value;

                foreach (DataGatheringBinding binding in dataBindings)
                {
                    binding.headerSeparator = _columnSeparator;
                }
            }
        }


        [SerializeField]
        private string _localExportPath = DEFAULT_EXPORT_FILE_NAME;
        public string localExportPath
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
        private bool _newExportFilePerPlaythrough = true;
        public bool newExportFilePerPlaythrough
        {
            get => _newExportFilePerPlaythrough;
            set => _newExportFilePerPlaythrough = value;
        }


        [SerializeField]
        private string _httpExportPath;
        public string httpExportPath
        {
            get => _httpExportPath;
            set => _httpExportPath = value;
        }


        // Triggers
        [SerializeField]
        private bool _exportDuringUpdateEnabled;
        public bool exportDuringUpdateEnabled
        {
            get => _exportDuringUpdateEnabled;
            set => _exportDuringUpdateEnabled = value;
        }


        [SerializeField]
        private InputActionReference[] _inputActionTrigger;
        public InputActionReference[] inputActionTrigger
        {
            get => _inputActionTrigger;
            set => _inputActionTrigger = value;
        }


        [SerializeField]
        private bool _periodicExportEnabled = false;
        public bool periodicExportEnabled
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
        private float _periodicExportTime = 1.0f;
        public float periodicExportTime
        {
            get => _periodicExportTime;
            set => _periodicExportTime = value;
        }


        // Data
        [SerializeField]
        [Tooltip("Includes a timestamp in a human-readable format ('yyyy-MM-dd HH:mm:ss'). "
                    + "Its value is relative to the computers local timezone.")]
        private bool _includeHumanReadableTimestamp = true;
        public bool includeHumanReadableTimestamp
        {
            get => _includeHumanReadableTimestamp;
            set => _includeHumanReadableTimestamp = value;
        }

        [SerializeField]
        private bool _includeUnixTimestamp = true;
        public bool includeUnixTimestamp
        {
            get => _includeUnixTimestamp;
            set => _includeUnixTimestamp = value;
        }


        [SerializeField]
        private bool _includeUnityTime = true;
        public bool includeUnityTime
        {
            get => _includeUnityTime;
            set => _includeUnityTime = value;
        }

        [SerializeField]
        private bool _includeDeltaTime = true;
        public bool includeDeltaTime
        {
            get => _includeDeltaTime;
            set => _includeDeltaTime = value;
        }


        [SerializeField]
        private DataGatheringBinding[] _dataBindings = new DataGatheringBinding[0];
        public DataGatheringBinding[] dataBindings
        {
            get => _dataBindings;
            set => _dataBindings = value;
        }


        [SerializeField]
        private InputActionReference[] _inputActionDataBindings = new InputActionReference[0];
        public InputActionReference[] inputActionDataBindings
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
            if (exportDuringUpdateEnabled)
            {
                ExportNewCSVLine();
            }
        }

        #region Export
        public void ExportNewCSVLine()
        {
            if (!isActiveAndEnabled)
            {
                Debug.LogError("Trying to Export a new CSV line of a disabled DataGatherer. This is not allowed.");
                return;
            }

            string data = GetExportCSVLine();
            if (dataExportType == ExportType.Http || dataExportType == ExportType.Both)
            {
                // Debug.Log($"Posting '{httpExportPath}' to '{data}'.");
                StartCoroutine(PostHttpData(httpExportPath, data));
            }

            if (dataExportType == ExportType.Local || dataExportType == ExportType.Both)
            {
                // Debug.Log($"Saving '{data}' at '{GetLocalSavePath()}'.");
                _outputWriter.WriteLine(data);
            }
        }


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
            bindingHeaders.AddRange(_dataBindings.Select(v => v != null ? v.exportColumnName : ""));
            escapeIndividual.AddRange(_dataBindings.Select(v => !(v?.IsBoundToMultiColumnValue() ?? false) && _escapeColumns));
            // Add InputAction bindings
            bindingHeaders.AddRange(_inputActionDataBindings.Select(v => v != null ? v.name : ""));
            escapeIndividual.AddRange(Enumerable.Repeat(_escapeColumns, bindingHeaders.Count - escapeIndividual.Count));
            
            // Convert to string
            if (escapeColumns)
            {
                return CsvUtility.JoinAsCsv(bindingHeaders, escapeIndividual, columnSeparator);
            }
            return CsvUtility.JoinAsCsv(bindingHeaders.ToArray(), columnSeparator, false);
        }


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
            bindingValues.AddRange(_inputActionDataBindings.Select(v => v != null ? v.action.ReadValueAsObject().ToString() : ""));
            escapeIndividual.AddRange(Enumerable.Repeat(false, bindingValues.Count - escapeIndividual.Count));

            // Convert to string
            if (escapeColumns)
            {
                return CsvUtility.JoinAsCsv(bindingValues, escapeIndividual, columnSeparator);
            }
            return CsvUtility.JoinAsCsv(bindingValues.ToArray(), columnSeparator, false);
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

            if (dataExportType == ExportType.Http || dataExportType == ExportType.Both)
            {
                StartCoroutine(PostHttpData(httpExportPath, GetExportCSVHeader()));
            }

            if (dataExportType == ExportType.Local || dataExportType == ExportType.Both)
            {
                if (!HasExportableFileEnding(localExportPath))
                {
                    Debug.LogWarning("File does not end on '.txt', '.log' or '.csv'."
                            + "Appending '.csv' and creating a new file if necessary. "
                            + $"New path is: '{localExportPath}.csv'.");
                    _localExportPath += ".csv";
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
            foreach (InputActionReference actionRef in inputActionTrigger)
            {
                if (actionRef != null)
                {
                    actionRef.action.performed += OnInputActionExportRequested;
                }
            }
        }

        private void DisconnectInputActions()
        {
            foreach (InputActionReference actionRef in inputActionTrigger)
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
            if (periodicExportEnabled && Application.isPlaying)
            {
                if (_periodicExportTime > 0)
                {
                    if (_periodicExportCoroutine != null)
                    {
                        StopCoroutine(_periodicExportCoroutine);
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

        private void OnInputActionExportRequested(InputAction.CallbackContext callback) => ExportNewCSVLine();
        #endregion

        #region Utility
        public string GetLocalSavePath()
        {
            string path = _newExportFilePerPlaythrough ? InsertBeforeExportPostfixes(localExportPath, $"_{timestampSafe}") : localExportPath;

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

        private static bool HasExportableFileEnding(string path) => EXPORT_FILE_ENDINGS.Any(ending => path.EndsWith($".{ending}"));

        private static string InsertBeforeExportPostfixes(string path, string toInsert)
        {
            string pattern = $"(\\.{string.Join("|\\.", EXPORT_FILE_ENDINGS)})$";

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
            separatorType = _separatorType;
            columnSeparator = _columnSeparator;
            ValidateBindings(false);
        }
        #endregion

        #region Enums
        public enum SeparatorType
        {
            Semicolon,
            Comma,
            Custom
        }


        public enum ExportType
        {
            Local,
            Http,
            Both
        }
        #endregion
    }
}