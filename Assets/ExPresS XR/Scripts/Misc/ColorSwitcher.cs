using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExPresSXR.Misc
{
    public class ColorSwitcher : MonoBehaviour
    {
        [Tooltip("The material that is replacing the material applied to the GameObject via Editor.")]
        public Material alternativeMaterial;

        public float switchDuration = 1.0f;

        private Material _originalMaterial;
        private MeshRenderer _meshRenderer;


        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _originalMaterial = _meshRenderer.material;

            if (alternativeMaterial == null)
            {
                Debug.LogWarning("No Material assigned to Color Switcher. Materials won't switch.");
            }
        }

        // Instant Switches
        public void ActivateAlternativeMaterial()
        {
            StopAllCoroutines();
            SetAlternativeMaterialActive();
        }

        public void ActivateOriginalMaterial()
        {
            StopAllCoroutines();
            SetOriginalMaterialActive();
        }

        public void ToggleMaterial()
        {
            StopAllCoroutines();
            SetMaterialToggled();
        }

        // Fixed 1 second switches
        public void ActivateAlternativeMaterialForASecond()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateAlternativeMaterialForSecondsCoroutine(1f));
        }

        public void ActivateOriginalMaterialForASecond()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateOriginalMaterialForSecondsCoroutine(1f));
        }

        public void ToggleMaterialForASecond()
        {
            StopAllCoroutines();
            StartCoroutine(ToggleMaterialForSecondsCoroutine(1f));
        }


        // Switches for `switchDuration`
        public void ActivateAlternativeMaterialForSwitchDuration()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateAlternativeMaterialForSecondsCoroutine(switchDuration));
        }

        public void ActivateOriginalMaterialForSwitchDuration()
        {
            StopAllCoroutines();
            StartCoroutine(ActivateOriginalMaterialForSecondsCoroutine(switchDuration));
        }

        public void ToggleMaterialForSwitchDuration()
        {
            StopAllCoroutines();
            StartCoroutine(ToggleMaterialForSecondsCoroutine(switchDuration));
        }


        // Coroutine Switches
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
            if (_meshRenderer != null && alternativeMaterial != null)
            {
                _meshRenderer.material = alternativeMaterial;
            }
        }

        private void SetOriginalMaterialActive()
        {
            if (_meshRenderer != null)
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
    }
}