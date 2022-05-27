using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorSwitcher : MonoBehaviour
{
    [Tooltip("The material that is replacing the material applied to the GameObject via Editor.")]
    public Material alternativeMaterial;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;

    private Coroutine switchCoroutine;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        if (alternativeMaterial == null)
        {
            Debug.LogWarning("No Material assigned to Color Switcher. Materials won't switch.");
        }
    }

    // Instant Switches
    public void ActivateAlternativeMaterial()
    {
        if (alternativeMaterial != null)
        {
            meshRenderer.material = alternativeMaterial;
        }
    }

    public void ActivateOriginalMaterial()
    {
        meshRenderer.material = originalMaterial;
    }

    public void ToggleMaterial()
    {
        if (alternativeMaterial != null && meshRenderer.material == alternativeMaterial)
        {
            meshRenderer.material = originalMaterial;
        }
        else
        {
            meshRenderer.material = alternativeMaterial;
        }
    }

    // Coroutine Switches
    public IEnumerator ActivateAlternativeMaterialForSecondsCoroutine(float time)
    {
        ActivateAlternativeMaterial();
        yield return new WaitForSeconds(time);
        ActivateOriginalMaterial();
    }

    public IEnumerator ActivateOriginalMaterialForSecondsCoroutine(float time)
    {
        ActivateOriginalMaterial();
        yield return new WaitForSeconds(time);
        ActivateAlternativeMaterial();
    }

    public void ActivateAlternativeMaterialForASecond() => StartCoroutine(ActivateAlternativeMaterialForSecondsCoroutine(1f));
    
    public void ActivateOriginalMaterialForASecond() => StartCoroutine(ActivateOriginalMaterialForSecondsCoroutine(1f));
}
