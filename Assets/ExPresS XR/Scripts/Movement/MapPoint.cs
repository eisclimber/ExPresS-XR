using System.Collections;
using System.Collections.Generic;
using ExPresSXR.Misc;
using UnityEditor;
using UnityEngine;

namespace ExPresSXR.Movement
{
    public class MapPoint : MonoBehaviour
    {
        private const string TP_OPTION_PATH = "Assets/ExPresS XR/Prefabs/Movement/Teleport Option.prefab";
        private const float DEFAULT_TP_OPTION_CREATION_RADIUS = 2.0f;

        /// <summary>
        /// Visible when the player is in TP-Mode
        /// 
        /// !Attention!: This GameObject will stay be visible/active when the player is NOT present.
        /// To hide it, configure this as (child) of _playerPresentVisible.
        /// </summary>
        [SerializeField]
        private GameObject _showInTpMode;

        /// <summary>
        /// Visible when the player is NOT in TP-Mode. 
        /// 
        /// !Attention!: This GameObject will stay be visible/active when the player is NOT present.
        /// To hide it, configure this as (child) of _playerPresentVisible.
        /// </summary>
        [SerializeField]
        private GameObject _hideInTpMode;

        /// <summary>
        /// Visible when the player is present.
        /// </summary>
        [SerializeField]
        private GameObject _playerPresentVisible;

        
        // For adding Tp Options
        [SerializeField]
        private float _createdTpOptionRadius = DEFAULT_TP_OPTION_CREATION_RADIUS;


        private void Start()
        {
            SetTeleportModeVisible(false);
            SetPlayerPresent(false);
        }

        /// <summary>
        /// Changes visibility of the 'Show In TP Mode' and 'Hide In TP Mode' GameObjects.
        /// Their state will be always inverse ('Show In TP Mode'.active = !'Hide In TP Mode'.active)
        /// </summary>
        /// <param name="visible">If 'Show In TP Mode' should be visible or not</param>    
        public void SetTeleportModeVisible(bool visible)
        {
            if (_showInTpMode != null)
            {
                _showInTpMode.SetActive(visible);
            }

            if (_hideInTpMode != null)
            {
                _hideInTpMode.SetActive(!visible);
            }
        }

        /// <summary>
        /// (De-)activates the 'Player Present' GameObject.
        /// </summary>
        /// <param name="visible">If it should be visible or not</param>    
        public void SetPlayerPresent(bool present)
        {
            if (_playerPresentVisible != null)
            {
                _playerPresentVisible.SetActive(present);
            }
        }

        /// <summary>
        /// Creates the a new MapPoint, only available in the editor
        /// </summary>
        public void CreateNewTpOptionObject()
        {
#if UNITY_EDITOR
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(TP_OPTION_PATH, typeof(UnityEngine.Object));
            GameObject tpOption = (GameObject)PrefabUtility.InstantiatePrefab(prefab, _showInTpMode.transform);

            if (tpOption != null && tpOption.transform.childCount > 0)
            {
                Transform offsetTransform = tpOption.transform.GetChild(0);
                float distance = _createdTpOptionRadius >= 0.0f ? _createdTpOptionRadius : DEFAULT_TP_OPTION_CREATION_RADIUS;
                // Offset the transform of the first (and only) child of the by the specified distance
                offsetTransform.localPosition = Vector3.forward * distance;
                Debug.Log($"Created new Tp Option as a child of '{ _showInTpMode.gameObject.name }' "
                            + $"and offset it by { _createdTpOptionRadius }. Rotate it to your likings and "
                            + "set to 'Teleport Anchor Transform' of the TPOptions first child to your teleportation target.", this);
            }
            else
            {
                Debug.LogError($"Could not create a new Tp Option for '{ gameObject.name }'. Make sure the default tp Option was not deleted "
                                + "and it has at least one child to offset.", this);
            }
#else
            Debug.LogError("Creating of new TeleportOptions is only available in the Editor.", this);
#endif
        }
    }
}