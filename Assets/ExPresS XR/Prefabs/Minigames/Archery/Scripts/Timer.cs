using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    
    [SerializeField, Tooltip("Frequenze")]
    private float freq;
    [SerializeField, Tooltip("Delay")]
    private float delay = 0;
    private bool first = true;
    

    [SerializeField, Tooltip("Invoke after timer")]
    public UnityEvent timerempty;

    private float elapsedTime;
    
    
    private void Start()
    {
        elapsedTime = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        if (first)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > delay)
            {
                first = false;
                elapsedTime = 0f;
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > freq)
            {
                timerempty?.Invoke();
                elapsedTime = 0f;
            }
        }
        
    }
    
}
