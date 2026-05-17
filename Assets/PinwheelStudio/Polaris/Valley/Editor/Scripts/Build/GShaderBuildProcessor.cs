#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using TerrainRenderingSettings = Pinwheel.Griffin.GRuntimeSettings.TerrainRenderingSettings;
using TerrainMaterialTemplate = Pinwheel.Griffin.GRuntimeSettings.TerrainRenderingSettings.TerrainMaterialTemplate;
using FoliageRenderingSettings = Pinwheel.Griffin.GRuntimeSettings.FoliageRenderingSettings;
using UnityEditor.Rendering;

namespace Pinwheel.Griffin.Build
{
    public class GShaderBuildProcessor : IOrderedCallback, IPreprocessShaders
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            string shaderName = shader.name;
            bool isPolarisBiRPShader = shaderName.StartsWith("Polaris/BuiltinRP/");
            bool isPolarisURPShader = shaderName.StartsWith("Polaris/URP/");

            bool willStrip = false;
            if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Builtin &&
                isPolarisURPShader)
            {
                willStrip = true;
            }

            if (GCommon.CurrentRenderPipeline == GRenderPipelineType.Universal &&
                isPolarisBiRPShader)
            {
                willStrip = true;
            }

            if (willStrip)
            {
                Debug.Log($"POLARIS: Stripping shader {shaderName}");
                data.Clear();
            }
        }
    }
}
#endif
