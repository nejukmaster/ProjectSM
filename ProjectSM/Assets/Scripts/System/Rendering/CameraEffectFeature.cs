using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffectFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Material mat;
    }

        class RenderPass : ScriptableRenderPass{

            private RenderTargetIdentifier colorBuffer, renderBuffer;
            private Material material;
            
            private int renderBufferID = Shader.PropertyToID("_CameraBuffer");

            public RenderPass(CustomPassSettings settings)
            {
                this.material = settings.mat;
                this.renderPassEvent = settings.renderPassEvent;
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
                RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

                cmd.GetTemporaryRT(renderBufferID, descriptor, FilterMode.Point);
                renderBuffer = new RenderTargetIdentifier(renderBufferID);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, new ProfilingSampler("Camera Action Pass")))
                {
                    Blit(cmd, colorBuffer, renderBuffer, material);
                    Blit(cmd, renderBuffer, colorBuffer);
                }
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                if (cmd == null) throw new System.ArgumentNullException("cmd");
                cmd.ReleaseTemporaryRT(renderBufferID);
            }
        }

    private RenderPass renderPass;
    [SerializeField] CustomPassSettings settings;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera) return;
#endif
        renderer.EnqueuePass(renderPass);
    }

    public override void Create()
    {
        renderPass = new RenderPass(settings);
    }
}
