using ExPresSXR.Interaction;
using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace ExPresSXR.Minigames.TileGame
{
    public class ClothTileRespawnSocket : PutBackSocketInteractor
    {
        [SerializeField]
        private GameObject[] _respawnVariants;

        [SerializeField]
        private Transform _scoreReferenceTransform;

        public bool HasRespawnVariants
        {
            get => _respawnVariants.Length > 0;
        }

        public UnityEvent OnRespawned;

        protected override void ResetPutBackTimer(SelectEnterEventArgs args)
        {
            if (args.interactorObject is ClothTileSubmitSocket)
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

            // Only randomize prefab if it is random
            if (!HasRespawnVariants)
            {
                if (_putBackObjectInstance.TryGetComponent(out ClothTileDisplay display))
                {
                    display.RandomizeClothTypes();
                    display.DisplayOffsetReference = _scoreReferenceTransform;
                }
                else
                {
                    Debug.LogError("Failed to get a ClothTileDisplay from the new PutbackInstance. Please check your Prefab.", this);
                }
            }
        }
    }
}