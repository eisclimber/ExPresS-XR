using UnityEngine;

namespace ExPresSXR.Tutorial
{
    /// <summary>
    /// Interface to handle a step in a tutorial.
    /// </summary>
    public interface ITutorialStepHandler
    {
        /// <summary>
        /// Used to display parts of a tutorial depending on the current tutorial step.
        /// </summary>
        /// <param name="stepIdx">Current step of the tutorial.</param>
        public void HandleTutorialStep(int stepIdx);
    }

    /// <summary>
    /// Class used for serialization.
    /// </summary>
    public abstract class TutorialStepHandler : MonoBehaviour, ITutorialStepHandler
    {
        /// <inheritdoc />
        public abstract void HandleTutorialStep(int stepIdx);
    }
}