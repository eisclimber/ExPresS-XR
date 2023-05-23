using UnityEngine;


namespace ExPresSXR.Misc
{
    public class ObjectSpinner : MonoBehaviour
    {
        public Vector3 rotationAxis = Vector3.up;
        public float speed = 10.0f;
        public bool paused;

        private void Update()
        {
            if (!paused)
            {
                transform.Rotate(speed * Time.deltaTime * rotationAxis);
            }
        }
    }
}