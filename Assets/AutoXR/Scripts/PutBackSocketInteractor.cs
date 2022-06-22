using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class PutBackSocketInteractor : XRSocketInteractor
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
                startingSelectedInteractable = null;
                _putBackInteractable.selectExited.RemoveListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.RemoveListener(ResetPutBackTimer);
            }

            if (_putBackObject != null)
            {
                if (!_putBackObject.TryGetComponent<XRBaseInteractable>(out _putBackInteractable))
                {
                    Debug.LogWarning("PutBackObject does not contain a XRBaseInteractable-Component. Adding one.");
                    _putBackInteractable = _putBackObject.AddComponent<XRBaseInteractable>();
                }

                startingSelectedInteractable = _putBackInteractable;
                _putBackInteractable.selectExited.AddListener(StartPutBackTimer);
                _putBackInteractable.selectEntered.AddListener(ResetPutBackTimer);
            }
        }
    }

    private XRBaseInteractable _putBackInteractable;


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
        return (interactable.transform.gameObject == _putBackInteractable.gameObject);
    }
}
