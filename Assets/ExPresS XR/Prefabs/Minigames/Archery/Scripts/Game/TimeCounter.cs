using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    [SerializeField, Tooltip("Duration")]
    private float duration;

    [SerializeField, Tooltip("Reference to the  Text on the Counter Display")]
    private TMP_Text text;
    [SerializeField, Tooltip("Invoke after timer")]
    public UnityEvent timerup;


    public bool count = false;
    private float elapsedTime;


    private void Start()
    {
        elapsedTime = duration;
    }


    void Update()
    {
        if (count)
        {
            elapsedTime -= Time.deltaTime;
            text.text = elapsedTime.ToString("0.00");
            if (elapsedTime <= 0)
            {
                timerup?.Invoke();
                elapsedTime = 0f;
                text.text = elapsedTime.ToString("0.00");
                count = false;
            }
            
        }
    }

    public void Reset()
    {
        elapsedTime = duration;
    }
}
