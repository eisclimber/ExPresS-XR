using UnityEngine;
using UnityEngine.Events;

namespace ExPresSXR.Minigames.Archery
{
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

        private void Update()
        {
            if (first)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > delay)
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
}
