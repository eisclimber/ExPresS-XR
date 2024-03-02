using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageVelocity : MonoBehaviour
{
    [SerializeField]
    private int _numSamples = 3;

    private List<Vector3> _samples = new();


    private void OnEnable()
    {
        // Clear old values
        _samples = new();
    }

    private void Update()
    {
        if (_samples.Count >= _numSamples)
        {
            _samples.RemoveAt(0);
        }
        _samples.Add(transform.position);
    }

    public Vector3 GetEstimatedVelocity()
    {
        Vector3 differences = Vector3.zero;
        for (int i = 0; i < _samples.Count - 1; i++)
        {
            differences += _samples[i + 1] - _samples[i];
        }
        int numSamples = Mathf.Max(1, _samples.Count - 1); // Ensure at least one sample
        return differences / numSamples;
    }

    public bool HasEnoughSamples() => _samples.Count == _numSamples;
}
