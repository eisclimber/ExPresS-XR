using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    public class ClothGameDebug : MonoBehaviour
    {
        [SerializeField]
        private ClothType _centerCloth;
        public ClothType CenterCloth
        {
            get => _centerCloth;
        }

        [Space]

        [SerializeField]
        private ClothType _topCloth;
        public ClothType TopCloth
        {
            get => _topCloth;
        }


        [SerializeField]
        private ClothType _bottomCloth;
        public ClothType BottomCloth
        {
            get => _bottomCloth;
        }

        [SerializeField]
        private ClothType _leftCloth;
        public ClothType LeftCloth
        {
            get => _leftCloth;
        }

        [SerializeField]
        private ClothType _rightCloth;
        public ClothType RightCloth
        {
            get => _rightCloth;
        }

        [Space]

        [SerializeField]
        private Vector2Int pos;


        [Space]

        [SerializeField]
        private ClothTileDisplay display;

        [SerializeField]
        private ClothGame game;


        private GameObject _tmp;


        void OnEnable()
        {
            _tmp = new("DEBUG Temp Cloth Tiles");
            _tmp.transform.SetParent(transform);
        }


        [ContextMenu("Add Tile From Display")]
        public void AddTileFromDisplay()
        {
            display.DisplayedTile = new(_centerCloth, _topCloth, _bottomCloth, _leftCloth, _rightCloth);
            game.AddClothTileFromDisplay(new(display, pos));
        }

        [ContextMenu("Add Tile")]
        public void AddTile()
        {
            ClothTile tile = new(_centerCloth, _topCloth, _bottomCloth, _leftCloth, _rightCloth);
            ClothGame.ScoreResults score = game.AddClothTileAt(tile, pos);
            score.PrintScore();
        }

        [ContextMenu("Evaluate From")]
        public void Evaluate()
        {
            bool[,] visited = new bool[ClothGame.FIELD_WIDTH, ClothGame.FIELD_HEIGHT];
            game.EvaluatePointsFrom(pos, 0, visited);
        }
    }
}