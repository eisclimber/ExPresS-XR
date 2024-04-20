using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExPresSXR.Misc
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshInverter : MonoBehaviour
    {
        private void Start()
        {
            if (TryGetComponent(out MeshFilter meshFilter))
            {
                Mesh mesh = meshFilter.mesh;
                Vector3[] normals = mesh.normals;

                for (int i = 0; i < normals.Length; i++)
                {
                    normals[i] *= -1.0f;
                }

                mesh.normals = normals;

                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    int[] triangles = mesh.GetTriangles(i);

                    for (int j = 0; j < triangles.Length; j += 3)
                    {
                        (triangles[j], triangles[j + 1]) = (triangles[j + 1], triangles[j]);
                    }

                    mesh.SetTriangles(triangles, i);
                }
            }
            else
            {
                Debug.LogError($"MeshInverter did not find a Mesh on '{gameObject.name}' so it could not be inverted.");
            }

        }
    }
}