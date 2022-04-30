using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

// Credits to VR with Andrew on Youtube

public class AutoXRBaseButton : XRBaseInteractable
{
    private const float PRESS_PCT = 0.3f;

    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    public AudioClip pressedSound;
    public AudioClip releasedSound;

    public Transform baseAnchor;
    public Transform pushAnchor;

    [SerializeField]
    private float _yMin = 0.019f;

    [SerializeField]
    private float _yMax = 0.029f;

    [SerializeField]
    private Vector3 _colliderSize;
    public Vector3 colliderSize
    {
        get => _colliderSize;
        set
        {
            _colliderSize = value;

            if (pushAnchor != null && pushAnchor.GetComponent<BoxCollider>() != null)
            {
                pushAnchor.GetComponent<BoxCollider>().size = _colliderSize;
            }
        }
    }

    private bool _pressed = false;
    public bool pressed
    {
        get => _pressed;
    }

    private float triggerStartTime = 0.0f;

    private float previousHandHeight = 0.0f;
    private XRBaseInteractor hoverInteractor = null;

    ////////

    // Can be used to measure the time since between any point in time and a button press
    public void StartTriggerTime()
    {
        triggerStartTime = Time.time;
    }

    public float GetTimeSinceTriggerStarted()
    {
        return Time.time - triggerStartTime;
    }
    
    ////////

    protected override void Awake()
    {
        base.Awake();
        hoverEntered.AddListener(StartPress);
        hoverExited.AddListener(EndPress);

        triggerStartTime = Time.time;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        hoverEntered.RemoveListener(StartPress);
        hoverExited.RemoveListener(EndPress);
    }

    private void StartPress(HoverEnterEventArgs args)
    {
        hoverInteractor = (XRBaseInteractor)args.interactorObject;
        previousHandHeight = GetLocalYPosition(args.interactableObject.transform.position);
    }

    private void EndPress(HoverExitEventArgs args)
    {
        hoverInteractor = null;
        previousHandHeight = 0.0f;

        _pressed = false;
        SetYPosition(_yMax);
    }

    private void Start()
    {
        SetMinMax();
    }

    private void SetMinMax()
    {
        Collider collider = pushAnchor.GetComponent<Collider>();
        _yMin = pushAnchor.transform.localPosition.y - (collider.bounds.size.y * 0.5f);
        _yMax = pushAnchor.transform.localPosition.y;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (hoverInteractor != null)
        {
            float newHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
            float handDifference = previousHandHeight - newHandHeight;
            previousHandHeight = newHandHeight;

            float newPosition = pushAnchor.transform.localPosition.y - handDifference;
            SetYPosition(newPosition);

            CheckPress();
        }
    }

    private float GetLocalYPosition(Vector3 position)
    {
        Vector3 localPosition = transform.root.InverseTransformPoint(position);
        return localPosition.y;
    }

    private void SetYPosition(float position)
    {
        Vector3 newPosition = pushAnchor.localPosition;
        newPosition.y = Mathf.Clamp(position, _yMin, _yMax);
        pushAnchor.localPosition = newPosition;
    }

    private void CheckPress()
    {
        bool isDown = IsInDownPosition();

        // Debug.Log(isDown + "    " + pressed);
        if (isDown && !pressed)
        {
            _pressed = true;
            // Debug.Log("Pressed");
            OnPressed.Invoke();
        }
        else if (!isDown && pressed)
        {
            _pressed = false;
            // Debug.Log("Released");
            OnReleased.Invoke();
        }
    }

    private bool IsInDownPosition()
    {
        float downPct = (pushAnchor.transform.localPosition.y - _yMin) / (_yMax - _yMin);
        downPct = Mathf.Clamp(downPct, 0.0f, 1.0f);

        return (downPct <= PRESS_PCT);
    }

    private void OnValidate()
    {
        // Prevents weird behavior
        colliderSize = _colliderSize;
    }
}
