using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery
{
    public class LineRespawnInvoker : MonoBehaviour
    {
        public UnityEvent<Collider> OnTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (isActiveAndEnabled && other.CompareTag("Respawn"))
            {
                OnTriggered?.Invoke(other);
            }
        }
    }
}