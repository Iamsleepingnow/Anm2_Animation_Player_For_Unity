using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

/// <summary>【固定分辨率渲染（URP管线限定）Constant Resolution Rendering (URP)】</summary>
public class URPConstantResolutionRendering : MonoBehaviour
{
    [InfoBox("渲染缩放范围为0.1~2.0\nRender scale range: 0.1~2.0", EInfoBoxType.Normal)]
    [BoxGroup("Target Reso 目标分辨率")]
    [SerializeField] public int targetWidth = 1920;
    [BoxGroup("Target Reso 目标分辨率")]
    [SerializeField] public int targetHeight = 1080;
    [BoxGroup("Target Reso 目标分辨率"), Range(0, 1)]
    [SerializeField] public float lerpFactor = 0f; // 默认基于宽度

    private UniversalRenderPipelineAsset urpAsset;
    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start() {
        urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset; // 获取URP管线配置 | Get URP pipeline config
        if (urpAsset == null) { return; }
        UpdateRenderScale();
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void Update() {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight) { // 检测分辨率变化 | Check resolution change
            UpdateRenderScale();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    void UpdateRenderScale() {
        if (urpAsset == null) return;
        // 计算基于宽度和高度的独立缩放值 | Calculate independent scale values based on width and height
        float scaleX = (float)targetWidth / Screen.width;
        float scaleY = (float)targetHeight / Screen.height;
        // 使用过渡值进行线性插值 | Linear interpolation using transition values
        float scale = Mathf.Lerp(scaleX, scaleY, lerpFactor);
        // 可选：检查宽高比偏差 | Optional: Check for aspect ratio deviation
        float targetAspect = (float)targetWidth / targetHeight;
        float currentAspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(currentAspect - targetAspect) > 0.01f) {
            // 宽高比不匹配，图像可能拉伸 | Aspect ratio does not match. Image may be stretched.
        }
        // 设置渲染缩放，并限制范围 | Set render scale and limit range
        urpAsset.renderScale = Mathf.Clamp(scale, 0.1f, 2.0f);
        // 渲染分辨率 | Rendering resolution: urpAsset.renderScale
        // 内部分辨率 | Internal resolution: (Screen.width, Screen.height) * urpAsset.renderScale
    }
}
