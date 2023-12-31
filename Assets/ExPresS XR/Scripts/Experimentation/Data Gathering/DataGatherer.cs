using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
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
        private DataGathererExportType _dataExportType;
        public DataGathererExportType dataExportType
        {
            get => _dataExportType;
            set => _dataExportType = value;
        }


        [SerializeField]
        private char _columnSeparator = CsvUtility.DEFAULT_COLUMN_SEPARATOR;
        public char columnSeparator
        {
            get => _columnSeparator;
            set => _columnSeparator = value;
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
        private bool _newExportFilePerPlaythrough;
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
        [Tooltip("Includes a timestamp in a human-readable format ('yyyy-MM-dd HH:mm:ss'). Its value is relative to the computers local timezone.")]
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
        private DataGatheringBinding[] _dataBindings;
        public DataGatheringBinding[] dataBindings
        {
            get => _dataBindings;
            set => _dataBindings = value;
        }


        [SerializeField]
        private InputActionReference[] _inputActionDataBindings;
        public InputActionReference[] inputActionDataBindings
        {
            get => _inputActionDataBindings;
            set => _inputActionDataBindings = value;
        }

        private Coroutine _periodicExportCoroutine;

        private StreamWriter _outputWriter;

        private void Awake()
        {
            TryStartPeriodicCoroutine();

            foreach (InputActionReference actionRef in inputActionTrigger)
            {
                if (actionRef != null)
                {
                    actionRef.action.performed += OnInputActionExportRequested;
                }
            }

            ValidateBindings();

            SetupExport();
        }

        private void FixedUpdate()
        {
            if (exportDuringUpdateEnabled)
            {
                ExportNewCSVLine();
            }
        }

        private void OnDestroy()
        {
            if (_outputWriter != null)
            {
                // Write everything that might not be written & close writer
                _outputWriter.Flush();
                _outputWriter.Close();
            }
        }

        public void ExportNewCSVLine()
        {
            if (!isActiveAndEnabled)
            {
                Debug.LogError("Trying to Export a new CSV line of a disabled DataGatherer. This is not allowed.");
                return;
            }

            string data = GetExportCSVLine();
            if (dataExportType == DataGathererExportType.Http || dataExportType == DataGathererExportType.Both)
            {
                // Debug.Log(String.Format("Posting '{0}' to '{1}'.", httpExportPath, data));
                StartCoroutine(PostHttpData(httpExportPath, data));
            }

            if (dataExportType == DataGathererExportType.Local || dataExportType == DataGathererExportType.Both)
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

            // Join all non-empty prepended headers together with commas in-between
            string header = string.Join(_columnSeparator, prependedHeaders.Where(s => !string.IsNullOrEmpty(s)));
            // Add another comma at the end if columns were added and there are more bindings to export
            header += (header != "" && HasBindingsToExport()) ? _columnSeparator : "";

            // Get Data Bindings headers
            for (int i = 0; i < _dataBindings.Length; i++)
            {
                header += _dataBindings[i].exportColumnName;

                if (i < _dataBindings.Length - 1 || _inputActionDataBindings.Length > 0)
                {
                    header += _columnSeparator;
                }
            }

            // Get Input Action Data Bindings Header
            for (int i = 0; i < _inputActionDataBindings.Length; i++)
            {
                header += _inputActionDataBindings[i].name;

                if (i < _dataBindings.Length - 1)
                {
                    header += _columnSeparator;
                }
            }
            return header;
        }


        public string GetExportCSVLine()
        {
            string[] prependedValues = {
                _includeHumanReadableTimestamp ? timestampPretty : "",
                _includeUnixTimestamp ? DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() : "",
                _includeUnityTime ? Time.time.ToString() : "",
                _includeDeltaTime ? Time.deltaTime.ToString() : ""
            };

            // Join all non-empty prepended values together with commas in-between
            string line = string.Join(_columnSeparator, prependedValues.Where(s => !string.IsNullOrEmpty(s)));
            // Add another comma at the end if values were added and there are more bindings to export
            line += (line != "" && HasBindingsToExport()) ? _columnSeparator : "";

            // Read Data Bindings
            for (int i = 0; i < _dataBindings.Length; i++)
            {
                line += _dataBindings[i].GetBindingValue(_columnSeparator);

                if (i < _dataBindings.Length - 1 || _inputActionDataBindings.Length > 0)
                {
                    line += _columnSeparator;
                }
            }
            // Read Input Action Data Bindings
            for (int i = 0; i < _inputActionDataBindings.Length; i++)
            {
                line += _inputActionDataBindings[i].action.ReadValueAsObject() ?? "null";

                if (i < _dataBindings.Length - 1)
                {
                    line += _columnSeparator;
                }
            }

            return line;
        }

        public void ValidateBindings()
        {
            for (int i = 0; i < _dataBindings.Length; i++)
            {
                if (_dataBindings[i] != null && !_dataBindings[i].ValidateBinding())
                {
                    Debug.LogWarning("The following binding is invalid and will always be empty: "
                        + $"{_dataBindings[i].GetBindingDescription()}", this);
                }
            }
        }

        private void SetupExport()
        {
            // Clean up old Output writer if exits
            if (_outputWriter != null)
            {
                _outputWriter.Flush();
                _outputWriter.Close();
            }

            if (dataExportType == DataGathererExportType.Http || dataExportType == DataGathererExportType.Both)
            {
                StartCoroutine(PostHttpData(httpExportPath, GetExportCSVHeader()));
            }

            if (dataExportType == DataGathererExportType.Local || dataExportType == DataGathererExportType.Both)
            {
                if (!HasExportableFileEnding(localExportPath))
                {
                    Debug.LogWarning("File does not end on '.txt', '.log' or '.csv'."
                            + $"Appending '.csv' and creating a new file if necessary. New path is: '{localExportPath}.csv'.");
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


        private bool HasBindingsToExport()
            => (_dataBindings != null && _dataBindings.Length > 0)
                || (_inputActionDataBindings != null && _inputActionDataBindings.Length > 0);

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


        private IEnumerator PostHttpData(string url, string data)
        {
            string actualUrl = _newExportFilePerPlaythrough ? url + $"_{timestampSafe}" : url;
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
                Debug.Log(string.Format("Failed to send data to server: '{0}'.", request.error));
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

        // Utiltiy

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
    }

    public enum DataGathererExportType
    {
        Local,
        Http,
        Both
    }
}