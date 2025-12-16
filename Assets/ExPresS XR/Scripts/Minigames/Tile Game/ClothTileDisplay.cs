using System.Collections;
using ExPresSXR.Minigames.Common;
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    public class ClothTileDisplay : MonoBehaviour
    {
        [SerializeField]
        private ClothTile _displayedTile = null;
        public ClothTile DisplayedTile
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

        [Space]

        [SerializeField]
        private Material _lightBurlapMaterial;

        [SerializeField]
        private Material _darkBurlapMaterial;

        [SerializeField]
        private Material _leatherMaterial;

        private Coroutine _displayScoreCoroutine;


        private void UpdateVisuals()
        {
            if (_displayedTile == null || _renderer == null)
            {
                return;
            }

            Material[] newMats = new Material[5];
            // Materials are messed up when exported for some reason...
            // Left, Top, Right, Bottom, Center
            newMats[4] = ClothTypeToMaterial(_displayedTile.CenterCloth);
            newMats[1] = ClothTypeToMaterial(_displayedTile.TopCloth);
            newMats[3] = ClothTypeToMaterial(_displayedTile.BottomCloth);
            newMats[0] = ClothTypeToMaterial(_displayedTile.LeftCloth);
            newMats[2] = ClothTypeToMaterial(_displayedTile.RightCloth);
            _renderer.materials = newMats;
        }

        public void DisplayScore(ClothGame.ScoreResults score)
        {
            // Maybe order the scores...
            if (_displayScoreCoroutine != null)
            {
                StopCoroutine(_displayScoreCoroutine);
            }
            // score.PrintScore();
            _displayScoreCoroutine = StartCoroutine(ShowScoresSequential(score));
        }

        private IEnumerator ShowScoresSequential(ClothGame.ScoreResults score)
        {
            // score.PrintScore();
            if (score.CenterScore > 0)
            {
                SpawnPointsDisplay(score.CenterScore, score.CenterColor, Vector3.zero);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.TopScore > 0)
            {
                SpawnPointsDisplay(score.TopScore, score.TopColor, Vector3.forward);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.BottomScore > 0)
            {
                SpawnPointsDisplay(score.BottomScore, score.BottomColor, Vector3.back);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.LeftScore > 0)
            {
                SpawnPointsDisplay(score.LeftScore, score.LeftColor, Vector3.left);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }

            if (score.RightScore > 0)
            {
                SpawnPointsDisplay(score.RightScore, score.RightColor, Vector3.right);
                yield return new WaitForSeconds(_subScoreShowDelay);
            }
        }

        private void SpawnPointsDisplay(int points, Color textColor, Vector3 direction)
        {
            GameObject scoreNumbersGo = Instantiate(_pointsDisplayPrefab);
            // Use offset of the reference or global offset
            Vector3 displayOffset = _displayOffsetReference != null ? _displayOffsetReference.rotation * _pointsDisplayOffset : _pointsDisplayOffset;
            scoreNumbersGo.transform.SetPositionAndRotation(transform.position + direction * _pointsDisplayRadius + displayOffset, Quaternion.identity);
            scoreNumbersGo.transform.localScale = Vector3.one * _pointsDisplayScale;

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


        [ContextMenu("Randomize Material")]
        public void RandomizeClothTypes()
        {
            DisplayedTile = new();
        }


        private Material ClothTypeToMaterial(ClothType clothType)
        {
            return clothType switch
            {
                ClothType.LightBurlap => _lightBurlapMaterial,
                ClothType.DarkBurlap => _darkBurlapMaterial,
                ClothType.Leather => _leatherMaterial,
                _ => null
            };
        }
    }
}