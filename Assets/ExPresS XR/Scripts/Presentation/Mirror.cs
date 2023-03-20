using UnityEngine;

namespace ExPresSXR.Presentation
{
    public class Mirror : MonoBehaviour
    {   
        private const int RENDER_TEXTURE_DEPTH = 16;

        [Tooltip("Aspect ratio of the mirror plane in px.")]
        [SerializeField]
        private Vector2 _pixelRatio = new(1080, 1080);
        public Vector2 pixelRatio
        {
            get => _pixelRatio;
            set
            {
                _pixelRatio = value;
                UpdateRenderTextures();
            }
        }
        
        [Tooltip("Resolution of the mirror in percentage. Scales the amount of pixels of the aspect ratio, higher values might impact performance.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _resolutionPct = 1.0f;
        public float resolutionPct
        {
            get => _resolutionPct;
            set
            {
                _resolutionPct = value;
                UpdateRenderTextures();
            }
        }

        [Tooltip("If enabled require providing a custom RenderTexture. Else it will be generated automatically.")]
        [SerializeField]
        private bool _provideCustomRenderTexture;
        public bool provideCustomRenderTexture
        {
            get => _provideCustomRenderTexture;
            set
            {
                _provideCustomRenderTexture = value;
                UpdateRenderTextures();
            }
        }

        [Tooltip("The RenderTexture that is used when 'provideCustomRenderTexture' is enabled.")]
        [SerializeField]
        private RenderTexture _customRenderTexture;
        public RenderTexture customRenderTexture
        {
            get => _customRenderTexture;
            set
            {
                _customRenderTexture = value;
                UpdateRenderTextures();
            }
        }

        // Image Modification
        [Tooltip("Texture that is laid over the mirror to make it look more realistic (e.g. dirt, fingerprints, ...). "
                + "Some example textures can be found at 'ExPresS XR/Sprites/Mirror/'.")]
        [SerializeField]
        private Texture _overlayTexture;
        public Texture overlayTexture
        {
            get => _overlayTexture;
            set
            {
                _overlayTexture = value;
                UpdateMirrorMaterial();
            }
        }

        [Tooltip("Strength of the effect applied by the overlayTexture.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _overlayStrength = 0.5f;
        public float overlayStrength
        {
            get => _overlayStrength;
            set
            {
                _overlayStrength = value;
                UpdateMirrorMaterial();
            }
        }

        [Tooltip("Color that is mixed with the displayed image to change it's color. Use white for no tinting.")]
        [SerializeField]
        private Color _tintColor = Color.white;
        public Color tintColor
        {
            get => _tintColor;
            set
            {
                _tintColor = value;
                UpdateMirrorMaterial();
            }
        }

        [Tooltip("Factor that shifts the bightness the displayed image.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _brighteningFactor = 0.0f;
        public float brighteningFactor
        {
            get => _brighteningFactor;
            set
            {
                _brighteningFactor = value;
                UpdateMirrorMaterial();
            }
        }

        // Targets and GameObjects

        [Tooltip("Target for which the mirror effect is simulated. It should be best set to the Camera of an XR Rig.")]
        [SerializeField]
        private Transform _trackedTarget;
        
        [Tooltip("Reference to the Mirror's Camera.")]
        [SerializeField]
        private Camera _mirrorCamera;

        [Tooltip("Reference to the Mirror's Plane.")]
        [SerializeField]
        private Transform _mirrorPlane;
        public Transform mirrorPlane
        {
            get => _mirrorPlane;
            set
            {
                _mirrorPlane = value;
                UpdateMirrorMaterial();
            }
        }

        [Tooltip("The RenderTexture currently used for the mirror.")]
        public RenderTexture activeRenderTexture
        {
            get => provideCustomRenderTexture ? _customRenderTexture : _generatedRenderTexture;
        }

        [Tooltip("The pixel size of the currently used RenderTexture. Returns (0,0) if no texture is set.")]
        public Vector2 actualPixelResultion
        {
            get
            {
                Texture renderTexture = activeRenderTexture;
                return renderTexture != null ? 
                        new Vector2(renderTexture.width, renderTexture.height) : 
                        Vector2.zero;
            }
        }

        [Tooltip("The material used to display the RenderTexture.")]
        [SerializeField]
        private Material _mirrorMaterial;
        public Material mirrorMaterial
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
            if (!provideCustomRenderTexture)
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
            if (activeRenderTexture != null)
            {
                if (_mirrorCamera != null)
                {
                    _mirrorCamera.targetTexture = activeRenderTexture;
                }
                else
                {
                    Debug.LogWarning("MirrorCamera is null. The mirror won't display anything.");
                }

                if (_mirrorMaterial != null)
                {
                    _mirrorMaterial.mainTexture = activeRenderTexture;
                }
                else
                {
                    Debug.LogWarning("MirrorMaterial is null. The mirror won't display anything.");
                }
            }
            else
            {
                Debug.LogWarning("Active RenderTexture is null. The mirror won't display anything."
                        + (provideCustomRenderTexture ?
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
                    _mirrorMaterial.mainTexture = activeRenderTexture;
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
                    if (brighteningFactor > 0)
                    {
                        _mirrorMaterial.EnableKeyword("_EMISSION");
                        _mirrorMaterial.SetTexture("_EmissionMap", activeRenderTexture);
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