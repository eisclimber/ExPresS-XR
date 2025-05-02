using UnityEngine;
using UnityEngine.Localization.Components;

namespace ExPresSXR.Presentation.Pictures
{
    public class PictureDataProvider : MonoBehaviour
    {
        const string DESCRIPTION_SEPARATOR = "----";


        /// <summary>
        /// Picture data to be provided.
        /// </summary>
        [Tooltip("Picture data to be provided.")]
        public PictureData data;

        /// <summary>
        /// Allows to set the PictureData's title to be localized.
        /// </summary>
        /// <param name="title">Title to be set.</param>
        public void SetDataTitle(string title)
        {
            if (data != null)
            {
                data.Title = title;
            }
        }

        /// <summary>
        /// Allows to set the PictureData's description to be localized.
        /// </summary>
        /// <param name="description">Description to be set.</param>
        public void SetDataDescription(string description, int idx)
        {
            if (data != null && idx >= 0 && idx < data.Descriptions.Length)
            {
                data.Descriptions[idx] = description;
            }
        }

        public void SetDataDescriptionJoined(string description)
        {
            string[] descriptions = description.Split(DESCRIPTION_SEPARATOR);

            if (descriptions.Length < data.Descriptions.Length)
            {
                Debug.LogWarning("Localizing picture data descriptions but too few were provided. Padding missing ones with an empty string.", this);
            }
            else if (descriptions.Length > data.Descriptions.Length)
            {
                Debug.LogWarning("Localizing picture data descriptions but too may were provided. Ignoring them.", this);
            }

            for (int i = 0; i < data.Descriptions.Length; i++)
            {
                // Make sure to remove surrounding whitespace/Linebreaks from the formatting
                data.Descriptions[i] = (i < descriptions.Length ? descriptions[i] : "").Trim();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Localize")]
        private void AddAndSetupLocalizeStringEvent()
        {
            // Check if LocalizeStringEvent component exists, add if it doesn't
            LocalizeStringEvent[] localizations = gameObject.GetComponents<LocalizeStringEvent>();
            LocalizeStringEvent titleLocalization = localizations.Length > 0 ? localizations[0] : gameObject.AddComponent<LocalizeStringEvent>();
            LocalizeStringEvent textLocalization = localizations.Length > 1 ? localizations[1] : gameObject.AddComponent<LocalizeStringEvent>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(titleLocalization.OnUpdateString, SetDataTitle);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(textLocalization.OnUpdateString, SetDataDescriptionJoined);
        }
#endif
    }
}