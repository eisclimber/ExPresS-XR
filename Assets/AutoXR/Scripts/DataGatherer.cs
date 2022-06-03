using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;


public class DataGatherer : MonoBehaviour
{
    [SerializeField]
    private DataGathererExportType _dataExportType;
    public DataGathererExportType dataExportType { get; set; }


    [SerializeField]
    private string localExportPath;

    [SerializeField]
    private string httpExportPath;


    // Triggers
    [SerializeField]
    InputActionReference[] inputActionTrigger;


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

    private Coroutine _periodicExportCoroutine;


    // Data
    [SerializeField]
    private bool _includeTimeStamp = true;


    [SerializeField]
    private List<DataGatheringBinding> _dataBindings;


    StreamWriter outputWriter;

    private void Start()
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

    private void OnDestroy() {
        if (outputWriter != null)
        {
            // Write everything that might not be written & close writer
            outputWriter.Flush();
            outputWriter.Close();
        }
    }

    public void ExportNewCSVLine()
    {
        string data = GetExportCSVLine();
        if (_dataExportType == DataGathererExportType.Http || _dataExportType == DataGathererExportType.Both)
        {
            Debug.Log(String.Format("Posting '{0}' to '{1}'.", httpExportPath, data));
            PostHttpData(httpExportPath, data);
        }
        if (_dataExportType == DataGathererExportType.Local || _dataExportType == DataGathererExportType.Both)
        {
            Debug.Log("Saving: " + data + " at " + GetLocalSavePath());
            outputWriter.WriteLine(data);
        }
    }


    public string GetExportCSVLine()
    {
        string line = (_includeTimeStamp ?  DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() : "");
        line += (_dataBindings.Count > 0 ? "," : "");
        for (int i = 0; i < _dataBindings.Count; i++)
        {
            line += _dataBindings[i].GetBindingValue();

            if (i < _dataBindings.Count - 1)
            {
                line += ",";
            }
        }
        return line;
    }


    public string GetExportCSVHeader()
    {
        string header = (_includeTimeStamp ? "time" : "");
        header += (_dataBindings.Count > 0 ? "," : "");
        for (int i = 0; i < _dataBindings.Count; i++)
        {
            header += _dataBindings[i].exportColumnName;

            if (i < _dataBindings.Count - 1)
            {
                header += ",";
            }
        }
        return header;
    }
    
    public void ValidateBindings()
    {
        for (int i = 0; i < _dataBindings.Count; i++)
        {

            if (_dataBindings[i] != null && !_dataBindings[i].ValidateBinding())
            {
                Debug.LogWarning(String.Format("The following binding is invalid and will always be empty: {0}", 
                                                _dataBindings[i].GetBindingDescription()));
            }
        }
    }

    private void SetupExport()
    {
        if (_dataExportType == DataGathererExportType.Local || _dataExportType == DataGathererExportType.Both)
        {
            StartCoroutine(PostHttpData(httpExportPath, GetExportCSVHeader()));
        }

        if (_dataExportType == DataGathererExportType.Local || _dataExportType == DataGathererExportType.Both)
        {
            string path = GetLocalSavePath();
            try
            {
                // Throws an error if invalid
                string fullPath = Path.GetFullPath(path);

                if (!File.Exists(fullPath)
                     && (!fullPath.EndsWith(".txt") || !fullPath.EndsWith(".csv") || !fullPath.EndsWith(".log")))
                {
                    localExportPath += ".csv";
                    fullPath += ".csv";
                    Debug.LogWarning("File does not exist and does not end on '.txt' or '.csv'."
                         + String.Format("Appending '.csv' and creating a new file if necessary. New path is: '{0}'. ", localExportPath));
                }

                outputWriter = new StreamWriter(fullPath);
                // If empty append csv header
                if (new FileInfo(fullPath).Length == 0)
                {
                    outputWriter.WriteLine(GetExportCSVHeader());
                    outputWriter.Flush();
                }
            } 
            catch (Exception e)
            {
                Debug.LogError("Export path '" + path + "' is invalid: " + e.Message);
            }
        }
    }

    private string GetLocalSavePath()
    {
#if UNITY_EDITOR
        return Path.Combine(Application.dataPath + "/Data/", localExportPath);
#elif UNITY_ANDROID
        return Path.Combine(Application.persistentDataPath + localExportPath);
#elif UNITY_IPHONE
        return Path.Combine(Application.persistentDataPath + "/" + localExportPath);
#else
        return Path.Combine(Application.dataPath + "/" + localExportPath);
#endif
    }


    private void TryStartPeriodicCoroutine()
    {
        if (periodicExportEnabled)
        {
            if (_periodicExportTime > 0)
            {
                if (_periodicExportCoroutine != null)
                {
                    StopCoroutine(_periodicExportCoroutine);
                }
                _periodicExportCoroutine = StartCoroutine("TimeTriggerCoroutine");
            }
            else
            {
                Debug.LogError("PeriodicExportTime must be greater than zero.");
            }
        }
    }


    private IEnumerator PostHttpData(string url, string data)
    {
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

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
            Debug.Log(String.Format("Failed to send data to server: '{0}'.", request.error));
        }
    }


    private IEnumerator TimeTriggerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_periodicExportTime);
            ExportNewCSVLine();
        }
    }

    private void OnInputActionExportRequested(InputAction.CallbackContext callback) => ExportNewCSVLine();
}


public enum DataGathererExportType
{
    Local,
    Http,
    Both
}
