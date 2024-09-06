using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector3 direction;

    private void FixedUpdate()
    {
        gameObject.transform.position = gameObject.transform.position + direction * speed * Time.deltaTime;

    }

    //change movement according to given parameters
    public void ChangeMovement(float newspeed, Vector3 newdirection)
    {
        speed = newspeed;
        direction = newdirection;

    }

}
