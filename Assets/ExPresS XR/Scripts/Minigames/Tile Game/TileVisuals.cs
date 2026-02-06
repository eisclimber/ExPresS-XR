using System.Collections;
using UnityEngine;
using ExPresSXR.Minigames.Common;
using ExPresSXR.Misc;

namespace ExPresSXR.Minigames.TileGame
{
    public class TileVisuals : MonoBehaviour
    {
        private const int NUM_AREAS = 5;
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
        private float _subScoreShowDelay = 0.3f;

        [SerializeField]
        private float _pointsDisplayScale = 0.25f;

        [SerializeField]
        private float _pointsDisplayRadius = 0.09f;

        [Space]

        [SerializeField]
        private Transform _displayOffsetReference;
        public Transform DisplayOffsetReference
        {
            get => _displayOffsetReference;
            set => _displayOffsetReference = value;
        }

        [SerializeField]
        private Vector3 _pointsDisplayOffset = new(0.0f, 0.0f, -0.09f);

        [SerializeField]
        private MaterialMapping _materialIdxs;

        private Coroutine _displayScoreCoroutine;

        [ContextMenu("Update Visuals")]
        private void UpdateVisuals()
        {
            if (_displayedTile == null || _renderer == null || _areas == null || _areas.Length <= 0)
            {
                return;
            }

            Material[] newMats = Application.isPlaying ? _renderer.materials : _renderer.sharedMaterials;
            // Materials are messed up when exported for some reason...
            // Left, Top, Right, Bottom, Center
            newMats[_materialIdxs.Center] = _areas[_displayedTile.CenterAreaId].Material;
            newMats[_materialIdxs.Top] = _areas[_displayedTile.TopAreaId].Material;
            newMats[_materialIdxs.Bottom] = _areas[_displayedTile.BottomAreaId].Material;
            newMats[_materialIdxs.Left] = _areas[_displayedTile.LeftAreaId].Material;
            newMats[_materialIdxs.Right] = _areas[_displayedTile.RightAreaId].Material;
            _renderer.materials = newMats;
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
            // score.PrintScore();
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
            Vector3 displayOffset = _displayOffsetReference != null ? _displayOffsetReference.rotation * _pointsDisplayOffset : _pointsDisplayOffset;
            scoreNumbersGo.transform.SetPositionAndRotation(transform.position + direction * _pointsDisplayRadius + displayOffset, Quaternion.identity);
            scoreNumbersGo.transform.localScale = Vector3.one * _pointsDisplayScale;
            Color textColor = areaId >= 0 && areaId < _areas.Length ? _areas[areaId].Color : Color.white;

            if (scoreNumbersGo.TryGetComponent(out ScoreNumbers scoreNumbers))
            {
                scoreNumbers.ShowScore(points, textColor);
            }
        }

        public void RotateTileDataDegrees(float degrees) => _displayedTile.RotateDegrees(degrees);

        [ContextMenu("Rotate Tile Data")]
        public void RotateTileData() => _displayedTile?.Rotate(1);

        public void RotateTileData(int steps) => _displayedTile?.Rotate(steps);


        [ContextMenu("Display Test Score")]
        public void DisplayTestScore() => DisplayScore(new(1, 2, 10, 0, 4));


        [ContextMenu("Randomize Area Types")]
        public void RandomizeAreaTypes() =>  DisplayedTile = new(_areas.Length);

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

            // Color centerColor = _areasValid ? _areas[DisplayedTile.CenterAreaId].Color : Color.black;
            // Color topColor = _areasValid ? _areas[DisplayedTile.TopAreaId].Color : Color.black;
            // Color bottomColor = _areasValid ? _areas[DisplayedTile.BottomAreaId].Color : Color.black;
            // Color leftColor = _areasValid ? _areas[DisplayedTile.LeftAreaId].Color : Color.black;
            // Color rightColor = _areasValid ? _areas[DisplayedTile.RightAreaId].Color : Color.black;

            GizmoUtils.DrawLabel($"{DisplayedTile.CenterAreaId}", new Vector3(1.0f, 0.0f, 1.0f) * GIZMO_LABEL_OFFSET / 4.0f, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.TopAreaId}", Vector3.forward * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.BottomAreaId}", -Vector3.forward * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.LeftAreaId}", Vector3.left * GIZMO_LABEL_OFFSET, transform);
            GizmoUtils.DrawLabel($"{DisplayedTile.RightAreaId}", Vector3.right * GIZMO_LABEL_OFFSET, transform);
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
}