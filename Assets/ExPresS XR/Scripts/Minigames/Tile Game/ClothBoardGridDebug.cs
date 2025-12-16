using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    public class ClothBoardGridDebug : MonoBehaviour
    {
        [SerializeField]
        private float _spacing = 0.2f;

        [SerializeField]
        private float _zOffset = -0.03f;

        [SerializeField]
        private GameObject _socketPrefab;

        [SerializeField]
        private ClothGame game;


        [ContextMenu("Setup Board")]
        private void SetupBoard()
        {
            int initialChildCount = transform.childCount;
            for (int y = 0; y < ClothGame.FIELD_HEIGHT; y++)
            {
                for (int x = 0; x < ClothGame.FIELD_WIDTH; x++)
                {
                    SetupBoardTile(x, y, initialChildCount);
                }
            }
            // Don't care for deleting the board rn.. 
        }

        private void SetupBoardTile(int x, int y, int initialChildCount)
        {
            int childIdx = y * ClothGame.FIELD_WIDTH + x;
            Vector3 localGridPos = new(
                (x - (ClothGame.FIELD_WIDTH - 1) / 2.0f) * _spacing,
                (((ClothGame.FIELD_HEIGHT - 1) / 2.0f) - y) * _spacing,
                _zOffset
            );

            if (childIdx < initialChildCount)
            {
                SetupExistingBoardTile(childIdx, localGridPos, new(x, y));
            }
            else
            {
                SetupNewBoardTile(localGridPos, new(x, y));
            }
        }

        private void SetupExistingBoardTile(int childIdx, Vector3 localGridPos, Vector2Int boardPos)
        {
            Transform child = transform.GetChild(childIdx);
            child.SetLocalPositionAndRotation(localGridPos, Quaternion.identity);
            if (child.TryGetComponent(out ClothTileSubmitSocket socket))
            {
                SetupSocket(socket, boardPos);
            }
            else
            {
                Debug.LogWarning($"Failed set the existing GameObject {child.gameObject} idx {childIdx} since it had no ClothTileSubmitSocket-Component.", this);
            }
        }

        private void SetupNewBoardTile(Vector3 localGridPos, Vector2Int boardPos)
        {
            GameObject childGo = Instantiate(_socketPrefab, transform);
            childGo.transform.SetLocalPositionAndRotation(localGridPos, Quaternion.identity);
            if (childGo.TryGetComponent(out ClothTileSubmitSocket socket))
            {
                SetupSocket(socket, boardPos);
            }
            else
            {
                Debug.LogWarning($"Failed set the new GameObject {childGo} since it had no ClothTileSubmitSocket-Component.", this);
            }
        }

        private void SetupSocket(ClothTileSubmitSocket socket, Vector2Int boardPos)
        {
            socket.BoardPos = boardPos;
            socket.gameObject.name = $"Board Socket {boardPos}";
        }
    }
}