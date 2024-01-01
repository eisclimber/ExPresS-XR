using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    public UnityEvent OnClick;

    void OnTriggerEnter(Collider other)
    {
        OnClick?.Invoke();
    }

}
