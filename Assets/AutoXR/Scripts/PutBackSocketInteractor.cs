using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PutBackSocketInteractor : HighlightableSocketInteractor
{
    [SerializeField]
    private GameObject _putBackObject;
    public GameObject putBackObject
    {
        get => _putBackObject;
        set
        {
            _putBackObject = value;

            // Remove previous Interactable
            if (_putBackInteractable != null)
            {
                if (Application.isPlaying && _putBackInteractable.isSelected)
                {
                    interactionManager.SelectExit(this, (IXRSelectInteractable) _putBackInteractable);
                }

                startingSelectedInteractable = null;
                _putBackInteractable.selectExited.RemoveListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.RemoveListener(ResetPutBackTimer);
            }

            if (_putBackObject != null)
            {
                _putBackInteractable = _putBackObject.GetComponent<XRGrabInteractable>();
                if (_putBackInteractable)
                {
                    startingSelectedInteractable = _putBackInteractable;
                    _putBackObject.transform.position = transform.position;
                    if (_putBackInteractable != null && Application.isPlaying)
                    {
                        interactionManager.SelectEnter(this, (IXRSelectInteractable) _putBackInteractable);
                    }

                    _putBackInteractable.selectExited.AddListener(StartPutBackTimer);
                    _putBackInteractable.selectEntered.AddListener(ResetPutBackTimer);
                }
            }
        }
    }

    private XRGrabInteractable _putBackInteractable;


    [SerializeField]
    private float _putBackTime;
    public float putBackTime
    {
        get => _putBackTime;
        set => _putBackTime = value;
    }

    private Coroutine putBackCoroutine;


    protected override void Awake() 
    {
        base.Awake();

        putBackObject = _putBackObject;
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && IsObjectMatch(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && IsObjectMatch(interactable) ;
    }

    private void StartPutBackTimer(SelectExitEventArgs args)
    {
        if (_putBackInteractable == null || args.interactorObject == (IXRSelectInteractor) this)
        {
            // Do nothing if the interactable does not exists or is exiting this object, 
            // e.g. was picked up from socket
            return;
        }
        if (putBackCoroutine != null)
        {
            StopCoroutine(putBackCoroutine);
        }

        if (_putBackTime <= 0)
        {
            // Put Object back
            interactionManager.SelectEnter(this, (IXRSelectInteractable) _putBackInteractable);
        }
        else
        {
            putBackCoroutine = StartCoroutine(CreatePutBackCoroutine(_putBackTime));
        }
    }

    private void ResetPutBackTimer(SelectEnterEventArgs args)
    {
        if (putBackCoroutine != null)
        {
            StopCoroutine(putBackCoroutine);
        }
    }


    private IEnumerator CreatePutBackCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        // Put Object back
        interactionManager.SelectEnter(this, (IXRSelectInteractable) _putBackInteractable);
        // EndManualInteraction();
        putBackCoroutine = null;
    }

    private bool IsObjectMatch(IXRInteractable interactable) 
    {
        return (_putBackInteractable != null && interactable.transform.gameObject == _putBackInteractable.gameObject);
    }
}
