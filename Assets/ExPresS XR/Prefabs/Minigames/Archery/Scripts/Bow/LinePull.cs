
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class StringInteraction : XRBaseInteractable
{
    /*------------------------------------------------------------------------------------
    This Script handles the String-Bow interaction.
    The original idea is inspired by https://fistfullofshrimp.com/unity-vr-bow-and-arrow-part-1/
    -----------------------------------------------------------------------------------
    */
    [SerializeField,Tooltip("Event when String is released")]
    public UnityEvent<float> StringReleased;  

    // setting the pull amount for the bow
    [SerializeField,Tooltip("startposition")]
    private Transform startPosition;
    [SerializeField, Tooltip("endposition")]
    private Transform endPosition;
    //anchor on the Line
    [SerializeField, Tooltip("anchor")]
    private Transform anchor;
    [SerializeField, Tooltip("Prefab for the locked Arrow (locked in bow)")]
    private GameObject ArrowLockedPrefab;

    //audiosource and clip for the pulling
    private AudioSource audiosource;
    [SerializeField, Tooltip("Pull String Sound")]
    private AudioClip pullStringSound;


    //needed for debugging -----
    [SerializeField,Tooltip("debugging -> arrows will fly automatically")]
    private bool debug;
    private int badtimer = 0;
    //------- 


  //baseinteractor 
    private XRBaseInteractor stringInteractor = null;
    public float pull { get; private set; } = 0.0f;



    //for debugging -> activate debug bool if needed
    private void Update()
    {
        if (debug && badtimer > 100)
        {
            StringReleased?.Invoke(0.5f);
            badtimer = 0;
        }
        else
        {
            badtimer += 1;

        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        stringInteractor = args.interactor;
        audiosource = GetComponent<AudioSource>();
        audiosource.PlayOneShot(pullStringSound, 0.5f);
        ArrowLockedPrefab.SetActive(true);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        stringInteractor = null;
        StringReleased?.Invoke(pull);
        ArrowLockedPrefab.SetActive(false);
        pull = 0f;
        Updatestring();

    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected)
        {
            Vector3 pullPosition = stringInteractor.transform.position;
            pull = CalculatePull(pullPosition);
            Updatestring();
        }
    }

    private float CalculatePull(Vector3 pullPosition)
    {
        Vector3 pullDir = pullPosition - startPosition.position;
        Vector3 targetDir = endPosition.position - startPosition.position;
        float max = targetDir.magnitude;

        targetDir.Normalize();

        float pullValue = Vector3.Dot(pullDir, targetDir) / max;
        return Mathf.Clamp(pullValue, 0, 1);           
    }
    
    private void Updatestring()
    {
        Vector3 line = new Vector3(1,0,0) * Mathf.Lerp(startPosition.localPosition.x, endPosition.localPosition.x, pull);    
                          
        LineRenderer _lineRenderer = GetComponent<LineRenderer>();
        line = line + new Vector3(0 , _lineRenderer.GetPosition(1).y, 0);
        _lineRenderer.SetPosition(1, line);        
        anchor.localPosition = new Vector3(line.x, anchor.transform.localPosition.y/*line.y*/, line.z);
    }
}
