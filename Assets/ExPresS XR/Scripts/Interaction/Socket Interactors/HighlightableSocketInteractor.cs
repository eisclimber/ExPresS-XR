using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ExPresSXR.Interaction.Interactors
{
    /// <summary>
    /// A SocketInteractor can shows a visual queue (`highlighterObject`) where the interactable area is of nothing is in the socket.
    /// 
    /// The visual queue can be scaled automatically if `useColliderSizeAsScale` is set to true and it **this** `SocketInteractor` 
    /// has a `SphereCollider`- or `BoxCollider`-Component.
    /// If not the size can be set by modifying `highlighterScale`.
    /// 
    /// Make sure the `highlighterObject` does **not** have a Collider, only a `Mesh` and `MeshRenderer`.
    /// </summary>
    public class HighlightableSocketInteractor : XRSocketInteractor
    {
        [SerializeField]
        [Tooltip("If the highlighter object should be shown when no object is in the socket.")]
        protected bool _showHighlighter;
        /// <summary>
        /// If the highlighter object should be shown when no object is in the socket.
        /// </summary>
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
        [Tooltip("The highlighter shown when no object is in the socket.")]
        protected GameObject _highlighterObject;
        /// <summary>
        /// The highlighter shown when no object is in the socket.
        /// </summary>
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
        [Tooltip("Controls the scale of the provided highlighter by the scale of the sockets collider.")]
        protected bool _useColliderSizeAsScale;
        /// <summary>
        /// Controls the scale of the provided highlighter by the scale of the sockets collider.
        /// </summary>
        public bool UseColliderSizeAsScale
        {
            get => _useColliderSizeAsScale;
            set
            {
                _useColliderSizeAsScale = value && CanSetHighlighterScaleWithCollider();

                UpdateHighlighterScaleWithCollider();
            }
        }

        [SerializeField]
        [Tooltip("The scale of the highlighterObject.")]
        protected Vector3 _highlighterScale = Vector3.one * 0.1f;
        /// <summary>
        /// The scale of the highlighterObject.
        /// </summary>
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

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            SetHighlighterVisible(ShowHighlighter && startingSelectedInteractable == null);

            selectEntered.AddListener(HideHighlighterFromSelect);
            selectExited.AddListener(ShowHighlighterFromSelect);
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();

            selectEntered.RemoveListener(HideHighlighterFromSelect);
            selectExited.RemoveListener(ShowHighlighterFromSelect);
        }


        /// <summary>
        /// Maps the size of a collider component of this GameObject to `HighlighterScale`.
        /// </summary>
        protected void UpdateHighlighterScaleWithCollider()
        {
            if (!_useColliderSizeAsScale)
            {
                return;
            }

            if (TryGetComponent(out SphereCollider sphereCollider))
            {
                HighlighterScale = Vector3.one * 2 * sphereCollider.radius;
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


        /// <summary>
        /// Sets the visibility of the highlighter.
        /// </summary>
        /// <param name="visible">Set to visible or not.</param>
        public virtual void SetHighlighterVisible(bool visible)
        {
            if (_highlighterObject != null && _highlighterObject.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.enabled = ShowHighlighter && visible;
            }
        }

        /// <summary>
        /// Select enter callback showing the highlighter. 
        /// </summary>
        /// <param name="args">Ignored.</param>
        protected void ShowHighlighterFromSelect(SelectExitEventArgs _) => SetHighlighterVisible(true);

        /// <summary>
        /// Select exit callback hiding the highlighter. 
        /// </summary>
        /// <param name="args">Ignored.</param>
        protected void HideHighlighterFromSelect(SelectEnterEventArgs _) => SetHighlighterVisible(false);


        /// <summary>
        /// If this component has either a Sphere-, Box- or CapsuleCollider and this `HighlighterScale` can be set automatically.
        /// </summary>
        /// <returns>If `HighlighterScale` can be derived from a collider.</returns>
        public bool CanSetHighlighterScaleWithCollider()
        {
            return TryGetComponent(out SphereCollider _) || TryGetComponent(out BoxCollider _) || TryGetComponent(out CapsuleCollider _);
        }

        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateHighlighterScaleWithCollider();
        }
    }
}