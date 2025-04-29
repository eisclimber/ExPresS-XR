using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


namespace ExPresSXR.Minigames.Excavation
{
    public class ExcavationGame : MonoBehaviour
    {
        /// <summary>
        /// Utility value for the big corner gizmo sphere.
        /// </summary>
        private const float BIG_CORNER_MARKER_SIZE = 0.02f;

        /// <summary>
        /// Utility value for the small corner gizmo sphere.
        /// </summary>
        private const float SMALL_CORNER_MARKER_SIZE = 0.01f;

        /// <summary>
        /// Granularity of the area, i.e. how often it is subdivided.
        /// Both width and height will have 2^{_granularity} segments.
        /// </summary>
        [SerializeField]
        [Tooltip("Granularity of the area, i.e. how often it is subdivided. Both width and height will have 2^{_granularity} segments.")]
        private int _granularity = 1;

        /// <summary>
        /// Excavation zones defining which areas need to be excavated and to which extent.
        /// </summary>
        [SerializeField]
        [Tooltip("Excavation zones defining which areas need to be excavated and to which extent.")]
        private ExcavationZone[] _zones;

        [Space]

        /// <summary>
        /// Area be checked when evaluating the excavation progress.
        /// </summary>
        [SerializeField]
        [Tooltip("Area be checked when evaluating the excavation progress.")]
        private ExcavationArea _area;

        /// <summary>
        /// Transform where the grid will be drawn.
        /// </summary>
        [SerializeField]
        [Tooltip("Transform where the grid will be drawn.")]
        private Transform _gridTransform;

        /// <summary>
        /// Width and height of the grid gizmos so it can be custom fit with the excavation area. 
        /// Only used for debugging to line up the excavation zones.
        /// </summary>
        [SerializeField]
        [Tooltip("Width and height of the grid gizmos so it can be custom fit with the excavation area. Only used for debugging to line up the excavation zones.")]
        private Vector2 _gridGizmoDrawScale = new(1, 1);


        /// <summary>
        /// Number of horizontal segment of the grid. Always a power of two and equal to the GridHeight.
        /// </summary>
        public int GridWidth
        {
            get => (int)Mathf.Pow(2, _granularity);
        }

        /// <summary>
        /// Number of vertical segment of the grid. Always a power of two and equal to the GridWidth.
        /// </summary>
        public int GridHeight
        {
            get => (int)Mathf.Pow(2, _granularity);
        }

        /// <summary>
        /// Number of segments of each axis of the grid. Both values are equal and always a power of two.
        /// </summary>
        public Vector2 GridSize
        {
            get => new(GridWidth, GridHeight);
        }

        // Events

        /// <summary>
        /// Emitted once all zones are completed.
        /// </summary>
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

        /// <summary>
        /// Sets the _granularity of every zone. Used internally for the editor.
        /// </summary>
        public void EnforceZonesGranularity()
        {
            foreach (ExcavationZone zone in _zones)
            {
                zone.Granularity = _granularity;
            }
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
            Vector2 extents = _gridGizmoDrawScale / 2.0f;
            Transform atTransform = _gridTransform != null ? _gridTransform : transform;
            GizmoUtils.DrawGrid(Vector3.zero, _gridGizmoDrawScale / 2.0f, GridSize, Color.blue, Color.white, atTransform);

            // Bottom left = (0, 0)
            Gizmos.color = Color.red;
            Vector3 bottomLeft = new(-extents.x, 0.0f, -extents.y);
            Gizmos.DrawSphere(bottomLeft, BIG_CORNER_MARKER_SIZE);
            GizmoUtils.DrawLabel("(0, 0)", bottomLeft, atTransform);

            // Top right = (1, 1)
            Gizmos.color = Color.green;
            Vector3 topRight = new(extents.x, 0.0f, extents.y);
            Gizmos.DrawSphere(topRight, BIG_CORNER_MARKER_SIZE);
            GizmoUtils.DrawLabel($"({GridWidth - 1}, {GridHeight - 1})", topRight, atTransform);

            // Bottom right = (0, 1)
            Gizmos.color = Color.magenta;
            Vector3 bottomRight = new(-extents.x, 0.0f, extents.y);
            Gizmos.DrawSphere(bottomRight, SMALL_CORNER_MARKER_SIZE);
            GizmoUtils.DrawLabel($"(0, {GridHeight - 1})", bottomRight, atTransform);

            // Top left = (1, 0)
            Gizmos.color = Color.cyan;
            Vector3 topLeft = new(extents.x, 0.0f, -extents.y);
            Gizmos.DrawSphere(topLeft, SMALL_CORNER_MARKER_SIZE);
            GizmoUtils.DrawLabel($"({GridWidth - 1}, 0)", topLeft, atTransform);
        }
    }
}