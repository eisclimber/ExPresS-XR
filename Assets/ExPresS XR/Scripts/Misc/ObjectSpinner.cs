using UnityEngine;


namespace ExPresSXR.Misc
{
    public class ObjectSpinner : MonoBehaviour
    {
        public Vector3 rotation;
        public float speed;
        public bool paused;

        private void Update()
        {
            if (!paused)
            {
                transform.Rotate(speed * Time.deltaTime * rotation);
            }
        }
    }
}