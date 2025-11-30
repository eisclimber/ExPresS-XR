using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Interaction
{
    public class HighlightableSocketInteractor : XRSocketInteractor
    {

        [SerializeField]
        protected bool _showHighlighter;
        public bool ShowHighlighter
        {
            get => _showHighlighter;
            set
            {
                _showHighlighter = value;

                SetHighlighterVisible(ShowHighlighter && startingSelectedInteractable == null);
            }
        }

        [SerializeField]
        protected GameObject _highlighterObject;
        public GameObject HighlighterObject
        {
            get => _highlighterObject;
            set
            {
                _highlighterObject = value;

                if (_highlighterObject != null)
                {
                    _highlighterObject.GetComponent<MeshRenderer>().enabled = _showHighlighter;
                    _highlighterObject.transform.localScale = _highlighterScale;
                }
            }
        }

        [SerializeField]
        protected bool _useColliderSizeAsScale;
        public bool UseColliderSizeAsScale
        {
            get => _useColliderSizeAsScale;
            set
            {
                _useColliderSizeAsScale = value && CanSetHighlighterScaleWithCollider();

                UpdateHighlighterScaleWithCollider();
            }
        }

        [Tooltip("The scale of the highlighterObject. Be sure to make it a little bit smaller (0.01f) to prevent z-fighting due to material overlapping.")]
        [SerializeField]
        protected Vector3 _highlighterScale = Vector3.one * 0.1f;
        public Vector3 HighlighterScale
        {
            get => _highlighterScale;
            set
            {
                _highlighterScale = value;

                if (_highlighterObject != null)
                {
                    _highlighterObject.transform.localScale = _highlighterScale;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SetHighlighterVisible(ShowHighlighter && startingSelectedInteractable == null);

            selectEntered.AddListener(HideHighlighterFromSelect);
            selectExited.AddListener(ShowHighlighterFromSelect);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            selectEntered.RemoveListener(HideHighlighterFromSelect);
            selectExited.RemoveListener(ShowHighlighterFromSelect);
        }


        protected void UpdateHighlighterScaleWithCollider()
        {
            if (!_useColliderSizeAsScale)
            {
                return;
            }

            if (TryGetComponent(out SphereCollider sphereCollider))
            {
                HighlighterScale = Vector3.one * (2 * sphereCollider.radius);
            }
            else if (TryGetComponent(out BoxCollider boxCollider))
            {
                HighlighterScale = boxCollider.size;
            }
            else if (TryGetComponent(out CapsuleCollider capsuleCollider))
            {
                HighlighterScale = new(capsuleCollider.radius, capsuleCollider.height, capsuleCollider.radius);
            }
            else
            {
                Debug.LogWarning("Did not find a SphereCollider nor a BoxCollider. Setting scale to (1, 1, 1).");
                HighlighterScale = Vector3.one;
            }
        }


        public bool CanSetHighlighterScaleWithCollider()
        {
            return TryGetComponent(out SphereCollider _) || TryGetComponent(out BoxCollider _) || TryGetComponent(out CapsuleCollider _);
        }


        public virtual void SetHighlighterVisible(bool visible)
        {
            if (_highlighterObject != null && _highlighterObject.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.enabled = ShowHighlighter && visible;
            }
        }


        protected void ShowHighlighterFromSelect(SelectExitEventArgs args) => SetHighlighterVisible(true);

        protected void HideHighlighterFromSelect(SelectEnterEventArgs args) => SetHighlighterVisible(false);


        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateHighlighterScaleWithCollider();
        }
    }
}