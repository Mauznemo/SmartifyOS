using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SmartifyOS.UI.TranslucentImage
{
public static class Extensions
{
    static Mesh fullscreenTriangle;

    /// <summary>
    /// A fullscreen triangle mesh.
    /// </summary>
    static Mesh FullscreenTriangle
    {
        get
        {
            if (fullscreenTriangle != null)
                return fullscreenTriangle;

            fullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };
            fullscreenTriangle.SetVertices(
                new List<Vector3> {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f, 3f,  0f),
                    new Vector3(3f,  -1f, 0f)
                }
            );
            fullscreenTriangle.SetIndices(new[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
            fullscreenTriangle.UploadMeshData(false);

            return fullscreenTriangle;
        }
    }

    public static void BlitCustom(
        this CommandBuffer     cmd,
        RenderTargetIdentifier source,
        RenderTargetIdentifier destination,
        Material               material,
        int                    passIndex,
        bool                   useBuiltin = false
    )
    {
        if (useBuiltin)
            cmd.Blit(source, destination, material, passIndex);
        else if (
            SystemInfo.graphicsShaderLevel >= 30
#if !UNITY_2023_1_OR_NEWER
         && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2
#endif
        )
            cmd.BlitProcedural(source, destination, material, passIndex);
        else
            cmd.BlitFullscreenTriangle(source, destination, material, passIndex);
    }

    public static void BlitFullscreenTriangle(
        this CommandBuffer     cmd,
        RenderTargetIdentifier source,
        RenderTargetIdentifier destination,
        Material               material,
        int                    pass
    )
    {
        cmd.SetGlobalTexture("_MainTex", source);

#if UNITY_2018_2_OR_NEWER
        cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
#else
            cmd.SetRenderTarget(destination);
#endif

        cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass);
    }

    public static void BlitProcedural(
        this CommandBuffer     cmd,
        RenderTargetIdentifier source,
        RenderTargetIdentifier destination,
        Material               material,
        int                    passIndex
    )
    {
        cmd.SetGlobalTexture(ShaderId.MAIN_TEX, source);
        cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.Store,
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.DontCare);
        cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
    }

    /// For normalized screen size
    internal static bool Approximately(this Rect self, Rect other)
    {
        return QuickApproximate(self.x,      other.x)
            && QuickApproximate(self.y,      other.y)
            && QuickApproximate(self.width,  other.width)
            && QuickApproximate(self.height, other.height);
    }

    const float EPSILON01 = 5.9604644e-8f; // different between 1 and largest float < 1

    private static bool QuickApproximate(float a, float b)
    {
        return Mathf.Abs(b - a) < EPSILON01;
    }

    public static Vector4 ToMinMaxVector(this Rect self)
    {
        return new Vector4(
            self.xMin,
            self.yMin,
            self.xMax,
            self.yMax
        );
    }

    public static Vector4 ToVector4(this Rect self)
    {
        return new Vector4(
            self.xMin,
            self.yMin,
            self.width,
            self.height
        );
    }
}
}
