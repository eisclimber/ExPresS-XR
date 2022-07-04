using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(MeshFilter))]
public class PlayAreaBoundingBox : MonoBehaviour
{
    const float DEFAULT_ROOM_HEIGHT = 2.5f;

    [SerializeField]
    private bool _useCustomBoundingBoxMaterial;
    public bool useCustomBoundingBoxMaterial
    {
        get => _useCustomBoundingBoxMaterial;
        set
        {
            _useCustomBoundingBoxMaterial = value;
            UpdateBoundarySize();
            GetComponent<MeshRenderer>().enabled = useCustomBoundingBoxMaterial;
        }
    }

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = useCustomBoundingBoxMaterial;
    }

    private void OnEnable()
    {
        InvertMesh();
        UpdateBoundarySize();
    }


    private void OnDisable()
    {
        // Disable/Stop input System
        List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances<XRInputSubsystem>(inputSubsystems);

        if (inputSubsystems.Count > 0)
        {
            inputSubsystems[0].Stop();
        }
    }

    private void UpdateBoundarySize()
    {
        List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances<XRInputSubsystem>(inputSubsystems);

        if (inputSubsystems.Count > 0)
        {
            XRInputSubsystem inputSubsystem = inputSubsystems[0];
            if (!inputSubsystem.running)
            {
                // Start the subsystem if not started yet
                inputSubsystem.Start();
            }
            List<Vector3> boundaryPoints = new List<Vector3>();
            if (inputSubsystem.TryGetBoundaryPoints(boundaryPoints))
            {
                SetSizeWithBoundaryPoints(boundaryPoints);
            }

            // if (_useCustomBoundingBoxMaterial)
            // {
            //     // Turn of the default bounding box by turning of the subsystem
            //     // NAH this kills the whole subsystem -> don't use it
            //     inputSubsystem.Stop();
            // }
        }
    }

    private void SetSizeWithBoundaryPoints(List<Vector3> boundaryPoints)
    {
        if (boundaryPoints.Count < 2)
        {
            return;
        }
        Vector3 minPoint = boundaryPoints[0];
        Vector3 maxPoint = boundaryPoints[0];

        foreach (Vector3 p in boundaryPoints)
        {
            minPoint.Set(Mathf.Min(minPoint.x, p.x), Mathf.Min(minPoint.y, p.y), Mathf.Min(minPoint.z, p.z));
            maxPoint.Set(Mathf.Max(maxPoint.x, p.x), Mathf.Max(maxPoint.y, p.y), Mathf.Max(maxPoint.z, p.z));
        }

        Vector3 diff = maxPoint - minPoint;

        if (diff.y == 0.0f)
        {
            // Some distributions, e.g. SteamVR, do not provide a height (all points at y = 0)
            diff.y = DEFAULT_ROOM_HEIGHT;
        }

        transform.localScale = diff;
        transform.position.Set(0, diff.y / 2 + 0.1f, 0);
    }

    private void InvertMesh()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;

        // Invert Normals
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -1 * normals[i];
        }
        mesh.normals = normals;

        // Invert Triangles to draw insides
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                //swap order of tri vertices
                int temp = triangles[j];
                triangles[j] = triangles[j + 1];
                triangles[j + 1] = temp;
            }
            mesh.SetTriangles(triangles, i);
        }
    }


    void OnDrawGizmos()
    {
        if (enabled)
        {
            // Draw a semitransparent red cube at the transforms position
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
