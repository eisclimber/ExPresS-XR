using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


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
    bool includeTimeStamp = true;

    [SerializeField]
    private List<DataGatheringBinding> _dataBindings;



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

        ValidateExportPath();
    }

    public void ExportNewCSVLine()
    {
        if (_dataExportType == DataGathererExportType.Http)
        {
            Debug.Log("(Currently not)Posting to : " + GetExportCSVLine() + " at " + GetLocalSavePath());
        }
        else
        {
            
            Debug.Log("(Currently not)Saving: " + GetExportCSVLine() + " at " + GetLocalSavePath());
        }
    }


    private string GetExportCSVLine()
    {
        string line = (includeTimeStamp ? System.DateTime.Now.ToString() + "," : "");
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


    private string GetExportCSVHeader()
    {
        string header = includeTimeStamp ? "time," : "";
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
    private void ValidateBindings()
    {
        for (int i = 0; i < _dataBindings.Count; i++)
        {
            if (_dataBindings[i].targetObject != null)
            {
                _dataBindings[i].ValidateBinding();

                if (!_dataBindings[i].bindingIsValid)
                {
                    Debug.LogWarning(("The binding to object '" + _dataBindings[i].targetObject
                        + "', component '" + _dataBindings[i].targetComponentName + "' and value/function '"
                        + _dataBindings[i].targetValueName + "' was invalid so the value in column '"
                        + _dataBindings[i].exportColumnName + "' will always be empty."));
                }
            }
        }
    }

    private void ValidateExportPath()
    {
        if (_dataExportType == DataGathererExportType.Http)
        {
            Debug.Log("(Currently not)Posting to : " + GetExportCSVLine() + " at " + GetLocalSavePath());
        }
        else
        {
            string path = GetLocalSavePath();
            try
            {
                // Throws an error if invalid
                string fullPath = Path.GetFullPath(path);
                if (!File.Exists(fullPath))
                {
                    // StreamWriter 
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
