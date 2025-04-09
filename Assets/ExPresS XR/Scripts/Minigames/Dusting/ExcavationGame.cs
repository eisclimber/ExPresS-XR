using UnityEngine;
using ExPresSXR.Misc;
using UnityEngine.Events;
using UnityEngine.Rendering;
using ExPresSXR.Experimentation.DataGathering;
using UnityEditor;


namespace ExPResSXR.Minigames.ExcavationGame
{
    public class ExcavationGame : MonoBehaviour
    {
        private const float BIG_CORNER_MARKER_SIZE = 0.02f;
        private const float SMALL_CORNER_MARKER_SIZE = 0.01f;

        [SerializeField]
        private ExcavationArea _area;

        [SerializeField]
        private Transform _gridTransform;

        [SerializeField]
        private int _granularity = 1; // I.e. subdivisions of the area

        [SerializeField]
        private Vector2 _gridDrawScale = new(1, 1);

        [SerializeField]
        private ExcavationZone[] _zones;

        public int GridWidth
        {
            get => (int)Mathf.Pow(2, _granularity);
        }

        public int GridHeight
        {
            get => (int)Mathf.Pow(2, _granularity);
        }

        public Vector2 GridSize
        {
            get => new(GridWidth, GridHeight);
        }


        public UnityEvent OnAllCompleted;


        /// <summary>
        /// Checks if the average of the draw channel is above the completion threshold and emits the completion event if so.
        /// </summary>
        public void StartCompletionCheck()
        {
            // Prevent spamming and unnecessary GetAvgColor()-calls.
            if (!isActiveAndEnabled)
            {
                return;
            }

            _area.GetAvgColors(_granularity, CompletionCheck);
        }

        private void CompletionCheck(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                // Assuming the image data is not on the gpu yet, 
                // this usually happens if nothing was drawn/excavated yet -> not completed
                return;
            }

            Color32[] avgColors = request.GetData<Color32>().ToArray();

            bool allCompleted = true;
            int count = 0;
            foreach (ExcavationZone zone in _zones)
            {
                if (zone.CheckCompletion(GridWidth, avgColors))
                {
                    count++;
                }
                else
                {
                    allCompleted = false;
                }
            }

            if (allCompleted)
            {
                OnAllCompleted.Invoke();
            }
        }


        private void OnDrawGizmosSelected()
        {
            Vector2 extents = _gridDrawScale / 2.0f;
            GizmoUtils.DrawGrid(Vector3.zero, _gridDrawScale / 2.0f, GridSize, Color.blue, Color.white, _gridTransform != null ? _gridTransform : transform);

            Gizmos.color = Color.red; // Bottom left = (0, 0)
            Gizmos.DrawSphere(new(extents.x, 0.0f, extents.y), BIG_CORNER_MARKER_SIZE);
            Gizmos.color = Color.green; // Top right = (1, 1)
            Gizmos.DrawSphere(new(-extents.x, 0.0f, -extents.y), BIG_CORNER_MARKER_SIZE);

            Gizmos.color = Color.magenta; // Bottom right
            Gizmos.DrawSphere(new(-extents.x, 0.0f, extents.y), SMALL_CORNER_MARKER_SIZE);
            Gizmos.color = Color.cyan; // Top left
            Gizmos.DrawSphere(new(extents.x, 0.0f, -extents.y), SMALL_CORNER_MARKER_SIZE);
        }
    }
}