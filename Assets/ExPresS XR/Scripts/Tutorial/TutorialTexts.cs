using TMPro;
using UnityEngine;

namespace ExPresSXR.Tutorial
{
    public class TutorialTexts : TutorialStepHandler
    {
        /// <summary>
        /// Text strings displayed per tutorial step.
        /// </summary>
        [SerializeField]
        [TextArea]
        [Tooltip("Texts displayed per tutorial step.")]
        private string[] _texts;

        [Space]

        /// <summary>
        /// Text to display the strings.
        /// </summary>
        [SerializeField]
        [Tooltip("Text to display the strings.")]
        private TMP_Text _text;

        /// <inheritdoc />
        private void OnEnable()
        {
            if (_text == null && !TryGetComponent(out _text))
            {
                Debug.LogError("Did not find a text component to display tutorial text.", this);
            }
        }

        /// <summary>
        /// Changes the contents of a text component per step. 
        /// </summary>
        /// <param name="stepIdx">Current step of the tutorial.</param>
        public override void HandleTutorialStep(int stepIdx)
        {
            if (stepIdx >= 0 && stepIdx < _texts.Length && _texts[stepIdx] != "")
            {
                _text.text = _texts[stepIdx];
            }
        }
    }
}