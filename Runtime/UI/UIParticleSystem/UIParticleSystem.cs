using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer), typeof(ParticleSystem))]
    public class UIParticleSystem : MaskableGraphic
    {
        public bool fixedTime = true;
        [Range(1, 100)] public int maxParticleCount = 100;

        private Transform _transform;
        private ParticleSystem pSystem;
        private ParticleSystem.Particle[] particles;
        private UIVertex[] _quad = new UIVertex[4];
        private Vector4 imageUV = Vector4.zero;
        private ParticleSystem.TextureSheetAnimationModule textureSheetAnimation;
        private int textureSheetAnimationFrames;
        private Vector2 textureSheetAnimationFrameSize;
        private ParticleSystemRenderer pRenderer;

        private Material currentMaterial;

        private Texture currentTexture;

        private ParticleSystem.MainModule mainModule;

        public override Texture mainTexture
        {
            get { return currentTexture; }
        }

        protected bool Initialize()
        {
            // initialize members
            if (_transform == null)
            {
                _transform = transform;
            }

            if (pSystem == null)
            {
                pSystem = GetComponent<ParticleSystem>();

                if (pSystem == null)
                {
                    return false;
                }

                mainModule = pSystem.main;
                if (pSystem.main.maxParticles > maxParticleCount)
                {
                    mainModule.maxParticles = maxParticleCount;
                }

                pRenderer = pSystem.GetComponent<ParticleSystemRenderer>();
                if (pRenderer != null)
                {
                    pRenderer.material = null;
                    pRenderer.enabled = false;
                }

                currentMaterial = material;
                mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;

                particles = null;
            }

            if (particles == null)
                particles = new ParticleSystem.Particle[pSystem.main.maxParticles];

            imageUV = new Vector4(0, 0, 1, 1);

            textureSheetAnimation = pSystem.textureSheetAnimation;
            textureSheetAnimationFrames = 0;
            textureSheetAnimationFrameSize = Vector2.zero;
            if (textureSheetAnimation.enabled)
            {
                textureSheetAnimationFrames = textureSheetAnimation.numTilesX * textureSheetAnimation.numTilesY;
                textureSheetAnimationFrameSize = new Vector2(1f / textureSheetAnimation.numTilesX,
                    1f / textureSheetAnimation.numTilesY);
            }

            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            if (!Initialize())
                enabled = false;

            raycastTarget = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!Initialize())
                {
                    return;
                }
            }
#endif

            vh.Clear();

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            Vector2 temp = Vector2.zero;
            Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.zero;
            // iterate through current particles
            int count = pSystem.GetParticles(particles);

            for (int i = 0; i < count; ++i)
            {
                ParticleSystem.Particle particle = particles[i];


                Vector2 position = (mainModule.simulationSpace == ParticleSystemSimulationSpace.Local
                    ? particle.position
                    : _transform.InverseTransformPoint(particle.position));
                float rotation = -particle.rotation * Mathf.Deg2Rad;
                float rotation90 = rotation + Mathf.PI / 2;
                Color32 color = particle.GetCurrentColor(pSystem);
                float size = particle.GetCurrentSize(pSystem) * 0.5f;

                if (mainModule.scalingMode == ParticleSystemScalingMode.Shape)
                    position /= canvas.scaleFactor;

                Vector4 particleUV = imageUV;
                if (textureSheetAnimation.enabled)
                {
                    float frameProgress = 1 - (particle.remainingLifetime / particle.startLifetime);

                    if (textureSheetAnimation.frameOverTime.curveMin != null)
                    {
                        frameProgress =
                            textureSheetAnimation.frameOverTime.curveMin.Evaluate(1 - (particle.remainingLifetime /
                                particle.startLifetime));
                    }
                    else if (textureSheetAnimation.frameOverTime.curve != null)
                    {
                        frameProgress =
                            textureSheetAnimation.frameOverTime.curve.Evaluate(1 - (particle.remainingLifetime /
                                particle.startLifetime));
                    }
                    else if (textureSheetAnimation.frameOverTime.constant > 0)
                    {
                        frameProgress = textureSheetAnimation.frameOverTime.constant -
                                        (particle.remainingLifetime / particle.startLifetime);
                    }

                    frameProgress = Mathf.Repeat(frameProgress * textureSheetAnimation.cycleCount, 1);
                    int frame = 0;

                    switch (textureSheetAnimation.animation)
                    {
                        case ParticleSystemAnimationType.WholeSheet:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimationFrames);
                            break;

                        case ParticleSystemAnimationType.SingleRow:
                            frame = Mathf.FloorToInt(frameProgress * textureSheetAnimation.numTilesX);

                            int row = textureSheetAnimation.rowIndex;
                            frame += row * textureSheetAnimation.numTilesX;
                            break;
                    }

                    frame %= textureSheetAnimationFrames;

                    particleUV.x = (frame % textureSheetAnimation.numTilesX) * textureSheetAnimationFrameSize.x;
                    particleUV.y = Mathf.FloorToInt(frame / textureSheetAnimation.numTilesX) *
                                   textureSheetAnimationFrameSize.y;
                    particleUV.z = particleUV.x + textureSheetAnimationFrameSize.x;
                    particleUV.w = particleUV.y + textureSheetAnimationFrameSize.y;
                }

                temp.x = particleUV.x;
                temp.y = particleUV.y;

                _quad[0] = UIVertex.simpleVert;
                _quad[0].color = color;
                _quad[0].uv0 = temp;

                temp.x = particleUV.x;
                temp.y = particleUV.w;
                _quad[1] = UIVertex.simpleVert;
                _quad[1].color = color;
                _quad[1].uv0 = temp;

                temp.x = particleUV.z;
                temp.y = particleUV.w;
                _quad[2] = UIVertex.simpleVert;
                _quad[2].color = color;
                _quad[2].uv0 = temp;

                temp.x = particleUV.z;
                temp.y = particleUV.y;
                _quad[3] = UIVertex.simpleVert;
                _quad[3].color = color;
                _quad[3].uv0 = temp;

                if (rotation == 0)
                {
                    // no rotation
                    corner1.x = position.x - size;
                    corner1.y = position.y - size;
                    corner2.x = position.x + size;
                    corner2.y = position.y + size;

                    temp.x = corner1.x;
                    temp.y = corner1.y;
                    _quad[0].position = temp;
                    temp.x = corner1.x;
                    temp.y = corner2.y;
                    _quad[1].position = temp;
                    temp.x = corner2.x;
                    temp.y = corner2.y;
                    _quad[2].position = temp;
                    temp.x = corner2.x;
                    temp.y = corner1.y;
                    _quad[3].position = temp;
                }
                else
                {
                    // apply rotation
                    Vector2 right = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)) * size;
                    Vector2 up = new Vector2(Mathf.Cos(rotation90), Mathf.Sin(rotation90)) * size;

                    _quad[0].position = position - right - up;
                    _quad[1].position = position - right + up;
                    _quad[2].position = position + right + up;
                    _quad[3].position = position + right - up;
                }

                vh.AddUIVertexQuad(_quad);
            }
        }

        void Update()
        {
            if (!fixedTime && Application.isPlaying)
            {
                pSystem.Simulate(Time.unscaledDeltaTime, false, false, true);
                SetAllDirty();

                if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) ||
                    (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
                {
                    pSystem = null;
                    Initialize();
                }
            }
        }

        void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                SetAllDirty();
            }
            else
            {
                if (fixedTime)
                {
                    pSystem.Simulate(Time.unscaledDeltaTime, false, false, false);
                    SetAllDirty();
                    if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) ||
                        (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
                    {
                        pSystem = null;
                        Initialize();
                    }
                }
            }

            if (material == currentMaterial)
                return;
            pSystem = null;
            Initialize();
        }
    }
}