#if GRIFFIN 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
#if GRIFFIN_URP
using UnityEngine.Rendering.Universal;
#endif

namespace Pinwheel.Griffin.PoseidonPreview
{
    public class PoseidonWaterBodyPreview : MonoBehaviour
    {
        public delegate void ShapeChangedHandler(PoseidonWaterBodyPreview sender);
        public static event ShapeChangedHandler shapeChanged;

        [SerializeField]
        protected Material m_defaultMaterial;
        [SerializeField]
        protected Material m_material;
        public Material material
        {
            get
            {
                return m_material;
            }
            set
            {
                m_material = value;
            }
        }

        [SerializeField]
        private TimeMode m_timeMode;
        public TimeMode timeMode
        {
            get
            {
                return m_timeMode;
            }
            set
            {
                m_timeMode = value;
            }
        }

        [SerializeField]
        private float m_manualTimeSeconds;
        public float manualTimeSeconds
        {
            get
            {
                return m_manualTimeSeconds;
            }
            set
            {
                m_manualTimeSeconds = value;
            }
        }

        public float GetTimeParam()
        {
            if (timeMode == TimeMode.Auto)
            {
                return Time.realtimeSinceStartup;
            }
            else
            {
                return manualTimeSeconds;
            }
        }

        protected virtual void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("Water");

            if (m_defaultMaterial != null)
            {
                m_material = Object.Instantiate(m_defaultMaterial);
                m_defaultMaterial = null;
            }
        }

        protected virtual void OnEnable()
        {
            Camera.onPreCull += OnCameraPreCullBiRP;
#if GRIFFIN_URP 
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRenderingSRP;
#endif
        }

        protected virtual void OnDisable()
        {
            Camera.onPreCull -= OnCameraPreCullBiRP;
#if GRIFFIN_URP
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRenderingSRP;
#endif
        }

        protected virtual void Update()
        {

        }

        private void OnCameraPreCullBiRP(Camera cam)
        {
            if (material != null)
            {
                bool requireDepthTexture = PMat.HasLightAbsorptionFX(material) || PMat.HasFoamFX(material);
                if (requireDepthTexture)
                {
                    cam.depthTextureMode |= DepthTextureMode.Depth;
                }
            }
        }

#if GRIFFIN_URP //originally POSEIDON_URP but Poseidon package initializer will be stripped, so use this instead
        private void OnBeginCameraRenderingSRP(ScriptableRenderContext context, Camera cam)
        {
            if (material != null)
            {
#if UNITY_EDITOR               
                UniversalRenderPipelineAsset uAsset = UniversalRenderPipeline.asset;
                bool requireDepthTexture = PMat.HasLightAbsorptionFX(material) || PMat.HasFoamFX(material) || PMat.HasCausticFX(material);
                bool uAssetChanged = false;
                if (requireDepthTexture)
                {
                    cam.depthTextureMode = DepthTextureMode.Depth;
                    if (uAsset.supportsCameraDepthTexture == false)
                    {
                        uAsset.supportsCameraDepthTexture = true;
                        uAssetChanged = true;
                        Debug.Log($"Enabling Depth Texture in {uAsset.name} for Light Absorption & Foam.", uAsset);
                    }
                }

                //bool requireOpaqueTexture = PMat.HasRefractionFX(material);
                //if (requireOpaqueTexture)
                //{
                //    if (uAsset.supportsCameraOpaqueTexture == false)
                //    {
                //        uAsset.supportsCameraOpaqueTexture = true;
                //        uAssetChanged = true;
                //        Debug.Log($"Enabling Opaque Texture in {uAsset.name} for Refraction.", uAsset);
                //    }
                //}


                if (uAssetChanged)
                {
                    UnityEditor.EditorUtility.SetDirty(uAsset);
                }
#endif
            }
        }
#endif

        public virtual void GetRenderers(List<MeshRenderer> container)
        {
            container.Clear();
        }

        public void NotifyShapeChanged()
        {
            shapeChanged?.Invoke(this);
        }
    }
}
#endif
