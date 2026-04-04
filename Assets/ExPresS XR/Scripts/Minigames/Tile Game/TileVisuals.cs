using System;
using System.Collections;
using UnityEngine;
using ExPresSXR.Minigames.Common;
using ExPresSXR.Misc;
using UnityEditor;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// Controls the visual appearance of a tile of the tile game.
    /// </summary>
    public class TileVisuals : MonoBehaviour
    {
        /// <summary>
        /// Offset for drawing the labels displaying the area types configured for the tile.
        /// </summary>
        private const float GIZMO_LABEL_OFFSET = 0.075f;

        [SerializeField]
        [Tooltip("Tile data to be displayed.")]
        private Tile _displayedTile = null;
        /// <summary>
        /// Tile data to be displayed.
        /// </summary>
        public Tile DisplayedTile
        {
            get => _displayedTile;
            set
            {
                _displayedTile = value;
                UpdateVisuals();
            }
        }

        /// <summary>
        /// Renderer to set the area materials in.
        /// </summary>
        [SerializeField]
        [Tooltip("Renderer to set the area materials in.")]
        private Renderer _renderer;

        [SerializeField]
        [Tooltip("Areas to be displayed. Should be managed and set by the TileGame.")]
        [ReadonlyInInspector]
        private AreaDescription[] _areas;
        /// <summary>
        /// Areas to be displayed. Should be managed and set by the TileGame.
        /// </summary>
        public AreaDescription[] Areas
        {
            get => _areas;
            set
            {
                _areas = value;

                if (_areas != null)
                {
                    UpdateVisuals();
                }
            }
        }

        [Space]

        /// <summary>
        /// Mapping between the renderers materials and area ids.
        /// </summary>
        [SerializeField]
        [Tooltip("Mapping between the renderers materials and area ids.")]
        private MaterialMapping _materialIdxs;


        /// <summary> Utility accessor for the center area id with -1 if the displayed tile is null. </summary>
        public int CenterAreaId => _displayedTile != null ? _displayedTile.CenterAreaId : -1;
        /// <summary> Utility accessor for the top area id with -1 if the displayed tile is null. </summary>
        public int UpAreaId => _displayedTile != null ? _displayedTile.UpAreaId : -1;
        /// <summary> Utility accessor for the bottom area id with -1 if the displayed tile is null. </summary>
        public int DownAreaId => _displayedTile != null ? _displayedTile.DownAreaId : -1;
        /// <summary> Utility accessor for the left area id with -1 if the displayed tile is null. </summary>
        public int LeftAreaId => _displayedTile != null ? _displayedTile.LeftAreaId : -1;
        /// <summary> Utility accessor for the right area id with -1 if the displayed tile is null. </summary>
        public int RightAreaId => _displayedTile != null ? _displayedTile.RightAreaId : -1;



        [ContextMenu("Update Visuals")]
        private void UpdateVisuals()
        {
            if (_displayedTile == null || _renderer == null || _areas == null || _areas.Length <= 0)
            {
                return;
            }

#if UNITY_EDITOR
            // Record the change
            Undo.RecordObject(this, "Update visuals");
#endif
            Material[] newMats = Application.isPlaying ? _renderer.materials : _renderer.sharedMaterials;
            // Materials are messed up when exported for some reason...
            // Left, Up, Right, Down, Center
            newMats[_materialIdxs.Center] = _areas[CenterAreaId].Material;
            newMats[_materialIdxs.Up] = _areas[UpAreaId].Material;
            newMats[_materialIdxs.Down] = _areas[DownAreaId].Material;
            newMats[_materialIdxs.Left] = _areas[LeftAreaId].Material;
            newMats[_materialIdxs.Right] = _areas[RightAreaId].Material;
            _renderer.materials = newMats;

#if UNITY_EDITOR
            // Mark the object as dirty to trigger save
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Rotates the tile data the specified amount of degrees.
        /// </summary>
        /// <param name="degrees">Degrees to rotate.</param>
        public void RotateTileDataDegrees(float degrees) => _displayedTile.RotateDegrees(degrees);

        /// <summary>
        /// Rotates the tile data one step (= 90 degrees).
        /// </summary>
        [ContextMenu("Rotate Tile Data")]
        public void RotateTileData() => _displayedTile?.Rotate(1);

        /// <summary>
        /// Rotates the tile data a certain amount of 90 degree steps.
        /// </summary>
        /// <param name="steps">Steps to rotate.</param>
        public void RotateTileData(int steps) => _displayedTile?.Rotate(steps);

        /// <summary>
        /// Randomizes the displayed area types.
        /// </summary>
        [ContextMenu("Randomize Area Types")]
        public void RandomizeAreaTypes()
        {
#if UNITY_EDITOR
            // Record the change (no need to set it dirty as it will be by chaning the DisplayedTile()
            Undo.RecordObject(this, "Update visuals");
#endif
            DisplayedTile = new(_areas.Length);
        }

        private void OnDrawGizmosSelected()
        {
            if (_displayedTile == null)
            {
                GizmoUtils.DrawLabel("X", Vector3.zero, Color.red, transform);
                return;
            }

            bool _areasValid = _areas != null && _areas.Length > 0;
            if (!_areasValid)
            {
                GizmoUtils.DrawLabel("No _areas set, can't display tile colors.", Vector3.up * GIZMO_LABEL_OFFSET / 4.0f, transform);
            }

            GizmoUtils.DrawLabel($"{DisplayedTile.CenterAreaId}", new Vector3(1.0f, 0.0f, 1.0f) * GIZMO_LABEL_OFFSET / 4.0f, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.UpAreaId}", Vector3.forward * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.DownAreaId}", -Vector3.forward * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.LeftAreaId}", Vector3.left * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.RightAreaId}", Vector3.right * GIZMO_LABEL_OFFSET, transform);
        }
    }

    /// <summary>
    /// Helper class for mapping a renderers materials to the areas as the material ordering is not consistent.
    /// Default values are set up for the example model coming with the project.
    /// </summary>
    [Serializable]
    public class MaterialMapping
    {
        /// <summary>
        /// Renderer material index for the center area.
        /// </summary>
        public int Center = 1;

        /// <summary>
        /// Renderer material index for the top area.
        /// </summary>
        public int Up = 4;
        /// <summary>
        /// Renderer material index for the bottom area.
        /// </summary>
        public int Down = 2;
        /// <summary>
        /// Renderer material index for the left area.
        /// </summary>
        public int Left = 3;
        /// <summary>
        /// Renderer material index for the right area.
        /// </summary>
        public int Right = 5;
    }
}