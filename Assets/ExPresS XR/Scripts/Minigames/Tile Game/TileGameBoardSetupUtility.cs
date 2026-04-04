using ExPresSXR.Misc;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// A helper utility for creating the board of a tile game instantiating and placing the sockets.
    /// The setup functionality is provided via the context menu (three dots in the header of the component).
    /// </summary>
    public class TileGameBoardSetupUtility : MonoBehaviour
    {
        /// <summary>
        /// Spacing between sockets in local space.
        /// </summary>
        [SerializeField]
        [Tooltip("Spacing between sockets in local space.")]
        private float _spacing = 0.2f;

        /// <summary>
        /// Local height offset for the sockets.
        /// </summary>
        [SerializeField]
        [Tooltip("Height offset for the sockets in local space.")]
        private float _heightOffset = 0.03f;

        /// <summary>
        /// Reference to the TileGame to set the socket up for.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the TileGame to set the socket up for.")]
        private TileGame _game;

        /// <summary>
        /// Prefab used to create new board sockets from.
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab used to create new board sockets from.")]
        private GameObject _boardSocketPrefab;


        [ContextMenu("Setup Board")]
        private void SetupBoard()
        {
            if (_game == null && !TryGetComponent(out _game))
            {
                Debug.LogError("Can not setup up board. No reference to the board provided or found.");
                return;
            }

            Transform socketsParent = _game.BoardSocketsParent;
            if (socketsParent == null)
            {
                Debug.LogError("No board sockets parent transform provided. Can not set up sockets without it.");
                return;
            }

            int initialChildCount = socketsParent.childCount;
            int numTooMuch = initialChildCount - _game.NumBoardSlots;

            // Delete old tiles
            DeleteBoardTiles(numTooMuch, socketsParent);

            // Add and setup new ones
            SetupBoardTiles(initialChildCount, socketsParent);
        }

        private void DeleteBoardTiles(int numTooMuch, Transform parent)
        {
            for (int i = 0; i < numTooMuch; i++)
            {
                GameObject toBeDeleted = parent.GetChild(parent.childCount - 1).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(toBeDeleted);
                }
                else
                {
                    DestroyImmediate(toBeDeleted);
                }
            }
        }

        private void SetupBoardTiles(int initialChildCount, Transform parent)
        {
            for (int y = 0; y < TileGame.DEFAULT_BOARD_HEIGHT; y++)
            {
                for (int x = 0; x < TileGame.DEFAULT_BOARD_WIDTH; x++)
                {
                    SetupBoardTileAt(x, y, initialChildCount, parent);
                }
            }
        }

        private void SetupBoardTileAt(int x, int y, int initialChildCount, Transform parent)
        {
            Vector2Int size = _game.BoardSize;
            int childIdx = y * size.x + x;
            Vector3 localGridPos = new(
                (x - (size.x - 1) / 2.0f) * _spacing,
                _heightOffset,
                (((size.y - 1) / 2.0f) - y) * _spacing
            );

            if (childIdx < initialChildCount)
            {
                SetupExistingBoardTile(childIdx, localGridPos, new(x, y), parent);
            }
            else
            {
                SetupNewBoardTile(localGridPos, new(x, y), parent);
            }
        }

        private void SetupExistingBoardTile(int childIdx, Vector3 localGridPos, Vector2Int boardPos, Transform parent)
        {
            Transform child = parent.GetChild(childIdx);
            child.SetLocalPositionAndRotation(localGridPos, Quaternion.identity);
            if (child.TryGetComponent(out TileSubmitSocket socket))
            {
                SetupSocket(socket, boardPos);
            }
            else
            {
                Debug.LogWarning($"Failed set the existing GameObject {child.gameObject} idx {childIdx} since it had no TileSubmitSocket-Component.", this);
            }
        }

        private void SetupNewBoardTile(Vector3 localGridPos, Vector2Int boardPos, Transform parent)
        {
            GameObject childGo = Instantiate(_boardSocketPrefab, parent);
            childGo.transform.SetLocalPositionAndRotation(localGridPos, Quaternion.identity);
            if (childGo.TryGetComponent(out TileSubmitSocket socket))
            {
                SetupSocket(socket, boardPos);
            }
            else
            {
                Debug.LogWarning($"Failed set the new GameObject {childGo} since it had no TileSubmitSocket-Component.", this);
            }
        }

        private void SetupSocket(TileSubmitSocket socket, Vector2Int boardPos)
        {
            socket.BoardPos = boardPos;
            socket.gameObject.name = $"Board Socket {boardPos}";

#if UNITY_EDITOR
            int numEvents = socket.OnTileSubmitted.GetPersistentEventCount();
            if (numEvents < 1)
            {
                UnityEventTools.AddPersistentListener(socket.OnTileSubmitted, _game.AddTileFromBoardSubmission);
                // Force the editor to update
                EditorUtility.SetDirty(socket);
            }
            else
            {
                Debug.LogWarning($"Socket {socket.gameObject.name} already has persistent event listeners, "
                    + "not adding one for `AddTileFromBoardSubmission` to avoid duplication. "
                    + "Please ensure that the sockets `OnTileSubmitted` is connected to "
                    + "`AddTileFromBoardSubmission` of the TileGame.", socket);
            }
#endif
        }
    }
}