using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using ExPresSXR.Misc;

namespace ExPresSXR.Movement
{
    public class MapPointManager : MonoBehaviour
    {
        private const string MAP_POINT_LOCATION = "Movement/Map Point";
        private const float POST_TELEPORT_WAIT_TIME = 0.1f;

        /// <summary>
        /// Map points to be managed.
        /// </summary>
        [SerializeField]
        private List<MapPoint> _mapPoints;

        /// <summary>
        /// Map Point which is initially active.
        /// </summary>
        [SerializeField]
        private MapPoint _initialMapPoint;

        [Space]

        /// <summary>
        /// InputActionReference to the action that enters TP-Mode with the left hand.
        /// </summary>
        [SerializeField]
        private InputActionReference _leftTeleportModeActivate;

        /// <summary>
        /// InputActionReference to the action that enters TP-Mode with the right hand.
        /// </summary>
        [SerializeField]
        private InputActionReference _rightTeleportModeActivate;

        [Space]

        /// <summary>
        /// InputActionReference to the action that cancels TP-Mode with the left hand.
        /// </summary>
        [SerializeField]
        private InputActionReference _leftTeleportModeCancel;

        /// <summary>
        /// InputActionReference to the action that cancels TP-Mode with the right hand.
        /// </summary>
        [SerializeField]
        private InputActionReference _rightTeleportModeCancel;


        private MapPoint _currentMapPoint;

        private Coroutine _postTeleportCoroutine;


        private void Awake()
        {
            if (_leftTeleportModeActivate == null && _rightTeleportModeActivate == null)
            {
                Debug.LogError("No _[left/right]teleportModeActivate was provided, you will not be able to activate MapPoints.");
            }
        }

        private void Start()
        {
            SwitchMapPoint(_initialMapPoint);
        }

        private void OnEnable()
        {
            SetupTeleportEvents();
        }

        private void OnDisable()
        {
            TeardownTeleportEvents();
        }


        /// <summary>
        /// Deactivates the current active MapPoint and switches to the new one.
        /// </summary>
        /// <param name="newMapPoint">The MapPoint to switch to</param>
        public void SwitchMapPoint(MapPoint newMapPoint)
        {
            // Hide previous teleports
            if (_currentMapPoint != null)
            {
                _currentMapPoint.SetTeleportModeVisible(false);
            }

            // Change Value
            _currentMapPoint = newMapPoint;
        }


        /// <summary>
        /// Sets all MapPoints active or inactive.
        /// </summary>
        /// <param name="mapPointActive">If the points should be active or inactive</param>
        public void SetMapPointsActive(bool mapPointActive)
        {
            foreach (MapPoint mapPoint in _mapPoints)
            {
                mapPoint.gameObject.SetActive(mapPointActive);
            }
        }


        private void SetupTeleportEvents()
        {
            // Activate
            InputAction leftTeleportModeActivateAction = _leftTeleportModeActivate?.action;
            if (leftTeleportModeActivateAction != null)
            {
                leftTeleportModeActivateAction.performed += OnStartTeleport;
                leftTeleportModeActivateAction.canceled += OnCancelTeleport;
            }

            InputAction rightTeleportModeActivateAction = _rightTeleportModeActivate?.action;
            if (rightTeleportModeActivateAction != null)
            {
                rightTeleportModeActivateAction.performed += OnStartTeleport;
                rightTeleportModeActivateAction.canceled += OnCancelTeleport;
            }

            // Cancel
            InputAction leftTeleportModeCancelAction = _leftTeleportModeCancel?.action;
            if (leftTeleportModeCancelAction != null)
            {
                leftTeleportModeCancelAction.performed += OnCancelTeleport;
            }

            InputAction rightTeleportModeCancelAction = _rightTeleportModeCancel?.action;
            if (rightTeleportModeCancelAction != null)
            {
                rightTeleportModeCancelAction.performed += OnCancelTeleport;
            }
        }


        private void TeardownTeleportEvents()
        {
            // Activate
            InputAction leftTeleportModeActivateAction = _leftTeleportModeActivate != null ? _leftTeleportModeActivate.action : null;
            if (leftTeleportModeActivateAction != null)
            {
                leftTeleportModeActivateAction.performed -= OnStartTeleport;
                leftTeleportModeActivateAction.canceled -= OnCancelTeleport;
            }

            InputAction rightTeleportModeActivateAction = _rightTeleportModeActivate != null ? _rightTeleportModeActivate.action : null;
            if (rightTeleportModeActivateAction != null)
            {
                rightTeleportModeActivateAction.performed -= OnStartTeleport;
                rightTeleportModeActivateAction.canceled -= OnCancelTeleport;
            }

            // Cancel
            InputAction leftTeleportModeCancelAction = _leftTeleportModeCancel != null ? _leftTeleportModeCancel.action : null;
            if (leftTeleportModeCancelAction != null)
            {
                leftTeleportModeCancelAction.performed -= OnCancelTeleport;
            }

            InputAction rightTeleportModeCancelAction = _rightTeleportModeCancel != null ? _rightTeleportModeCancel.action : null;
            if (rightTeleportModeCancelAction != null)
            {
                rightTeleportModeCancelAction.performed -= OnCancelTeleport;
            }
        }



        private void OnStartTeleport(InputAction.CallbackContext context)
        {
            // Stop after teleport coroutine just to be sure:)
            if (_postTeleportCoroutine != null)
            {
                StopCoroutine(_postTeleportCoroutine);
            }

            // Set Teleport Mode stuff visible
            if (_currentMapPoint != null)
            {
                _currentMapPoint.SetTeleportModeVisible(true);
            }
        }


        private void OnCancelTeleport(InputAction.CallbackContext context)
        {
            // Start Coroutine to let teleportation finish
            if (_postTeleportCoroutine != null)
            {
                StopCoroutine(_postTeleportCoroutine);
            }

            if (isActiveAndEnabled)
            {
                // Wait a bit to disable teleport mode
                _postTeleportCoroutine = StartCoroutine(PostCollisionWaitTimer());
            }
        }


        private IEnumerator PostCollisionWaitTimer()
        {
            yield return new WaitForSeconds(POST_TELEPORT_WAIT_TIME);

            // Set Normal Mode stuff visible
            if (_currentMapPoint != null)
            {
                _currentMapPoint.SetTeleportModeVisible(false);
            }

            _postTeleportCoroutine = null;
        }


        /// <summary>
        /// Creates the a new MapPoint.
        /// Attention!: Only available in the editor.
        /// </summary>
        public void CreateNewMapPointObject()
        {
#if UNITY_EDITOR
            string prefabPath = RuntimeEditorUtils.MakeExPresSXRPrefabPath(MAP_POINT_LOCATION);
            Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
            GameObject mapPointGo = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);

            if (mapPointGo != null && mapPointGo.TryGetComponent(out MapPoint mapPoint))
            {
                _mapPoints.Add(mapPoint);

                if (_mapPoints.Count == 1)
                {
                    _initialMapPoint = mapPoint;
                    Debug.Log($"Created the first MapPoint as a child of '{gameObject.name}' "
                            + $"and added it to it's managed MapPoints as well as initial mapPoint.", this);
                }
                else
                {
                    Debug.Log($"Created new MapPoint as a child of '{gameObject.name}' "
                            + $"and added it to it's managed MapPoints.", this);
                }
            }
            else
            {
                Debug.LogError($"Could not create a new MapPoint for '{gameObject.name}'. "
                                + "Make sure the default MapPoint was not deleted.", this);
            }
#else
            Debug.LogError("Creating of new MapPoints is only available in the Editor.", this);
#endif
        }
    }
}