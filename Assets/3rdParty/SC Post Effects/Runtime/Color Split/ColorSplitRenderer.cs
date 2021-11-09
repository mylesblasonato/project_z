﻿#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
#if URP
    public class ColorSplitRenderer : ScriptableRendererFeature
    {
        class ColorSplitRenderPass : PostEffectRenderer<ColorSplit>
        {
            public ColorSplitRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.ColorSplit;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<ColorSplit>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                Material.SetFloat("_Offset", volumeSettings.offset.value / 100);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, (int)volumeSettings.mode.value);
            }
        }

        ColorSplitRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();
        
        public override void Create()
        {
            m_ScriptablePass = new ColorSplitRenderPass(settings);
            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
#endif
}