using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (Camera))]
public class FullScreenCameraFade : MonoBehaviour
{
    /// <summary>
    /// IMPORTANT!! CURRENTLY NOT AVAILABLE
    /// </summary>

    private const string DEFAULT_MATERIAL_PATH = "Materials/AutoXR/";

    [SerializeField]
    private Material _material;

    [SerializeField]
    private Color _fadeColor;

    [SerializeField]
    private float _fadeToBlackTime = 0.5f;

    [SerializeField]
    private float _fadeToClearTime = 0.5f;

    [SerializeField]
    private float _fadeDirection = 0.0f;

    private Vector3 pTL;
    private Vector3 pTR;
    private Vector3 pBL;
    private Vector3 pBR;


    private void Awake()
    {
        if (_material == null)
        {
            // Use unity's built-in shader for drawing the rect
            var shader = Shader.Find("Hidden/Internal-Colored");
            _material = new Material(shader);
            _material.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FadeToBlack(Input.GetKeyDown(KeyCode.LeftShift));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            FadeToClear(Input.GetKeyDown(KeyCode.LeftShift));
        }

        if (_fadeDirection < 0.0f)
        {
            // Fade to Black
            _fadeColor.a = Mathf.Max(0.0f, _fadeColor.a - (Time.deltaTime / _fadeToBlackTime));
            // Debug.Log(_fadeColor.a);
        }
        else if (_fadeDirection > 0.0f)
        {
            // Fade to Clear
            _fadeColor.a = Mathf.Min(1.0f, _fadeColor.a + (Time.deltaTime / _fadeToClearTime));
            // Debug.Log(_fadeColor.a);
        }
    }

    void OnPostRender()
    {
        if (_material == null)
        {
            return;
        }

        // Draw a rect that covers the cameras view
        GL.PushMatrix();
        GL.LoadOrtho();
        _material.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.Color(_fadeColor);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(0, 1, 0);
        GL.End();
        GL.PopMatrix();
    }

    private void FadeToBlack(bool instant = false)
    {
        _fadeDirection = 1.0f;
        if (instant)
        {
            _fadeColor = Color.black;
        }
    }

    private void FadeToClear(bool instant = false)
    {
        _fadeDirection = -1.0f;
        if (instant)
        {
            _fadeColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }


    // Links OnPostRender with the events of the URP/HDRP
    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering +=
            RenderPipelineManagerEndCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -=
            RenderPipelineManagerEndCameraRendering;
    }

    private void RenderPipelineManagerEndCameraRendering(
        ScriptableRenderContext context,
        Camera camera
    )
    {
        OnPostRender();
    }
}
