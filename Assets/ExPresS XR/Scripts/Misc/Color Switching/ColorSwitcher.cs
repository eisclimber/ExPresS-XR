using System.Collections;
using UnityEngine;


namespace ExPresSXR.Misc.ColorSwitching
{
    /// <summary>
    /// Provides the option to switch materials of a renderer.
    /// 
    /// It is highly recommended to use a ColorAnimationSwitcher for more complex scenarios.
    /// </summary>
   public class ColorSwitcher : MonoBehaviour
    {
        /// <summary>
        /// The material that is replacing the material applied to the GameObject via Editor.
        /// </summary>
        [Tooltip("The material that is replacing the material applied to the GameObject via Editor.")]
        public Material alternativeMaterial;

        /// <summary>
        /// The duration the material is switched when calling the '...ForSwitchDuration' functions.
        /// </summary>
        [Tooltip("The duration the material is switched when calling the '...ForSwitchDuration' functions.")]
        public float switchDuration = 1.0f;

        /// <summary>
        /// When changing to the Original Material, the object's material must be the Alternative Material.
        /// </summary>
        [Tooltip("When changing to the Original Material, the object's material must be the Alternative Material.")]
        public bool requireOriginalMaterialMatch;

        /// <summary>
        /// When changing to the Alternative Material, the object's material must be the Original Material.
        /// </summary>
        [Tooltip("When changing to the Alternative Material, the object's material must be the Original Material.")]
        public bool requireAlternativeMaterialMatch;

        /// <summary>
        /// The MeshRenderer whose material will be manipulated.
        /// </summary>
        [SerializeField]
        [Tooltip("The MeshRenderer whose material will be manipulated.")]
        private MeshRenderer _meshRenderer;

        private Material _originalMaterial;


        private void Awake()
        {
            if (_meshRenderer == null && !TryGetComponent(out _meshRenderer))
            {
                Debug.LogError("ColorSwitcher requires a MeshRenderer component to function properly.");
                return;
            }

            _originalMaterial = _meshRenderer.material;

            if (alternativeMaterial == null)
            {
                Debug.LogWarning("No Material assigned to Color Switcher. Materials won't switch.");
            }
        }

        #region Instant Switches
        /// <summary>
        /// Activates the alternative material instantaneous.
        /// </summary>
        public void ActivateAlternativeMaterial()
        {
            StopAllCoroutines();
            SetAlternativeMaterialActive();
        }

        /// <summary>
        /// Activates the original material instantaneous.
        /// </summary>
        public void ActivateOriginalMaterial()
        {
            StopAllCoroutines();
            SetOriginalMaterialActive();
        }

        /// <summary>
        /// Toggles the material instantaneous.
        /// </summary>
        public void ToggleMaterial()
        {
            StopAllCoroutines();
            SetMaterialToggled();
        }
        #endregion

        #region One Second Switches
        /// <summary>
        /// Activates the alternative material for a second switching back to the original.
        /// </summary>
        public void ActivateAlternativeMaterialForASecond()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateAlternativeMaterialForSecondsCoroutine(1f));
        }

        /// <summary>
        /// Activates the original material for a second switching back to the alternative.
        /// </summary>

        public void ActivateOriginalMaterialForASecond()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateOriginalMaterialForSecondsCoroutine(1f));
        }


        /// <summary>
        /// Toggles the material for a second and toggles back afterwards.
        /// </summary>

        public void ToggleMaterialForASecond()
        {
            StopAllCoroutines();
            StartCoroutine(ToggleMaterialForSecondsCoroutine(1f));
        }
        #endregion

        #region Using switchDuration
        /// <summary>
        /// Activates the alternative material for a duration switching back to the original.
        /// </summary>

        public void ActivateAlternativeMaterialForSwitchDuration()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateAlternativeMaterialForSecondsCoroutine(switchDuration));
        }

        /// <summary>
        /// Activates the original material for a duration switching back to the alternative.
        /// </summary>
        public void ActivateOriginalMaterialForSwitchDuration()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateOriginalMaterialForSecondsCoroutine(switchDuration));
        }

        /// <summary>
        /// Toggles the material for a duration and toggles back afterwards.
        /// </summary>
        public void ToggleMaterialForSwitchDuration()
        {
            StopAllCoroutines();
            StartCoroutine(ToggleMaterialForSecondsCoroutine(switchDuration));
        }
        #endregion

        #region Coroutines
        private IEnumerator ActivateAlternativeMaterialForSecondsCoroutine(float time)
        {
            SetAlternativeMaterialActive();
            yield return new WaitForSeconds(time);
            SetOriginalMaterialActive();
        }

        private IEnumerator ActivateOriginalMaterialForSecondsCoroutine(float time)
        {
            SetOriginalMaterialActive();
            yield return new WaitForSeconds(time);
            SetAlternativeMaterialActive();
        }

        private IEnumerator ToggleMaterialForSecondsCoroutine(float time)
        {
            SetMaterialToggled();
            yield return new WaitForSeconds(time);
            SetMaterialToggled();
        }

        // Private methods for setting materials to allow proper coroutine handling
        private void SetAlternativeMaterialActive()
        {
            if (_meshRenderer != null && alternativeMaterial != null
                && (!requireAlternativeMaterialMatch || _meshRenderer.material == _originalMaterial))
            {
                _meshRenderer.material = alternativeMaterial;
            }
        }

        private void SetOriginalMaterialActive()
        {
            if (_meshRenderer != null
                && (!requireOriginalMaterialMatch || _meshRenderer.material == alternativeMaterial))
            {
                _meshRenderer.material = _originalMaterial;
            }
        }

        private void SetMaterialToggled()
        {
            if (_meshRenderer != null)
            {
                if (_meshRenderer.material == alternativeMaterial)
                {
                    SetAlternativeMaterialActive();
                }
                else
                {
                    SetOriginalMaterialActive();
                }
            }
        }
        #endregion
    }
}