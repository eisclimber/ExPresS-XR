using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Misc;

namespace ExPresSXR.Minigames.TileGame
{
    /// <summary>
    /// A special putback socket that respawns a new tile if the old one gets submitted.
    /// New tiles are either one of the provided variants of a random one if none provided.
    /// </summary>
    public class TileRespawnSocket : PutBackSocketInteractor
    {
        /// <summary>
        /// TileVisuals interactable prefab to spawn new variants from.
        /// </summary>
        [SerializeField]
        [Tooltip("TileVisuals interactable prefabs to spawn new variants from.")]
        private GameObject[] _respawnVariants;

        /// <summary>
        /// Default TileVisuals interactable prefab to spawn and randomize if no variants are provided.
        /// </summary>
        [SerializeField]
        [Tooltip("Default TileVisuals interactable prefab to spawn and randomize if no variants are provided.")]
        [ReadonlyInInspector]
        private TileVisuals _currentVisuals;

        /// <summary>
        /// Areas used for tile randomization. Should be managed and set by the TileGame.
        /// </summary>
        [SerializeField]
        [Tooltip("Areas used for tile randomization. Should be managed and set by the TileGame.")]
        [ReadonlyInInspector]
        private AreaDescription[] _areas;
        public AreaDescription[] Areas
        {
            get => _areas;
            set
            {
                _areas = value;

                if (_currentVisuals != null)
                {
                    _currentVisuals.Areas = _areas;
                }
            }
        }

        /// <summary>
        /// If new tiles are spawned from variants of the default TileVisuals.
        /// </summary>
        public bool HasRespawnVariants { get => _respawnVariants.Length > 0; }

        /// <summary>
        /// Emitted when a tile is respawned.
        /// </summary>
        public UnityEvent OnRespawned;

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected override void ResetPutBackTimer(SelectEnterEventArgs args)
        {
            if (args != null && args.interactorObject is TileSubmitSocket)
            {
                // Instantly spawn a new Putback Instance
                UnregisterPutBackInteractable();
                InstantiatePutBackPrefab();
                RegisterPutBackInteractable();
                OnRespawned.Invoke();
            }

            base.ResetPutBackTimer(args);
        }

        /// <summary>
        /// < inheritdoc />
        /// </summary>
        protected override void InstantiatePutBackPrefab()
        {
            if (HasRespawnVariants)
            {
                _putBackPrefab = RuntimeUtils.GetRandomArrayElement(_respawnVariants);
            }

            base.InstantiatePutBackPrefab();

            if (_putBackObjectInstance == null)
            {
                return;
            }
            else if (_putBackObjectInstance.TryGetComponent(out _currentVisuals))
            {
                _currentVisuals.Areas = _areas;
                if (!HasRespawnVariants)
                {
                    // Only randomize prefab if there were no variants
                    _currentVisuals.RandomizeAreaTypes();
                }
            }
            else
            {
                Debug.LogError("Failed to get a TileVisuals from the new PutbackInstance. Please check your Prefab.", this);
            }
        }
    }
}