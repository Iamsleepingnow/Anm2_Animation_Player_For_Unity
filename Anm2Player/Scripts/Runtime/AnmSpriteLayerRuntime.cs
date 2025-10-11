using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm动画图集图层运行时】Runtime: Anm Sprite Layer</summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [AddComponentMenu("Anm2Player/Anm Sprite Layer Runtime")]
    public class AnmSpriteLayerRuntime : AnmLayerRuntime
    {
        /// <summary>【Mesh网格渲染器】Mesh Renderer</summary>
        [ShowNativeProperty]
        public MeshRenderer SelfMeshRenderer { get { return gameObject.GetComponentAroundOrAdd<MeshRenderer>(); } }

        /// <summary>【着色器质量】Shader quality</summary>
        public AnmShaderQuality ShaderQuality {
            get => shaderQuality;
            set {
                ShaderQuality = value;
                SetShaderQuality(ShaderQuality);
            }
        }
        private AnmShaderQuality shaderQuality = AnmShaderQuality.Lit_Transparent;

        /// <summary>【动画材质】Material</summary>
        [ShowNativeProperty]
        public Material SelfMeshMaterial {
            get {
                if (selfMeshMaterial == null) {
                    selfMeshMaterial = new(DataHandler.anmSpriteMaterialTemp_LitTransparent.shader);
                }
                return selfMeshMaterial;
            }
        }
        private Material selfMeshMaterial = null;


        /// <summary>【显示状态】Is visible</summary>
        [ShowNativeProperty]
        public bool IsVisible {
            get => isVisible;
            set {
                isVisible = value;
            }
        }
        private bool isVisible = true;

        private Texture2D _tex = null;
        private Vector4 _cropData = Vector4.zero;
        private Vector4 _pivotOffset = Vector4.zero;
        private Color _colorTint = Color.white;
        private Color _colorOffset = Color.black;
        private Vector3 _pixelBiasToTopLeft = new(0.5f, -0.5f, 0); // 距离左上角点的相对偏移
        private Bounds _meshFilterBounds = new(Vector3.zero, Vector3.zero);

        /// <summary>【设置材质的着色器质量】Set shader quality</summary>
        /// <param name="shaderQuality">【着色器质量】Shader quality</param>
        public AnmSpriteLayerRuntime SetShaderQuality(AnmShaderQuality shaderQuality) {
            SelfMeshMaterial.shader = shaderQuality switch {
                AnmShaderQuality.Lit_Transparent => DataHandler.anmSpriteMaterialTemp_LitTransparent.shader,
                AnmShaderQuality.Lit_Opaque => DataHandler.anmSpriteMaterialTemp_LitOpaque.shader,
                AnmShaderQuality.Unlit_Transparent => DataHandler.anmSpriteMaterialTemp_UnlitTransparent.shader,
                AnmShaderQuality.Unlit_Opaque => DataHandler.anmSpriteMaterialTemp_UnlitOpaque.shader,
                _ => DataHandler.anmSpriteMaterialTemp_LitTransparent.shader,
            };
            return this;
        }

        /// <summary>【设置图层帧的属性】Set sprite layer props</summary>
        /// <param name="tex">【图集图像】Texture</param>
        /// <param name="layerFrame">【图集图层帧】Sprite layer frame</param>
        /// <param name="visible">【可见性】Visible</param>
        /// <param name="updateBounds">【更新边界】Update bounds</param>
        public AnmSpriteLayerRuntime SetLayerFrameProperty(Texture2D tex, AnmLayerFrame layerFrame, bool visible, bool updateBounds) {
            if (layerFrame == null) return this;
            SetSpriteLayerFrameProperty(SelfMeshRenderer, tex, layerFrame, visible);
            SetFrameProperty(layerFrame);
            transform.localScale = new Vector3(layerFrame.Width * transform.localScale.x, layerFrame.Height * transform.localScale.y, 1f) / 100f;
            if (updateBounds) {
                SetLayerBounds(layerFrame, 0f); // 设置图层边界
            }
            return this;
        }

        /// <summary>【设置Mesh渲染器属性为图层帧的属性】Set mesh renderer from sprite layer frame</summary>
        /// <param name="renderer">【渲染器】Mesh renderer</param>
        /// <param name="tex">【图集图像】Texture</param>
        /// <param name="layerFrame">【图集图层帧】Sprite layer frame</param>
        /// <param name="visible">【可见性】Visible</param>
        private AnmSpriteLayerRuntime SetSpriteLayerFrameProperty(MeshRenderer renderer, Texture2D tex, AnmLayerFrame layerFrame, bool visible) {
            if (renderer == null || layerFrame == null) return this;
            IsVisible = tex != null;
            if (tex == null) return this;
            bool isVisible = layerFrame.Visible && IsVisible && visible;
            if (isVisible) {
                if (renderer.sharedMaterial != SelfMeshMaterial) {
                    renderer.sharedMaterial = SelfMeshMaterial;
                }
            }
            else { return this; }
            //
            if (_tex != tex) {
                _tex = tex;
                SelfMeshMaterial.SetTexture(DataHandler.anmShaderRef_BaseMap, tex); // 基础贴图
            }
            if (!_cropData.IsEqual(layerFrame.XCrop, layerFrame.YCrop, layerFrame.Width, layerFrame.Height)) {
                _cropData.Set(layerFrame.XCrop, layerFrame.YCrop, layerFrame.Width, layerFrame.Height);
                SelfMeshMaterial.SetVector(DataHandler.anmShaderRef_CropData, _cropData); // 裁切数据
            }
            if (!_pivotOffset.IsEqual(layerFrame.XPivot, layerFrame.YPivot, 0, 0)) {
                _pivotOffset.Set(layerFrame.XPivot, layerFrame.YPivot, 0, 0);
                SelfMeshMaterial.SetVector(DataHandler.anmShaderRef_PivotOffset, _pivotOffset); // 锚点偏移
            }
            if (!_colorTint.IsEqual(layerFrame.RedTint / 255f, layerFrame.GreenTint / 255f, layerFrame.BlueTint / 255f, layerFrame.AlphaTint / 255f)) {
                SetColorTint(layerFrame.RedTint / 255f, layerFrame.GreenTint / 255f, layerFrame.BlueTint / 255f, layerFrame.AlphaTint / 255f);
                SelfMeshMaterial.SetColor(DataHandler.anmShaderRef_ColorTint, _colorTint); // 正片叠底色
            }
            if (!_colorOffset.IsEqual(layerFrame.RedOffset / 255f, layerFrame.GreenOffset / 255f, layerFrame.BlueOffset / 255f, 0)) {
                SetColorOffset(layerFrame.RedOffset / 255f, layerFrame.GreenOffset / 255f, layerFrame.BlueOffset / 255f, 0);
                SelfMeshMaterial.SetColor(DataHandler.anmShaderRef_ColorOffset, _colorOffset); // 叠加色
            }
            return this;
        }

        // 使用固定颜色变量，可以减少内存分配 | Use fixed color variable to reduce memory allocation
        private void SetColorTint(float r, float g, float b, float a) {
            _colorTint.r = r; _colorTint.g = g; _colorTint.b = b; _colorTint.a = a;
        }
        private void SetColorOffset(float r, float g, float b, float a) {
            _colorOffset.r = r; _colorOffset.g = g; _colorOffset.b = b; _colorOffset.a = a;
        }

        /// <summary>【设置图层的渲染排序】Set frame ordering layer</summary>
        /// <param name="id">【渲染队列ID】Ordering layer ID</param>
        public AnmSpriteLayerRuntime SetLayerSortingId(int id) {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, id * -1 * DataHandler.LayerSortingGap);
            return this;
        }

        /// <summary>【设置MeshFilter及渲染器的渲染边界】Set mesh filter and mesh renderer's rendering bounds</summary>
        /// <param name="layerFrame">【图集图层帧】Sprite layer frame</param>
        /// <param name="margin">【边界厚度】border thick</param>
        public AnmSpriteLayerRuntime SetLayerBounds(AnmLayerFrame layerFrame, float margin) {
            if (layerFrame == null) return this;
            MeshFilter meshFilter = SelfMeshRenderer.gameObject.GetComponentAroundOrAdd<MeshFilter>();
            if (meshFilter.mesh == null) return this;
            float marginPixelSize = margin / 100f;
            layerFrame.Width = Mathf.Max(layerFrame.Width, 0.01f); // 防止宽为0
            layerFrame.Height = Mathf.Max(layerFrame.Height, 0.01f); // 防止高为0
            Vector3 pixelPivotProportionalOffset = new(layerFrame.XPivot / layerFrame.Width * -1, layerFrame.YPivot / layerFrame.Height, 0f); // 锚点的相对偏移
            _meshFilterBounds.center = _pixelBiasToTopLeft + pixelPivotProportionalOffset;
            _meshFilterBounds.size = Vector3.one + new Vector3(marginPixelSize * 2f, marginPixelSize * 2f, 0f);
            meshFilter.mesh.bounds = _meshFilterBounds;
            return this;
        }

        /// <summary>【重置MeshFilter及渲染器的渲染边界】Reset mesh filter and mesh renderer's rendering bounds</summary>
        public AnmSpriteLayerRuntime ResetLayerBounds() {
            if (SelfMeshRenderer == null) return this;
            MeshFilter meshFilter = SelfMeshRenderer.gameObject.GetComponentAroundOrAdd<MeshFilter>();
            if (meshFilter.mesh == null) return this;
            _meshFilterBounds.center = Vector3.zero;
            _meshFilterBounds.size = Vector3.zero;
            meshFilter.mesh.bounds = _meshFilterBounds;
            return this;
        }
    }
}