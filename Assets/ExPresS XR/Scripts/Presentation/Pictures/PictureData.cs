using System;
using UnityEngine;

namespace ExPresSXR.Presentation.Pictures
{
    [Serializable]
    public class PictureData : ScriptableObject
    {
        /// <summary>
        /// The title of the picture collection.
        /// </summary>
        [Tooltip("The title of the picture collection.")]
        public string Title = "";

        /// <summary>
        /// The description of the picture collection. Shown as Info Text.
        /// </summary>
        [Tooltip("The description of the picture collection. Shown as Info Text.")]
        public string[] Descriptions;

        /// <summary>
        /// List of viewable images in the picture collection, ordered from first to last.
        /// </summary>
        [Tooltip("List of viewable images in the picture collection, ordered from first to last.")]
        public Sprite[] Pictures;

        /// <summary>
        /// Fall back picture if one is null/missing/invalid...
        /// </summary>
        [Tooltip(" Fall back picture if one is null/missing/invalid...")]
        public Sprite FallbackPicture;

        /// <summary>
        /// Number of pictures in this asset.
        /// </summary>
        public int NumPictures
        {
            get => Pictures.Length;
        }
    }
}