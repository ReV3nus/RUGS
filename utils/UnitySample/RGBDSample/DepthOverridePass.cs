using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.IO;

class DepthOverridePass : CustomPass
{
    public Material depthMaterial;
    public RenderTexture depthRT;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {

//        depthRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.RFloat);
    }

    protected override void Execute(CustomPassContext ctx)
    {
        CommandBuffer cmd = ctx.cmd;
        HDCamera hdCamera = ctx.hdCamera;

        CoreUtils.SetRenderTarget(cmd, depthRT, ClearFlag.Color, Color.black);
        CustomPassUtils.DrawRenderers(ctx, ~0, RenderQueueType.All, depthMaterial);

        //cmd.Blit(depthRT, Shader.GetGlobalTexture("_CameraColorTexture"));
        cmd.Blit(depthRT, ctx.cameraColorBuffer);



        RenderTexture.active = depthRT;
        Texture2D depthImage = new Texture2D(1296, 864, TextureFormat.R16, false);
        depthImage.ReadPixels(new Rect(0, 0, 1296, 864), 0, 0);
        depthImage.Apply();
        byte[] bytes = depthImage.EncodeToPNG();
        File.WriteAllBytes("SampleOutput/tmp.png", bytes);
    }

    protected override void Cleanup()
    {
       // if (depthRT != null)
       //     depthRT.Release();
    }
}
