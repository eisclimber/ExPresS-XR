using UnityEngine;

namespace ExPResSXR.Presentation.Pictures
{
    public class PictureDataProvider : MonoBehaviour
    {
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
        public void SetDataDescription(string description)
        {
            if (data != null)
            {
                data.Description = description;
            }
        }
    }
}