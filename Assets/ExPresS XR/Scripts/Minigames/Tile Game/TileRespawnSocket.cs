using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using ExPresSXR.Interaction.Interactors;
using ExPresSXR.Misc;

namespace ExPresSXR.Minigames.TileGame
{
    public class TileRespawnSocket : PutBackSocketInteractor
    {
        [SerializeField]
        private GameObject[] _respawnVariants;

        [SerializeField]
        [ReadonlyInInspector]
        private TileVisuals _currentVisuals;

        [SerializeField]
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

        [SerializeField]
        private Transform _scoreReferenceTransform;

        public bool HasRespawnVariants { get => _respawnVariants.Length > 0; }

        public UnityEvent OnRespawned;

        protected override void ResetPutBackTimer(SelectEnterEventArgs args)
        {
            if (args.interactorObject is TileSubmitSocket)
            {
                // Instantly spawn a new Putback Instance
                UnregisterPutBackInteractable();
                InstantiatePutBackPrefab();
                RegisterPutBackInteractable();
                OnRespawned.Invoke();
            }

            base.ResetPutBackTimer(args);
        }

        protected override void InstantiatePutBackPrefab()
        {
            if (HasRespawnVariants)
            {
                _putBackPrefab = RuntimeUtils.GetRandomArrayElement(_respawnVariants);
            }

            base.InstantiatePutBackPrefab();

            if (_putBackObjectInstance.TryGetComponent(out _currentVisuals))
            {
                _currentVisuals.Areas = _areas;
                _currentVisuals.DisplayOffsetReference = _scoreReferenceTransform;
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