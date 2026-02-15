using System.Collections;
using UnityEngine;
using ExPresSXR.Minigames.Common;
using ExPresSXR.Misc;
using UnityEditor;

namespace ExPresSXR.Minigames.TileGame
{
    public class TileVisuals : MonoBehaviour
    {
        private const float GIZMO_LABEL_OFFSET = 0.075f;

        [SerializeField]
        private Tile _displayedTile = null;
        public Tile DisplayedTile
        {
            get => _displayedTile;
            set
            {
                _displayedTile = value;
                UpdateVisuals();
            }
        }

        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        [ReadonlyInInspector]
        private AreaDescription[] _areas;
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

        [SerializeField]
        private GameObject _pointsDisplayPrefab;

        [SerializeField]
        private float _subScoreShowDelay = 0.5f;

        [SerializeField]
        private float _pointsDisplayScale = 0.1f;

        [SerializeField]
        private float _pointsDisplayRadius = 0.08f;

        [Space]

        [SerializeField]
        private Vector3 _pointsDisplayOffset = new(0.0f, 0.02f, 0.0f);


        [SerializeField]
        private MaterialMapping _materialIdxs;

        private Coroutine _displayScoreCoroutine;


        /// <summary>
        /// Utility accessors
        /// </summary>
        public int CenterAreaId => _displayedTile != null ? _displayedTile.CenterAreaId : -1;
        public int TopAreaId => _displayedTile != null ? _displayedTile.TopAreaId : -1;
        public int BottomAreaId => _displayedTile != null ? _displayedTile.BottomAreaId : -1;
        public int LeftAreaId => _displayedTile != null ? _displayedTile.LeftAreaId : -1;
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
            // Left, Top, Right, Bottom, Center
            newMats[_materialIdxs.Center] = _areas[CenterAreaId].Material;
            newMats[_materialIdxs.Top] = _areas[TopAreaId].Material;
            newMats[_materialIdxs.Bottom] = _areas[BottomAreaId].Material;
            newMats[_materialIdxs.Left] = _areas[LeftAreaId].Material;
            newMats[_materialIdxs.Right] = _areas[RightAreaId].Material;
            _renderer.materials = newMats;

#if UNITY_EDITOR
            // Mark the object as dirty to trigger save
            EditorUtility.SetDirty(this);
#endif
        }

        public void DisplayScore(ScoreResults score)
        {
            // Maybe order the scores...
            if (_displayScoreCoroutine != null)
            {
                StopCoroutine(_displayScoreCoroutine);
            }
            _displayScoreCoroutine = StartCoroutine(ShowScoresSequential(score));
        }

        private IEnumerator ShowScoresSequential(ScoreResults score)
        {
            if (score.CenterScore > 0)
            {
                SpawnPointsDisplay(score.CenterScore, score.CenterAreaId, Vector3.zero);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.TopScore > 0)
            {
                SpawnPointsDisplay(score.TopScore, score.TopAreaId, Vector3.forward);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.BottomScore > 0)
            {
                SpawnPointsDisplay(score.BottomScore, score.BottomAreaId, Vector3.back);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.LeftScore > 0)
            {
                SpawnPointsDisplay(score.LeftScore, score.LeftAreaId, Vector3.left);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.RightScore > 0)
            {
                SpawnPointsDisplay(score.RightScore, score.RightAreaId, Vector3.right);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }
        }

        private void SpawnPointsDisplay(int points, int areaId, Vector3 direction)
        {
            GameObject scoreNumbersGo = Instantiate(_pointsDisplayPrefab);
            // Use offset of the reference or global offset
            scoreNumbersGo.transform.SetPositionAndRotation(transform.position + direction * _pointsDisplayRadius + _pointsDisplayOffset, Quaternion.identity);
            scoreNumbersGo.transform.localScale = Vector3.one * _pointsDisplayScale;

            if (scoreNumbersGo.TryGetComponent(out ScoreNumbers scoreNumbers))
            {
                scoreNumbers.SetupScoreData(points);
                if (areaId >= 0)
                {
                    Color areaColor = GetAreaIdColor(areaId);
                    scoreNumbers.SetScoreColor(areaColor);
                }
                else
                {
                    scoreNumbers.RandomizeColorHue();
                }
            }
        }

        public void RotateTileDataDegrees(float degrees) => _displayedTile.RotateDegrees(degrees);

        [ContextMenu("Rotate Tile Data")]
        public void RotateTileData() => _displayedTile?.Rotate(1);

        public void RotateTileData(int steps) => _displayedTile?.Rotate(steps);


        [ContextMenu("Display Test Score")]
        public void DisplayTestScore()
        {
            if (Application.isPlaying)
            {
                DisplayScore(new(1, CenterAreaId, 2, TopAreaId, 10, BottomAreaId, 1, LeftAreaId, 4, RightAreaId));
            }
            else
            {
                Debug.LogWarning("Not spawning scores, while the application is not running.", this);
            }

        }

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
            GizmoUtils.DrawLabel($"{DisplayedTile.TopAreaId}", Vector3.forward * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.BottomAreaId}", -Vector3.forward * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.LeftAreaId}", Vector3.left * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.RightAreaId}", Vector3.right * GIZMO_LABEL_OFFSET, transform);
        }

        private Color GetAreaIdColor(int areaId) => areaId >= 0 && areaId < _areas.Length ? _areas[areaId].Color : Color.white;
    }

    [System.Serializable]
    public class MaterialMapping
    {
        public int Center = 1;

        public int Top = 4;
        public int Bottom = 2;
        public int Left = 3;
        public int Right = 5;
    }
}