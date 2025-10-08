using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【全局数据字段共享单例】Global data props shared singleton</summary>
    [AddComponentMenu("Anm2Player/Anm Data Handler")]
    public class AnmDataHandler : AnmSingleton<AnmDataHandler>
    {
        public float PixelSizePerMeter {
            get => pixelSizePerMeter;
            set {
                if (!Application.isPlaying) return;
                if (AnmTimerManager.Ins != null) {
                    AnmTimerManager.Ins.UpdatePixelSizePerMeter(pixelSizePerMeter);
                }
            }
        }
        [Tooltip("\n每米可容纳多少像素\nHow many pixels per meter can be accommodated?\n")]
        [BoxGroup("Global 全局"), MinValue(0f), OnValueChanged("__DebugSetPixelSizePerMeter")]
        [SerializeField] private float pixelSizePerMeter = 100f; // 每米像素数量 | Pixel count per meter
        private void __DebugSetPixelSizePerMeter() {
            if (!Application.isPlaying) return;
            if (AnmTimerManager.Ins != null) {
                AnmTimerManager.Ins.UpdatePixelSizePerMeter(pixelSizePerMeter);
            }
        }
        /// <summary>【设置每米像素数量】Set pixel size per meter</summary>
        /// <param name="pixelSizePerMeter">【每米像素数量】Pixel size per meter</param>
        public void SetPixelSizePerMeter(float pixelSizePerMeter) {
            this.pixelSizePerMeter = Mathf.Max(0.0001f, pixelSizePerMeter);
        }

        [Tooltip("\nAnmSprite中每一图集图层之间Z轴排序的距离\nZ-axis sorting distance between sprite sheet layers in AnmSprite\n")]
        [BoxGroup("Global 全局"), MinValue(0.000001f)]
        [SerializeField] public float LayerSortingGap = 0.0001f;  // 图层排序间隔 | Layer sorting gap

        /// <summary>【默认矩形网格】</summary>
        public Mesh RectMesh {
            get {
                if (rectMesh == null) {
                    rectMesh = LIBRARY.CreateQuadMesh(Vector2.one);
                }
                return rectMesh;
            }
            set => rectMesh = value;
        }
        private Mesh rectMesh = null;

        [Tooltip("\nAnmSprite的图集图层网格初始化时所使用的材质模板\nMesh material template of sprite sheet layers when init in AnmSprite\n")]
        [BoxGroup("Material Template 材质模板"), Label("Hidden Material Template")]
        [SerializeField] public Material anmHiddenMaterialTemp = null; // Anm动画播放器的渲染器隐藏使用的材质模板 | Material template used by Anm animation player hidden

        [Tooltip("\nAnmSprite的图集图层网格所使用的材质模板\nMesh material template of sprite sheet layers in AnmSprite\n")]
        [BoxGroup("Material Template 材质模板"), Label("Sprite Material Template")]
        [SerializeField] public Material anmSpriteMaterialTemp = null; // Anm动画播放器的渲染器使用的材质模板 | Material template used by Anm animation player

        [Tooltip("\n着色器引用：基础贴图\nShader reference: Base map texture\n")]
        [BoxGroup("Shader 着色器"), Label("Ref_BaseMap")]
        [SerializeField] public string anmShaderRef_BaseMap = "_BaseMap"; // Anm动画播放器Shader引用-基础贴图 | Shader reference - BaseMap

        [Tooltip("\n着色器引用：裁切位置(左上角)\nShader reference: Crop position(TopLeft corner)\n")]
        [BoxGroup("Shader 着色器"), Label("Ref_CropPosition")]
        [SerializeField] public string anmShaderRef_CropPosition = "_CropPosition"; // Anm动画播放器Shader引用-裁切位置 | Shader reference - CropPosition

        [Tooltip("\n着色器引用：裁切像素大小\nShader reference: Crop pixel size\n")]
        [BoxGroup("Shader 着色器"), Label("Ref_CropSize")]
        [SerializeField] public string anmShaderRef_CropSize = "_CropSize"; // Anm动画播放器Shader引用-裁切尺寸 | Shader reference - CropSize

        [Tooltip("\n着色器引用：锚点偏移\nShader reference: Pivot offset\n")]
        [BoxGroup("Shader 着色器"), Label("Ref_PivotOffset")]
        [SerializeField] public string anmShaderRef_PivotOffset = "_PivotOffset"; // Anm动画播放器Shader引用-锚点偏移 | Shader reference - PivotOffset

        [Tooltip("\n着色器引用：正片叠底色(动画控制)\nShader reference: Color tint (Animation controlled)\n")]
        [BoxGroup("Shader 着色器"), Label("Ref_ColorTint(Anm)")]
        [SerializeField] public string anmShaderRef_ColorTint = "_AnmColorTint"; // Anm动画播放器Shader引用-正片叠底色 | Shader reference - ColorTint

        [Tooltip("\n着色器引用：叠加色(动画控制)\nShader reference: Color offset (Animation controlled)\n")]
        [BoxGroup("Shader 着色器"), Label("Ref_ColorOffset(Anm)")]
        [SerializeField] public string anmShaderRef_ColorOffset = "_AnmColorOffset"; // Anm动画播放器Shader引用-叠加色 | Shader reference - ColorOffset

        [Tooltip("\n着色器引用：可见性\nShader reference: Visibility\n")]
        [BoxGroup("Shader 着色器"), Label("Ref_ISVISIBLE")]
        [SerializeField] public string anmShaderRef_IsVisible = "_ISVISIBLE"; // Anm动画播放器Shader引用-可见性 | Shader reference - Visibility
    }
}