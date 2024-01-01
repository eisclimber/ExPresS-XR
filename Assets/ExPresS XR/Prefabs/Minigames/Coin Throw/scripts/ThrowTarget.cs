using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThrowTarget : MonoBehaviour
{
    public UnityEvent<int> OnHit;
    public UnityEvent OnHitInfo;

    [SerializeField, Tooltip("Score if hit")]
    private int score;

    [Tooltip("If hits should be detected")]
    [SerializeField]
    private bool _detectHits;
    public bool detectHits
    {
        get => _detectHits;
        set => _detectHits = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_detectHits && other != null && other.attachedRigidbody != null 
                && other.attachedRigidbody.TryGetComponent(out CoinReset _))
        {
            OnHit?.Invoke(score);
            OnHitInfo?.Invoke();
        }
    }

}
