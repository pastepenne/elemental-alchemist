#if GRIFFIN && !GRIFFIN_EXCLUDE_SUMMIT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Griffin.ErosionTool
{
    /// <summary>
    /// A component to simulate thermal and hydraulic erosion to the terrain, also apply texturing based on simulation data.
    /// The area of effective was determined using its Transform component.
    /// </summary>
    [ExecuteInEditMode]
    public class GErosionSimulator : MonoBehaviour
    {
        [SerializeField]
        private int groupId;
        /// <summary>
        /// The terrain group to perform simulation on.
        /// </summary>
        public int GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                groupId = value;
            }
        }

        [SerializeField]
        private bool enableTerrainMask;
        /// <summary>
        /// Use terrain mask texture (R) to lock a region from being edited.
        /// </summary>
        public bool EnableTerrainMask
        {
            get
            {
                return enableTerrainMask;
            }
            set
            {
                enableTerrainMask = value;
            }
        }

        [SerializeField]
        private AnimationCurve falloffCurve;
        /// <summary>
        /// Fade out erosion intensity from the center.
        /// </summary>
        public AnimationCurve FalloffCurve
        {
            get
            {
                if (falloffCurve == null)
                {
                    falloffCurve = AnimationCurve.Linear(0, 1, 1, 1);
                }
                return falloffCurve;
            }
            set
            {
                falloffCurve = value;
            }
        }

        [SerializeField]
        private float detailLevel;
        /// <summary>
        /// Simulation quality. Higher value will use higher resolution canvas, more micro detail but slower. Lower value is faster, producing larger erosion features but less micro details.
        /// </summary>
        public float DetailLevel
        {
            get
            {
                return detailLevel;
            }
            set
            {
                detailLevel = Mathf.Clamp(value, 0.2f, 2f);
            }
        }

        [SerializeField]
        private GHydraulicErosionConfigs hydraulicConfigs;
        /// <summary>
        /// Settings for hydraulic erosion effect.
        /// </summary>
        public GHydraulicErosionConfigs HydraulicConfigs
        {
            get
            {
                return hydraulicConfigs;
            }
            set
            {
                hydraulicConfigs = value;
            }
        }

        [SerializeField]
        private GThermalErosionConfigs thermalConfigs;
        /// <summary>
        /// Settings for thermal erosion effect.
        /// </summary>
        public GThermalErosionConfigs ThermalConfigs
        {
            get
            {
                return thermalConfigs;
            }
            set
            {
                thermalConfigs = value;
            }
        }

        [SerializeField]
        private GErosionTexturingConfigs texturingConfigs;
        /// <summary>
        /// Settings for texturing based on simulation data.
        /// </summary>
        public GErosionTexturingConfigs TexturingConfigs
        {
            get
            {
                return texturingConfigs;
            }
            set
            {
                texturingConfigs = value;
            }
        }

        private RenderTexture simulationData;
        public RenderTexture SimulationData
        {
            get
            {
                return simulationData;
            }
        }

        private RenderTexture simulationMask;
        public RenderTexture SimulationMask
        {
            get
            {
                return simulationMask;
            }
        }

        private RenderTexture erosionMap;
        public RenderTexture ErosionMap
        {
            get
            {
                return erosionMap;
            }
        }

        private Texture2D falloffTexture;
        public Texture2D FalloffTexture
        {
            get
            {
                return falloffTexture;
            }
        }

        private Vector3 bounds;
        /// <summary>
        /// The area of effective.
        /// </summary>
        public Vector3 Bounds
        {
            get
            {
                return bounds;
            }
        }

        private void Reset()
        {
            groupId = -1;
            enableTerrainMask = false;
            falloffCurve = AnimationCurve.Linear(0, 1, 1, 0);
            detailLevel = 0.5f;
            hydraulicConfigs = new GHydraulicErosionConfigs();
            thermalConfigs = new GThermalErosionConfigs();
            texturingConfigs = new GErosionTexturingConfigs();
        }

        private void OnEnable()
        {
            UpdateFalloffTexture();
            Initialize();
        }

        private void OnDisable()
        {
            CleanUp();
        }

        /// <summary>
        /// Call this function first, before any simulation.
        /// This will update the boundary as well as reset/refresh simulation data and input.
        /// </summary>
        public void Initialize()
        {
            GErosionInitializer initializer = new GErosionInitializer(this);
            initializer.Init(ref bounds, ref simulationData, ref simulationMask, ref erosionMap);
        }

        /// <summary>
        /// Call this function to clean up resources, once you've applied simulation result down to the terrains.
        /// </summary>
        public void CleanUp()
        {
            if (simulationData != null)
            {
                simulationData.Release();
                simulationData = null;
            }

            if (simulationMask != null)
            {
                simulationMask.Release();
                simulationMask = null;
            }

            if (erosionMap != null)
            {
                erosionMap.Release();
                erosionMap = null;
            }

            if (falloffTexture != null)
            {
                GUtilities.DestroyObject(falloffTexture);
            }
        }

        /// <summary>
        /// Perform hydraulic erosion. The result was stored in intermediate textures, not yet applied to the terrains belows. You can call this function many times before calling ApplyXXX()
        /// </summary>
        public void SimulateHydraulicErosion()
        {
            GHydraulicEroder eroder = new GHydraulicEroder(this);
            eroder.Init();

            int iteration = HydraulicConfigs.IterationCount;
            for (int i = 0; i < iteration; ++i)
            {
                float t = i * 1.0f / iteration;
                eroder.WaterSourceAmount = HydraulicConfigs.WaterSourceAmount * HydraulicConfigs.WaterSourceOverTime.Evaluate(t) * HydraulicConfigs.WaterSourceMultiplier;
                eroder.RainRate = HydraulicConfigs.RainRate * HydraulicConfigs.RainOverTime.Evaluate(t) * HydraulicConfigs.RainMultiplier;
                eroder.FlowRate = HydraulicConfigs.FlowRate * HydraulicConfigs.FlowOverTime.Evaluate(t) * HydraulicConfigs.FlowMultiplier;
                eroder.ErosionRate = HydraulicConfigs.ErosionRate * HydraulicConfigs.ErosionOverTime.Evaluate(t) * HydraulicConfigs.ErosionMultiplier;
                eroder.DepositionRate = HydraulicConfigs.DepositionRate * HydraulicConfigs.DepositionOverTime.Evaluate(t) * HydraulicConfigs.DepositionMultiplier;
                eroder.EvaporationRate = HydraulicConfigs.EvaporationRate * HydraulicConfigs.EvaporationOverTime.Evaluate(t) * HydraulicConfigs.EvaporationMultiplier;
                eroder.Bounds = Bounds;

                eroder.Simulate();
            }

            eroder.Dispose();
        }

        /// <summary>
        /// Perform thermal erosion. The result was stored in intermediate textures, not yet applied to the terrains belows. You can call this function many times before calling ApplyXXX()
        /// </summary>
        public void SimulateThermalErosion()
        {
            GThermalEroder eroder = new GThermalEroder(this);
            eroder.Init();

            int iteration = ThermalConfigs.IterationCount;
            for (int i = 0; i < iteration; ++i)
            {
                float t = i * 1.0f / iteration;
                eroder.MaskMap = SimulationMask;
                eroder.ErosionRate = ThermalConfigs.ErosionRate * ThermalConfigs.ErosionOverTime.Evaluate(t) * ThermalConfigs.ErosionMultiplier;
                eroder.RestingAngle = ThermalConfigs.RestingAngle * ThermalConfigs.RestingAngleOverTime.Evaluate(t) * ThermalConfigs.RestingAngleMultiplier;
                eroder.Bounds = Bounds;

                eroder.Simulate();
            }

            eroder.Dispose();
        }

        /// <summary>
        /// Apply geometry changes from simulation data to the terrains below.
        /// </summary>
        public void ApplyGeometry()
        {
            GErosionApplier applier = new GErosionApplier(this);
            applier.ApplyGeometry();
        }

        /// <summary>
        /// Apply texturing from simulation data to the terrains below.
        /// </summary>
        public void ApplyTexture()
        {
            GErosionApplier applier = new GErosionApplier(this);
            if (TexturingConfigs.TexturingMode == GErosionTexturingConfigs.GMode.Splat)
            {
                applier.ApplySplat();
            }
            else
            {
                applier.ApplyAMS();
            }
        }

        /// <summary>
        /// Get a list of terrains intersected with this component's area of effective."
        /// </summary>
        /// <returns>The intersected terrains.</returns>
        public List<GStylizedTerrain> GetIntersectedTerrains()
        {
            Vector3[] worldCorner = GetQuad();
            return GUtilities.ExtractTerrainsFromOverlapTest(GCommon.OverlapTest(GroupId, worldCorner));
        }

        /// <summary>
        /// Get 4 vertices of a world space quad of the area of effective.
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetQuad()
        {
            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Vector3[] quad = new Vector3[4]
            {
                matrix.MultiplyPoint(new Vector3(-0.5f, 0, -0.5f)),
                matrix.MultiplyPoint(new Vector3(-0.5f, 0, 0.5f)),
                matrix.MultiplyPoint(new Vector3(0.5f, 0, 0.5f)),
                matrix.MultiplyPoint(new Vector3(0.5f, 0, -0.5f))
            };

            return quad;
        }

        /// <summary>
        /// Regenerate the falloff texture from falloff curve.
        /// </summary>
        public void UpdateFalloffTexture()
        {
            if (falloffTexture != null)
            {
                GUtilities.DestroyObject(falloffTexture);
            }
            falloffTexture = GCommon.CreateTextureFromCurve(FalloffCurve, 2048, 1);
        }
    }
}
#endif
