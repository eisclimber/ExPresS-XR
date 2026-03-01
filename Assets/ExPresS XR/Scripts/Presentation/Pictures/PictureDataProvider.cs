using UnityEngine;
using UnityEngine.Localization.Components;

namespace ExPresSXR.Presentation.Pictures
{
    /// <summary>
    /// Makes a PictureData asset accessible to other components.
    /// </summary>
    public class PictureDataProvider : MonoBehaviour
    {
        /// <summary>
        /// Separator used when joining/splitting multiple descriptions into/from a single string.
        /// </summary>
        const string DESCRIPTION_SEPARATOR = "----";


        /// <summary>
        /// Picture data to be provided.
        /// </summary>
        [Tooltip("Picture data to be provided.")]
        [SerializeField]
        private PictureData _data;
        public PictureData Data
        {
            get => _data;
            set => _data = value;
        }

        /// <summary>
        /// Allows to set the PictureData's title to be localized.
        /// </summary>
        /// <param name="title">Title to be set.</param>
        public void SetDataTitle(string title)
        {
            if (_data != null)
            {
                _data.Title = title;
            }
        }

        /// <summary>
        /// Allows to set the PictureData's description to be localized.
        /// </summary>
        /// <param name="description">Description to be set.</param>
        /// <param name="idx">Index to the description.</param>
        public void SetDataDescription(string description, int idx)
        {
            if (_data != null && idx >= 0 && idx < _data.Descriptions.Length)
            {
                _data.Descriptions[idx] = description;
            }
        }

        /// <summary>
        /// Allows setting the description from a single value, separating descriptions by the value of `DESCRIPTION_SEPARATOR` to allow for more manageable localization.
        /// </summary>
        /// <param name="description">Description to parse and set.</param>
        public void SetDataDescriptionJoined(string description)
        {
            string[] descriptions = description.Split(DESCRIPTION_SEPARATOR);

            if (descriptions.Length < _data.Descriptions.Length)
            {
                Debug.LogWarning("Localizing picture data descriptions but too few were provided. Padding missing ones with an empty string.", this);
            }
            else if (descriptions.Length > _data.Descriptions.Length)
            {
                Debug.LogWarning("Localizing picture data descriptions but too may were provided. Ignoring them.", this);
            }

            for (int i = 0; i < _data.Descriptions.Length; i++)
            {
                // Make sure to remove surrounding whitespace/Linebreaks from the formatting
                _data.Descriptions[i] = (i < descriptions.Length ? descriptions[i] : "").Trim();
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