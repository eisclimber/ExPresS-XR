using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObject : MonoBehaviour
{
    private Vector3 startpos;
    private Vector3 startdir;

    // Start is called before the first frame update
    void Start()
    {
        startpos = gameObject.transform.position;
        startdir = gameObject.transform.eulerAngles;
    }

    public void ResetObj()
    {
        gameObject.transform.position = startpos;
        gameObject.transform.eulerAngles = startdir;
    }
}
