using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ExhibitionDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _displayedPrefab;
    public GameObject displayedPrefab
    {
        get => _displayedPrefab;
        set
        {
            _displayedPrefab = value;

            if (_displayedObjectInstance != null)
            {
                if (!Application.isPlaying)
                {
                    DestroyImmediate(_displayedObjectInstance);
                }
                else
                {
                    Destroy(_displayedObjectInstance);
                }
                _displayedObjectInstance = null;
            }

            if (_displayedPrefab != null)
            {
                if (_socket != null)
                {
                    _displayedObjectInstance = GameObject.Instantiate<GameObject>(_displayedPrefab, _socket.transform);
                    
                    bool canPickupAnswerObject = (_displayedObjectInstance.GetComponent<XRGrabInteractable>() != null);
                    
                    if (canPickupAnswerObject)
                    {
                        _socket.putBackObject = _displayedObjectInstance;
                    }
                    // Re-set the transform as the socket will remove the parenting
                    _displayedObjectInstance.transform.SetParent(transform);
                }
            }
        }
    }

    [SerializeField]
    private bool _spinObject;
    public bool spinObject
    {
        get => _spinObject;
        set
        {
            _spinObject = value;

            ObjectSpinner spinner = _socket.GetComponent<ObjectSpinner>();

            if (spinner == null)
            {
                spinner = _socket.gameObject.AddComponent<ObjectSpinner>();
                spinner.rotation = Vector3.up;
                spinner.speed = 30;
            }

            spinner.enabled = _spinObject;
        }
    }

    [SerializeField]
    private float _putBackTime;
    public float putBackTime
    {
        get => _putBackTime;
        set
        {
            _putBackTime = value;

            if (_socket != null)
            {
                _socket.putBackTime = putBackTime;
            }
        }
    }


    [SerializeField]
    private string _labelText;
    public string labelText
    {
        get => _labelText;
        set
        {
            _labelText = value;

            if (_labelTextGo != null)
            {
                _labelTextGo.text = _labelText;
            }
        }
    }

    [TextArea(3, 5)]
    [SerializeField]
    private string _infoText;
    public string infoText
    {
        get => _infoText;
        set
        {
            _infoText = value;

            if (_infoTextGo != null)
            {
                _infoTextGo.text = _infoText;
            }
        }
    }

    [SerializeField]
    private bool _usePhysicalInfoButton;
    public bool usePhysicalInfoButton
    {
        get => _usePhysicalInfoButton;
        set
        {
            _usePhysicalInfoButton = value;
            
            
            if (_uiShowInfoButtonCanvas != null)
            {
                _uiShowInfoButtonCanvas.gameObject.SetActive(!_usePhysicalInfoButton);
            }
            if (_worldShowInfoButton != null)
            {
                _worldShowInfoButton.gameObject.SetActive(_usePhysicalInfoButton);
            }
        }
    }

    [SerializeField]
    private bool _toggleInfoText;
    public bool toggleInfoText
    {
        get => _toggleInfoText;
        set 
        {
            _toggleInfoText = value;

            if (_worldShowInfoButton != null)
            {
                _worldShowInfoButton.toggleMode = toggleInfoText;
            }
        }
    }


    [Tooltip("Duration of how long the info is shown.")]
    [SerializeField]
    private float _showInfoTextDuration;
    public float showInfoTextDuration
    {
        get => _showInfoTextDuration;
        set => _showInfoTextDuration = value;
    }


    [SerializeField]
    private PutBackSocketInteractor _socket;
    public PutBackSocketInteractor socket
    {
        get => _socket;
        set
        {
            _socket = value;

            displayedPrefab = _displayedPrefab;
            putBackTime = _putBackTime;
        }
    }


    [SerializeField]
    private TMP_Text _labelTextGo;
    public TMP_Text labelTextGo
    {
        get => _labelTextGo;
        set
        {
            _labelTextGo = value;

            if (_labelTextGo != null)
            {
                _labelTextGo.text = _labelText;
            }
        }
    }


    [SerializeField]
    private Canvas _infoTextCanvas;
    public Canvas infoTextCanvas
    {
        get => _infoTextCanvas;
        set
        {
            _infoTextCanvas = value;
        }
    }

    [SerializeField]
    private TMP_Text _infoTextGo;
    public TMP_Text infoTextGo
    {
        get => _infoTextGo;
        set
        {
            _infoTextGo = value;

            if (_infoTextGo != null)
            {
                _infoTextGo.text = _infoText;
            }
        }
    }


    [SerializeField]
    private Button _uiShowInfoButton;
    public Button uiShowInfoButton
    {
        get => _uiShowInfoButton;
        set
        {
            _uiShowInfoButton = value;
        }
    }

    [SerializeField]
    private Canvas _uiShowInfoButtonCanvas;
    public Canvas uiShowInfoButtonCanvas
    {
        get => _uiShowInfoButtonCanvas;
        set
        {
            _uiShowInfoButtonCanvas = value;
        }
    }


    [SerializeField]
    private AutoXRBaseButton _worldShowInfoButton;
    public AutoXRBaseButton worldShowInfoButton
    {
        get => _worldShowInfoButton;
        set
        {
            _worldShowInfoButton = value;
        }
    }

    [SerializeField]
    private GameObject _displayedObjectInstance;

    private Coroutine showInfoCoroutine;


    private void Awake() {
        if (_uiShowInfoButton != null )
        {
            _uiShowInfoButton.onClick.AddListener(OnUiShowInfoButtonPressed);
        }
        if (_worldShowInfoButton != null)
        {
            toggleInfoText = _toggleInfoText;

            _worldShowInfoButton.OnPressed.AddListener(OnWorldShowInfoButtonPressed);
            
            _worldShowInfoButton.OnTogglePressed.AddListener(ShowInfoText);
            _worldShowInfoButton.OnToggleReleased.AddListener(HideInfoText);
        }

        displayedPrefab = _displayedPrefab;
        putBackTime = _putBackTime;
    }


    public void ShowInfoText() => SetInfoTextEnabled(true);

    public void HideInfoText() => SetInfoTextEnabled(false);


    public void SetInfoTextEnabled(bool textEnabled)
    {
        if (_infoTextCanvas != null)
        {
            _infoTextCanvas.gameObject.SetActive(textEnabled);
        }
        else
        {
            Debug.LogWarning("Please set the InfoTextCanvas reference.");
        }
    }

    private void OnUiShowInfoButtonPressed()
    {
        if (toggleInfoText)
        {
            if (_infoTextCanvas != null)
            {
                SetInfoTextEnabled(!_infoTextCanvas.gameObject.activeSelf);
            }
        }
        else
        {
            OnWorldShowInfoButtonPressed();
        }
        
    }

    private void OnWorldShowInfoButtonPressed()
    {
        if (showInfoCoroutine == null || (_infoTextCanvas != null && !_infoTextCanvas.gameObject.activeSelf))
        {
            // Show Button
            showInfoCoroutine = StartCoroutine(ShowInfoCoroutine());
        }
        else
        {
            StopShowInfoCoroutine();
        }
    }


    private void StopShowInfoCoroutine()
    {
        if (showInfoCoroutine != null)
        {
            StopCoroutine(showInfoCoroutine);
        }
        SetInfoTextEnabled(false);
    }


    private IEnumerator ShowInfoCoroutine()
    {
        SetInfoTextEnabled(true);
        yield return new WaitForSeconds(_showInfoTextDuration);
        showInfoCoroutine = null;
        SetInfoTextEnabled(false);
    }

    private void OnValidate() {
        labelText = _labelText;
        infoText = _infoText;
    }
}
