using ExPresSXR.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace ExPresSXR.Minigames.Archery
{
    public class RandomImage : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] _images;

        [SerializeField]
        [Tooltip("Give probabilities if you want to use the weightedRandom -> Same amount of probabilities as elements, they have to sum up to 1, need to be ordered descending, images have to be ordered regarding their probabilities!")]
        public float[] _probabilities;

        [SerializeField]
        [Tooltip("Reference to the Image on the Canvas")]
        private Image _displayedImage;

        [SerializeField]
        [Tooltip("Select images based on weights ")]
        private bool _useWeightedRandom;

        //Put random image on Canvas at start
        void Start()
        {
            if (_images == null || _images.Length <= 0)
            {
                Debug.LogError("No images to display provided!", this);
            }

            _displayedImage.sprite = _useWeightedRandom ? RuntimeUtils.GetRandomArrayElementWeighted(_images, _probabilities) : RuntimeUtils.GetRandomArrayElementUnweighted(_images);
        }

        // Change images to the one given in the array
        public void ChangeImages(Sprite[] newImages, float[] newProbabilities)
        {
            _images = newImages;
            _probabilities = newProbabilities;
        }
    }
}
