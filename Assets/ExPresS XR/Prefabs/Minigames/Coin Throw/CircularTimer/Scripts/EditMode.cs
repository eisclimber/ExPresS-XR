using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditMode : MonoBehaviour {

    CircularTimer circularTimer;

    void OnEnable()
    {
        circularTimer = this.GetComponent<CircularTimer>();
    }

    void Update()
    {
        circularTimer.UpdateUI();
    }
}
