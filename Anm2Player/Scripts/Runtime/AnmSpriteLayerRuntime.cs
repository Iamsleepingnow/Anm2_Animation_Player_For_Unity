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

        /// <summary>【动画材质】Material</summary>
        [ShowNativeProperty]
        public Material SelfMeshMaterial {
            get {
                if (selfMeshMaterial == null) {
                    selfMeshMaterial = new(DataHandler.anmSpriteMaterialTemp.shader);
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
                if (SelfMeshMaterial != null) {
                    SelfMeshMaterial.SetKeyword(new LocalKeyword(
                        DataHandler.anmSpriteMaterialTemp.shader, DataHandler.anmShaderRef_IsVisible), IsVisible); // 可见性
                }
            }
        }
        private bool isVisible = true;

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
        private AnmSpriteLayerRuntime SetSpriteLayerFrameProperty(MeshRenderer renderer, Texture2D tex, AnmLayerFrame layerFrame, bool visible) {
            if (renderer == null || layerFrame == null) return this;
            IsVisible = tex != null;
            if (tex == null) return this;
            if (renderer.sharedMaterial != SelfMeshMaterial) {
                renderer.sharedMaterial = SelfMeshMaterial;
            }
            // 设置Shader属性
            SelfMeshMaterial.SetTexture(DataHandler.anmShaderRef_BaseMap, tex); // 基础贴图
            SelfMeshMaterial.SetVector(DataHandler.anmShaderRef_CropPosition, new Vector4(layerFrame.XCrop, layerFrame.YCrop, 0, 0)); // 裁切位置
            SelfMeshMaterial.SetVector(DataHandler.anmShaderRef_CropSize, new Vector4(layerFrame.Width, layerFrame.Height, 0, 0)); // 裁切尺寸
            SelfMeshMaterial.SetVector(DataHandler.anmShaderRef_PivotOffset, new Vector4(layerFrame.XPivot, layerFrame.YPivot, 0, 0)); // 锚点偏移
            SelfMeshMaterial.SetColor(DataHandler.anmShaderRef_ColorTint, new Color(
                layerFrame.RedTint / 255f, layerFrame.GreenTint / 255f, layerFrame.BlueTint / 255f, layerFrame.AlphaTint / 255f)); // 正片叠底色
            SelfMeshMaterial.SetColor(DataHandler.anmShaderRef_ColorOffset, new Color(
                layerFrame.RedOffset / 255f, layerFrame.GreenOffset / 255f, layerFrame.BlueOffset / 255f, 0)); // 叠加色
            SelfMeshMaterial.SetKeyword(new LocalKeyword(DataHandler.anmSpriteMaterialTemp.shader, DataHandler.anmShaderRef_IsVisible), layerFrame.Visible && IsVisible && visible); // 可见性
            return this;
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
            Vector3 pixelBiasToTopLeft = new(0.5f, -0.5f, 0f); // 距离左上角点的相对偏移
            Vector3 pixelPivotProportionalOffset = new(layerFrame.XPivot / layerFrame.Width * -1, layerFrame.YPivot / layerFrame.Height, 0f); // 锚点的相对偏移
            meshFilter.mesh.bounds = new(
                pixelBiasToTopLeft + pixelPivotProportionalOffset,
                new Vector3(1, 1, 1) + new Vector3(marginPixelSize * 2f, marginPixelSize * 2f, 0f));
            return this;
        }

        /// <summary>【重置MeshFilter及渲染器的渲染边界】Reset mesh filter and mesh renderer's rendering bounds</summary>
        public AnmSpriteLayerRuntime ResetLayerBounds() {
            if (SelfMeshRenderer == null) return this;
            MeshFilter meshFilter = SelfMeshRenderer.gameObject.GetComponentAroundOrAdd<MeshFilter>();
            if (meshFilter.mesh == null) return this;
            meshFilter.mesh.bounds = new(Vector3.zero, Vector3.zero);
            return this;
        }
    }
}