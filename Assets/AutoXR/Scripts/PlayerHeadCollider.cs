using UnityEngine;
using UnityEngine.Events;

// Thanks to metaanomie (https://metaanomie.blogspot.com/2020/04/unity-vr-head-blocking-steam-vr-v2.html)
// Attach this to the 'Main Camera' of an XROrigin and assign an anchor (e.g. the XROrigin's CameraOffset) 
// to prevent the camera clipping through walls

public class PlayerHeadCollider : MonoBehaviour
{
    [Tooltip("The anchor that is moved when collisions occur. Usually should be set to the AutoXRRig/XROrigin.")]
    [SerializeField]
    private GameObject _pushbackAnchor;

    [Tooltip("Determines how close the camera can get to a wall/object. Smaller values may allow looking through Objects at the edge of the view.")]
    [SerializeField]
    private float backupCap = .22f;

    [Tooltip("Will be invoked once when the first collidion with a wall occurs. Gets reset when no collision is detected anymore.")]
    [SerializeField]
    public UnityEvent onCollisionStarted;
    
    [Tooltip("Will be invoked once when a collidion with wall ends.")]
    [SerializeField]
    public UnityEvent onCollisionEnded;

    
    private LayerMask layerMask;
    private Collider[] objs = new Collider[10];
    private Vector3 prevHeadPos;
    private bool colliding;

    private void Start()
    {
        if (_pushbackAnchor == null)
        {
            Debug.LogError("No GameObject was set as pushback anchor!");
        }

        layerMask = LayerMask.NameToLayer("Everything");

        prevHeadPos = transform.position;
    }

    private int DetectHit(Vector3 loc)
    {
        int hits = 0;
        int size = Physics.OverlapSphereNonAlloc(loc, backupCap, objs, layerMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < size; i++)
        {
            if (objs[i].tag != "Player")
            {
                hits++;
            }
        }
        return hits;
    }

    public void Update()
    {
        if (_pushbackAnchor != null)
        {
            int hits = DetectHit(transform.position);

            // No collision
            if (hits == 0)
            {
                prevHeadPos = transform.position;

                // Collision Ending
                if (colliding)
                {
                    colliding = false;
                    onCollisionEnded.Invoke();
                }
            }

            // Collision
            else
            {
                // Player pushback  
                Vector3 headDiff = transform.position - prevHeadPos;
                if (Mathf.Abs(headDiff.x) > backupCap)
                {
                    if (headDiff.x > 0)
                    {
                        headDiff.x = backupCap;
                    }
                    else
                    {
                        headDiff.x = -backupCap;
                    }
                }
                if (Mathf.Abs(headDiff.z) > backupCap)
                {
                    if (headDiff.z > 0)
                    {
                        headDiff.z = backupCap;
                    }
                    else
                    {
                        headDiff.z = -backupCap;
                    }
                }
                Vector3 adjHeadPos = new Vector3(_pushbackAnchor.transform.position.x - headDiff.x,
                                                 _pushbackAnchor.transform.position.y,
                                                 _pushbackAnchor.transform.position.z - headDiff.z);
                _pushbackAnchor.transform.SetPositionAndRotation(adjHeadPos, _pushbackAnchor.transform.rotation);

                // Collision Started
                if (!colliding)
                {
                    colliding = true;
                    onCollisionEnded.Invoke();
                }
            }
        }
    }
}
