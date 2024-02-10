using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ExPresSXR.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class AutoScrollRect : MonoBehaviour
    {
        /// <summary>
        /// If enabled, the rect will scroll automatically.
        /// </summary>
        [SerializeField]
        [Tooltip("If enabled, the rect will scroll automatically.")]
        private bool _autoScrolling;
        public bool autoScrolling
        {
            get => _autoScrolling;
            set
            {
                bool changed = _autoScrolling == value;
                _autoScrolling = value;

                if (changed && autoScrolling)
                {
                    OnScrollStarted.Invoke(_scrollDirection);
                }
            }
        }

        /// <summary>
        /// Direction in which the Rect moves.
        /// </summary>
        [SerializeField]
        [Tooltip("Direction in which the Rect moves.")]
        private ScrollDirection _scrollDirection = ScrollDirection.Down;

        /// <summary>
        /// Duration in seconds to scroll from the top to the bottom.
        /// </summary>
        [SerializeField]
        [Tooltip("Duration in seconds to scroll from the top to the bottom.")]
        private float _scrollDuration = 15.0f;

        /// <summary>
        /// Time in seconds until scrolling starts after awake. 
        /// Values below zero won't start scrolling. Zero starts scrolling instantaneously.
        /// </summary>
        [SerializeField]
        [Tooltip("Time in seconds until scrolling starts after awake. Values below zero won't start scrolling. Zero starts scrolling instantaneously.")]
        private float _initialWaitTime = -1.0f;

        [Space]

        /// <summary>
        /// Reference to the scroll rect to be controlled.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the scroll rect to be controlled.")]
        private ScrollRect _scrollRect;

        /// <summary>
        /// Event emitted when a scroll is started or the direction is changed.
        /// </summary>
        public UnityEvent<ScrollDirection> OnScrollStarted;
        /// <summary>
        /// Emitted when the scroll stops at the top.
        /// </summary>
        public UnityEvent OnScrollUpCompleted;
        /// <summary>
        /// Emitted when the scroll stops at the bottom.
        /// </summary>
        public UnityEvent OnScrollDownCompleted;


        private void Start()
        {
            if (_scrollRect == null && !TryGetComponent(out _scrollRect))
            {
                Debug.LogError("ScrollRect was not found.", this);
            }
        }


        private void Update()
        {
            if (_autoScrolling)
            {
                _scrollRect.verticalNormalizedPosition += (int)_scrollDirection * Time.deltaTime * (1.0f / _scrollDuration);

                if (_scrollRect.verticalNormalizedPosition <= 0.0f && _scrollDirection == ScrollDirection.Down)
                {
                    _autoScrolling = false;
                    OnScrollDownCompleted.Invoke();
                }
                else if (_scrollRect.verticalNormalizedPosition >= 1.0f && _scrollDirection == ScrollDirection.Up)
                {
                    _autoScrolling = false;
                    OnScrollUpCompleted.Invoke();
                }
            }
        }


        private void OnEnable()
        {
            ScrollToTop();

            if (_initialWaitTime > 0.0f)
            {
                StartCoroutine(InitialWaitTimer());
            }
            else if (_initialWaitTime == 0.0f)
            {
                _autoScrolling = true;
            }
        }


        private void OnDisable()
        {
            _autoScrolling = false;
        }


        /// <summary>
        /// Starts scrolling up.
        /// </summary>
        [ContextMenu("Scroll Up")]
        public void ScrollUp()
        {
            _scrollDirection = ScrollDirection.Up;
            _autoScrolling = true;
            OnScrollStarted.Invoke(_scrollDirection);
        }


        /// <summary>
        /// Starts scrolling up.
        /// </summary>
        [ContextMenu("Scroll Down")]
        public void ScrollDown()
        {
            _scrollDirection = ScrollDirection.Down;
            _autoScrolling = true;
            OnScrollStarted.Invoke(_scrollDirection);
        }

        /// <summary>
        /// Scrolls to the top instantly.
        /// </summary>
        /// <param name="keepScrolling">If false will disable '_autoScrolling', if true will not change the value of '_autoScrolling'. </param>
        [ContextMenu("Scroll To Top")]
        public void ScrollToTop(bool keepScrolling = false)
        {
            _scrollRect.normalizedPosition = new Vector2(0.0f, 1.0f);
            if (!keepScrolling)
            {
                _autoScrolling = false;
            }
        }

        /// <summary>
        /// Scrolls to the bottom instantly.
        /// </summary>
        /// <param name="keepScrolling">If false will disable '_autoScrolling', if true will not change the value of '_autoScrolling'. </param>
        [ContextMenu("Scroll To Bottom")]
        public void ScrollToBottom(bool keepScrolling = false)
        {
            _scrollRect.normalizedPosition = new Vector2(0.0f, 0.0f);
            if (!keepScrolling)
            {
                _autoScrolling = false;
            }
        }


        private IEnumerator InitialWaitTimer()
        {
            yield return new WaitForSeconds(_initialWaitTime);
            _autoScrolling = true;
        }

        public enum ScrollDirection
        {
            Up = 1,
            Down = -1
        }
    }
}