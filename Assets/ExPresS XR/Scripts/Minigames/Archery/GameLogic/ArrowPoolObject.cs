using UnityEngine;

public class ArrowPoolObject : MonoBehaviour, IPoolObject
{
    [SerializeField]
    private Rigidbody _baseRb;

    [SerializeField]
    private Rigidbody _tipRb;

    public void RetrieveFromPool() => ZeroVelocities();
    public void ReturnToPool() => ZeroVelocities();

    private void ZeroVelocities()
    {
        if (_baseRb != null)
        {
            _baseRb.velocity = Vector3.zero;
            _baseRb.angularVelocity = Vector3.zero;
        }

        if (_tipRb != null)
        {
            _tipRb.velocity = Vector3.zero;
            _tipRb.angularVelocity = Vector3.zero;
            _tipRb.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}