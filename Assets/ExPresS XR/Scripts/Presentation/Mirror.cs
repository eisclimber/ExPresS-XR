using UnityEngine;

namespace ExPresSXR.Presentation
{
    /// <summary>
    /// Represents a mirror. Be careful as the reflection angles to not exactly line up.
    /// For accurate and performant mirrors use a separate package.
    /// </summary>
    public class Mirror : MonoBehaviour
    {   
        private const int RENDER_TEXTURE_DEPTH = 16;

        [SerializeField]
        [Tooltip("Aspect ratio of the mirror plane in px.")]
        private Vector2 _pixelRatio = new(1080, 1080);
        /// <summary>
        /// Aspect ratio of the mirror plane in px.
        /// </summary>
        public Vector2 PixelRatio
        {
            get => _pixelRatio;
            set
            {
                _pixelRatio = value;
                UpdateRenderTextures();
            }
        }
        
        [Range(0.0f, 1.0f)]
        [SerializeField]
        [Tooltip("Resolution of the mirror in percentage. Scales the amount of pixels of the aspect ratio, higher values might impact performance.")]
        private float _resolutionPct = 1.0f;
        /// <summary>
        /// Resolution of the mirror in percentage. Scales the amount of pixels of the aspect ratio, higher values might impact performance.
        /// </summary>
        public float ResolutionPct
        {
            get => _resolutionPct;
            set
            {
                _resolutionPct = value;
                UpdateRenderTextures();
            }
        }

        [SerializeField]
        [Tooltip("If enabled require providing a custom RenderTexture. Else it will be generated automatically.")]
        private bool _provideCustomRenderTexture;
        /// <summary>
        /// If enabled require providing a custom RenderTexture. Else it will be generated automatically.
        /// </summary>
        public bool ProvideCustomRenderTexture
        {
            get => _provideCustomRenderTexture;
            set
            {
                _provideCustomRenderTexture = value;
                UpdateRenderTextures();
            }
        }

        [SerializeField]
        [Tooltip("The RenderTexture that is used when 'provideCustomRenderTexture' is enabled.")]
        private RenderTexture _customRenderTexture;
        /// <summary>
        /// The RenderTexture that is used when 'provideCustomRenderTexture' is enabled.
        /// </summary>
        public RenderTexture CustomRenderTexture
        {
            get => _customRenderTexture;
            set
            {
                _customRenderTexture = value;
                UpdateRenderTextures();
            }
        }

        [SerializeField]
        [Tooltip("Texture that is laid over the mirror to make it look more realistic (e.g. dirt, fingerprints, ...). "
                + "Some example textures can be found at 'ExPresS XR/Sprites/Mirror/'.")]
        private Texture _overlayTexture;
        /// <summary>
        /// Texture that is laid over the mirror to make it look more realistic (e.g. dirt, fingerprints, ...).
        /// Some example textures can be found at 'ExPresS XR/Sprites/Mirror/'.
        /// </summary>
        public Texture OverlayTexture
        {
            get => _overlayTexture;
            set
            {
                _overlayTexture = value;
                UpdateMirrorMaterial();
            }
        }

        [Range(0.0f, 1.0f)]
        [SerializeField]
        [Tooltip("Strength of the effect applied by the overlayTexture.")]
        private float _overlayStrength = 0.5f;
        /// <summary>
        /// Strength of the effect applied by the overlayTexture.
        /// </summary>
        public float OverlayStrength
        {
            get => _overlayStrength;
            set
            {
                _overlayStrength = value;
                UpdateMirrorMaterial();
            }
        }

        [SerializeField]
        [Tooltip("Color that is mixed with the displayed image to change it's color. Use white for no tinting.")]
        private Color _tintColor = Color.white;
        /// <summary>
        /// Color that is mixed with the displayed image to change it's color. Use white for no tinting.
        /// </summary>
        public Color TintColor
        {
            get => _tintColor;
            set
            {
                _tintColor = value;
                UpdateMirrorMaterial();
            }
        }

        [Range(0.0f, 1.0f)]
        [SerializeField]
        [Tooltip("Factor that shifts the bightness the displayed image.")]
        private float _brighteningFactor = 0.0f;
        /// <summary>
        /// Factor that shifts the bightness the displayed image.
        /// </summary>
        public float BrighteningFactor
        {
            get => _brighteningFactor;
            set
            {
                _brighteningFactor = value;
                UpdateMirrorMaterial();
            }
        }

        // Targets and GameObjects

        /// <summary>
        /// Target for which the mirror effect is simulated. It should be best set to the Camera of an XR Rig.
        /// </summary>
        [SerializeField]
        [Tooltip("Target for which the mirror effect is simulated. It should be best set to the Camera of an XR Rig.")]
        private Transform _trackedTarget;
        
        /// <summary>
        /// Reference to the Mirror's Camera.
        /// </summary>
        [SerializeField]
        [Tooltip("Reference to the Mirror's Camera.")]
        private Camera _mirrorCamera;

        [SerializeField]
        [Tooltip("Reference to the Mirror's Plane.")]
        private Transform _mirrorPlane;
        /// <summary>
        /// Reference to the Mirror's Plane.
        /// </summary>
        public Transform MirrorPlane
        {
            get => _mirrorPlane;
            set
            {
                _mirrorPlane = value;
                UpdateMirrorMaterial();
            }
        }

        /// <summary>
        /// The RenderTexture currently used for the mirror.
        /// </summary>
        public RenderTexture ActiveRenderTexture
        {
            get => ProvideCustomRenderTexture ? _customRenderTexture : _generatedRenderTexture;
        }

        /// <summary>
        /// The pixel size of the currently used RenderTexture. Returns (0,0) if no texture is set.
        /// </summary>
        public Vector2 ActualPixelResultion
        {
            get
            {
                Texture renderTexture = ActiveRenderTexture;
                return renderTexture != null ? 
                        new Vector2(renderTexture.width, renderTexture.height) : 
                        Vector2.zero;
            }
        }

        [SerializeField]
        [Tooltip("The material used to display the RenderTexture.")]
        private Material _mirrorMaterial;
        /// <summary>
        /// The material used to display the RenderTexture.
        /// </summary>
        public Material MirrorMaterial
        {
            get => _mirrorMaterial;
            set
            {
                _mirrorMaterial = value;

                if (_mirrorPlane.TryGetComponent(out Renderer renderer))
                {
                    renderer.sharedMaterial = _mirrorMaterial;
                }
                UpdateMirrorMaterial();
            }
        }

        // [SerializeField]
        private RenderTexture _generatedRenderTexture;



        private void Awake() {

            if (_trackedTarget == null)
            {
                Debug.LogWarning("The Tracked Target is null. The mirror won't follow anything.");
            }
            else if (!_trackedTarget.TryGetComponent<Camera>(out _))
            {
                Debug.LogWarning("Target was not set to a camera. The mirror might display the wrong perspective.");
            }
        }

        private void Update() {
            if (_trackedTarget == null)
            {
                return;
            }

            // Change Camera's position
            Vector3 targetPos = transform.InverseTransformPoint(_trackedTarget.position);
            _mirrorCamera.transform.position = transform.TransformPoint(new Vector3(targetPos.x, targetPos.y, -targetPos.z));

            // Change Camera's rotation
            Vector3 mirrorAnglePos = transform.TransformPoint(new Vector3(-targetPos.x, targetPos.y, targetPos.z));
            _mirrorCamera.transform.LookAt(mirrorAnglePos);
        }


        private void UpdateRenderTextures()
        {
            if (!ProvideCustomRenderTexture)
            {
                Vector3 texturePixels = _pixelRatio * _resolutionPct;

                // Ensure the texture has dimensions greater than 0
                texturePixels.x = Mathf.Max(texturePixels.x, 1.0f);
                texturePixels.y = Mathf.Max(texturePixels.y, 1.0f);

                _generatedRenderTexture = new((int)texturePixels.x, (int)texturePixels.y, RENDER_TEXTURE_DEPTH)
                {
                    name = "Generated Mirror Render Texture"
                };
            }
            DisplayActiveRenderTexture();
        }

        private void DisplayActiveRenderTexture()
        {
            if (ActiveRenderTexture != null)
            {
                if (_mirrorCamera != null)
                {
                    _mirrorCamera.targetTexture = ActiveRenderTexture;
                }
                else
                {
                    Debug.LogWarning("MirrorCamera is null. The mirror won't display anything.");
                }

                if (_mirrorMaterial != null)
                {
                    _mirrorMaterial.mainTexture = ActiveRenderTexture;
                }
                else
                {
                    Debug.LogWarning("MirrorMaterial is null. The mirror won't display anything.");
                }
            }
            else
            {
                Debug.LogWarning("Active RenderTexture is null. The mirror won't display anything."
                        + (ProvideCustomRenderTexture ?
                            "Be sure to provide your own renderTexture or disable the provideCustomRenderTexture." : 
                            "")
                );
            }
        }


        private void UpdateMirrorMaterial()
        {
            if (_mirrorPlane.TryGetComponent(out Renderer renderer))
            {
                _mirrorMaterial = renderer.sharedMaterial;

                if (_mirrorMaterial != null)
                {
                    // Change displayed mirror texture
                    _mirrorMaterial.mainTexture = ActiveRenderTexture;
                    _mirrorMaterial.color = _tintColor;

                    // Add/Enable overlay as metallic
                    _mirrorMaterial.SetTexture("_MetallicGlossMap", _overlayTexture);
                    if (_overlayTexture != null)
                    {
                        _mirrorMaterial.EnableKeyword("_METALLICSPECGLOSSMAP");
                        _mirrorMaterial.SetFloat("_Smoothness", _overlayStrength);
                    }
                    else
                    {
                        _mirrorMaterial.DisableKeyword("_METALLICSPECGLOSSMAP");
                        _mirrorMaterial.SetFloat("_Smoothness", 0.0f);
                    }

                    // Brighten up image using emission
                    if (BrighteningFactor > 0)
                    {
                        _mirrorMaterial.EnableKeyword("_EMISSION");
                        _mirrorMaterial.SetTexture("_EmissionMap", ActiveRenderTexture);
                        _mirrorMaterial.SetColor("_EmissionColor", Color.white * _brighteningFactor);
                    }
                    else
                    {
                        _mirrorMaterial.DisableKeyword("_EMISSION");
                    }
                }
                else
                {
                    Debug.LogWarning("The Mirror Plane does Material assigned. "
                        + "The mirror won't display anything.");
                }
            }
            else
            {
                Debug.LogWarning("The Mirror Plane does not have a Renderer-Component. "
                        + "The mirror won't display anything.");
            }
        }


        private void OnValidate() {
            UpdateRenderTextures();
            UpdateMirrorMaterial();
        }
    }
}