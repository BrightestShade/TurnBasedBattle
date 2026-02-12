using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable, VolumeComponentMenu("Custom/BodycamFisheye")]
public class BodycamFisheye : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter strength = new ClampedFloatParameter(-0.45f, -1f, 1f);
    public ClampedFloatParameter scaleBuffer = new ClampedFloatParameter(0.9f, 0.5f, 1f);
    public ClampedFloatParameter vignetteStrength = new ClampedFloatParameter(0.85f, 0f, 1f);
    public ClampedFloatParameter vignetteSmoothness = new ClampedFloatParameter(0.7f, 0f, 1f);

    Material m_Material;

    public bool IsActive() => m_Material != null && strength.value != 0f;

    public override CustomPostProcessInjectionPoint injectionPoint =>
        CustomPostProcessInjectionPoint.BeforePostProcess;

    public override void Setup()
    {
        var shader = Shader.Find("Hidden/Shader/BodycamFisheye");
        if (shader != null)
            m_Material = new Material(shader);
    }

    public override void Render(CommandBuffer cmd, HDCamera camera,
        RTHandle source, RTHandle destination)
    {
        if (m_Material == null) return;

        m_Material.SetFloat("_Strength", strength.value);
        m_Material.SetFloat("_ScaleBuffer", scaleBuffer.value);
        m_Material.SetFloat("_VignetteStrength", vignetteStrength.value);
        m_Material.SetFloat("_VignetteSmoothness", vignetteSmoothness.value);

        HDUtils.DrawFullScreen(cmd, m_Material, destination, source);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
