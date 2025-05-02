using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace ExPresSXR.Minigames.Excavation
{
    /// <summary>
    /// Make sure that the mesh you are using is **NOT** convex and in the import settings of the meshes's model (fbx, gltf, ...) "Read/Write" is enabled!
    /// If you don't do it, it will work in the editor but not in the build.
    /// </summary>
    public class ExcavationArea : MonoBehaviour
    {
        /// <summary>
        /// Size of the brush when drawing with the mouse.
        /// </summary>
        private const float DEFAULT_BRUSH_SIZE = 10.0f;

        /// <summary>
        /// The detail level of the source RenderTexture. The resulting size of the texture is 2^{_detailLevel}, i.e. a value of 10 equals 2^10 = 1024 pixels.
        /// </summary>
        [SerializeField]
        [Tooltip("The detail level of the source RenderTexture. The resulting size of the texture is 2^{_detailLevel}, i.e. a value of 10 equals 2^10 = 1024 pixels.")]
        private int _detailLevel = 10;

        /// <summary>
        /// Shader used for drawing. You can use the ExcavationDrawShader in the shader folder of ExPresS XR.
        /// </summary>
        [SerializeField]
        [Tooltip("Shader used for drawing. You can use the ExcavationDrawShader in the shader folder of ExPresS XR.")]
        private Shader _drawShader;

        /// <summary>
        /// Renderer holding the excavation material.
        /// </summary>
        [SerializeField]
        [Tooltip("Renderer holding the excavation material.")]
        private Renderer _renderer;

        /// <summary>
        /// Draws a texture at the top left of the screen when playing via the editor showing the texture with the current state of the excavation mask texture.
        /// </summary>
        [SerializeField]
        [Tooltip("Draws a texture at the top left of the screen when playing via the editor showing the texture with the current state of the excavation mask texture.")]
        private bool _debugDrawTexture;

        private RenderTexture _excavationTexture;
        private Material _excavationMaterial;
        private Material _drawMaterial;
        private RaycastHit _hit;


        private void Awake()
        {
            int textureSize = 2 << (_detailLevel - 1);
            _excavationTexture = new(textureSize, textureSize, 0, RenderTextureFormat.ARGB32)
            {
                useMipMap = true
            };

            _drawMaterial = new Material(_drawShader);
        }

        private void Start()
        {
            _excavationMaterial = _renderer.material;
            _excavationMaterial.SetTexture("_SplatMap", _excavationTexture);
        }

        /*
         * For debugging enable this function and point a normal camera towards the area.
         * Make sure you have SteamVR disabled as it messes up the raycast...
         */
        private void Update()
        {
#if UNITY_EDITOR
            bool lmbPressed = Mouse.current.leftButton.isPressed;
            bool rmbPressed = Mouse.current.rightButton.isPressed;
            if (lmbPressed || rmbPressed)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (Camera.allCameras.Length < 0)
                {
                    Debug.LogWarning("No camera found, can't paint with the mouse!");
                    return;
                }

                Ray ray = Camera.allCameras[0].ScreenPointToRay(mousePos);
                // Check if target was valid
                if (Physics.Raycast(ray, out _hit)
                        && _hit.collider.gameObject.TryGetComponent(out ExcavationArea _))
                {
                    ExcavateAt(_hit.textureCoord, lmbPressed ? Color.red : Color.green, DEFAULT_BRUSH_SIZE);
                }
            }
#endif
        }

        /// <summary>
        /// Excavates at a position by adjusting the parameters of the draw shader
        /// and drawing it to the texture that is used as depth map of the areas material.
        /// </summary>
        /// <param name="pos">Texture position (normalized) to draw at.</param>
        /// <param name="color">Color to draw with.</param>
        /// <param name="brushSize">Size of the brush.</param>
        public void ExcavateAt(Vector2 pos, Color color, float brushSize)
        {
            if (pos == Vector2.zero)
            {
                // Prevent drawing if nothing was hit
                return;
            }

            // Update draw position
            _drawMaterial.SetVector("_Coordinate", new Vector4(pos.x, pos.y, 0.0f, 0.0f));
            _drawMaterial.SetVector("_Color", color);
            _drawMaterial.SetFloat("_BrushSize", brushSize);
            RenderTexture temp = RenderTexture.GetTemporary(_excavationTexture.width, _excavationTexture.height, 0, RenderTextureFormat.ARGB32);
            // Copy the splatMap to not interfere later processing, then use the draw Material to add a circle at the coordinates.
            Graphics.Blit(_excavationTexture, temp);
            Graphics.Blit(temp, _excavationTexture, _drawMaterial, 0);
            // Write back texture
            _excavationMaterial.SetTexture("_SplatMap", _excavationTexture);
            // Release the temporary RenderTexture to not leak memory
            RenderTexture.ReleaseTemporary(temp);
        }

        /// <summary>
        /// Returns the average color of the splatMap.
        /// THIS IS INSANELY EXPENSIVE AS THE GPU AND CPU NEED TO SYNC! CALL SPARINGLY!!!
        /// </summary>
        /// <returns>Average Color.</returns>
        public void GetAvgColors(int _granularity, Action<AsyncGPUReadbackRequest> callback)
        {
            if (_granularity < 0)
            {
                Debug.LogError($"Granularity is less than 0. Its value must be in the range [0, {_detailLevel}]. "
                    + "Return data for the min granularity instead.", this);
            }
            if (_granularity > _detailLevel)
            {
                Debug.LogError($"Granularity is greater than the detail level of the texture. Its value must be in the range [0, {_detailLevel}]. "
                    + "Return data for the max granularity instead.", this);
            }
            // Ensure correct value, mipLevel is the inverse of granularity.
            int mipLevel = Mathf.Clamp(_detailLevel - _granularity, 0, _detailLevel);

            // Read texture from gpu
            AsyncGPUReadback.Request(_excavationTexture, mipLevel, callback);
        }

        /// <summary>
        /// For debugging.Draws the draw texture.
        /// </summary>
        private void OnGUI()
        {
            if (_debugDrawTexture)
            {
                GUI.DrawTexture(new Rect(0, 0, 256, 256), _excavationTexture, ScaleMode.ScaleToFit, false, 1);
            }
        }
    }
}