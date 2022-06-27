using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HighlightableSocketInteractor : XRSocketInteractor
{   

    [SerializeField]
    protected bool _showHighlighter;
    public bool showHighlighter
    {
        get => _showHighlighter;
        set
        {
            _showHighlighter = value;

            if (_highlighterObject != null)
            {
                _highlighterObject.GetComponent<MeshRenderer>().enabled = _showHighlighter;
            }
        }
    }
    
    [SerializeField]
    protected GameObject _highlighterObject;
    private GameObject highlighterObject
    {
        get => _highlighterObject;
        set
        {
            _highlighterObject = value;

            if (_highlighterObject != null)
            {
                _highlighterObject.GetComponent<MeshRenderer>().enabled = _showHighlighter;
                _highlighterObject.transform.localScale = _highlighterScale;
            }
        }
    }

    [SerializeField]
    protected bool _useColliderSizeAsScale;
    public bool useColliderSizeAsScale
    {
        get => _useColliderSizeAsScale;
        set
        {
            _useColliderSizeAsScale = value && CanSetHighlighterScaleWithCollider();

            UpdateHighlighterScaleWithCollider();
        }
    }

    [Tooltip("The scale of the highlighterObject. Be sure to make it a little bit smaller (0.01f) to prevent z-fighting due to material overlapping.")]
    [SerializeField]
    protected Vector3 _highlighterScale = Vector3.one * 0.1f;
    public Vector3 highlighterScale
    {
        get => _highlighterScale;
        set
        {
            _highlighterScale = value;

            if (_highlighterObject != null)
            {
                _highlighterObject.transform.localScale = _highlighterScale;
            }
        }
    }

    protected override void Awake() {
        base.Awake();

        hoverEntered.AddListener(HideHighlighter);
        hoverExited.AddListener(ShowHighlighter);
    }


    protected void UpdateHighlighterScaleWithCollider()
    {
        if (!_useColliderSizeAsScale)
        {
            return;
        }

        if (GetComponent<SphereCollider>())
        {
            SphereCollider collider = GetComponent<SphereCollider>();

            highlighterScale = Vector3.one * (2 * collider.radius);
        }
        else if (GetComponent<BoxCollider>())
        {
            BoxCollider collider = GetComponent<BoxCollider>();

            highlighterScale = collider.size;
        }
        else
        {
            Debug.LogWarning("Did not find a SphereCollider nor a BoxCollider. Setting scale to (1, 1, 1).");
            highlighterScale = Vector3.one;
        }
    }


    public bool CanSetHighlighterScaleWithCollider()
    {
        return GetComponent<SphereCollider>() != null || GetComponent<BoxCollider>() != null;
    }


    private void ShowHighlighter(HoverExitEventArgs args)
    {
        if (_highlighterObject != null)
        {
            _highlighterObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void HideHighlighter(HoverEnterEventArgs args)
    {
        if (_highlighterObject != null)
        {
            _highlighterObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }


    private void OnValidate() {
        UpdateHighlighterScaleWithCollider();
    }
}
