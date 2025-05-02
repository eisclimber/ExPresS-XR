using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ExPresSXR.Presentation.Pictures
{
    public class PictureWallHoverer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Image that can be manipulated when hovering using the `SetImage...`-functions.
        /// </summary>
        [SerializeField]
        [Tooltip("Image that can be manipulated when hovering using the `SetImage...`-functions.")]
        private Image _hoverImage;

        /// <summary>
        /// Emit the OnHoverExit()-event initially to make sure the initial state is set.
        /// </summary>
        [SerializeField]
        [Tooltip("Emit the OnHoverExit()-event initially to make sure the initial state is set.")]
        private bool _emitHoverExitInitially = true;

        /// <summary>
        /// Emitted when a pointer enters the bounds of this UI element.
        /// </summary>
        public UnityEvent OnHoverEnter;

        /// <summary>
        /// Emitted when a pointer exits the bounds of this UI element.
        /// </summary>
        public UnityEvent OnHoverExit;


        private void Start()
        {
            if (_hoverImage == null && !TryGetComponent(out _hoverImage))
            {
                Debug.LogError("No Image found to detect hovers.");
            }

            if (_emitHoverExitInitially)
            {
                OnHoverExit.Invoke();
            }
        }

        /// <summary>
        /// Makes the hover image fully opaque.
        /// </summary>
        public void SetImageOpaque() => SetImageColorAlpha01(1.0f);


        /// <summary>
        /// Makes the hover image fully transparent.
        /// </summary>
        public void SetImageTransparent() => SetImageColorAlpha01(0.0f);


        /// <summary>
        /// Sets the alpha value of the hover image, clamped between 0.0f and 1.0f.
        /// </summary>
        /// <param name="alpha">Value between 0.0f and 1.0f.</param>
        public void SetImageColorAlpha01(float alpha)
        {
            if (_hoverImage != null)
            {
                Color oldColor = _hoverImage.color;
                _hoverImage.color = new Color(oldColor.r, oldColor.g, oldColor.b, Mathf.Clamp01(alpha));
            }
        }

        /// <summary>
        /// Detects Ui interactions to emit the OnHoverEnter-event.
        /// Automatically called when a pointer (Ray, Poke, Mouse, ...) enters this element.
        /// </summary>
        /// <param name="eventData">Context of the event.</param>
        public void OnPointerEnter(PointerEventData eventData) => OnHoverEnter.Invoke();


        /// <summary>
        /// Detects Ui interactions to emit the OnHoverExit-event.
        /// Automatically called when a pointer (Ray, Poke, Mouse, ...) exits this element.
        /// </summary>
        /// <param name="eventData">Context of the event.</param>
        public void OnPointerExit(PointerEventData eventData) => OnHoverExit.Invoke();
    }
}