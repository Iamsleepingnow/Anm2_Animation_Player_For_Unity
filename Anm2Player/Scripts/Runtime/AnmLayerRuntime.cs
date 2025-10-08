using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm动画图层运行时】Runtime: Anm Layer</summary>
    public abstract class AnmLayerRuntime : MonoBehaviour
    {
        /// <summary>【图层名称】Layer name</summary>
        [Tooltip("\n图层名称\nLayer name\n")]
        [BoxGroup("Layer Info 图层信息")]
        [SerializeField] public string LayerName = string.Empty;

        /// <summary>【图层ID】Layer id</summary>
        [Tooltip("\n图层Id号\nLayer ID\n")]
        [BoxGroup("Layer Info 图层信息")]
        [SerializeField] public int LayerId = -1;

        /// <summary>【数据单例】Data handler</summary>
        protected AnmDataHandler DataHandler {
            get {
                if (dataHandler == null) {
                    dataHandler = AnmDataHandler.Ins;
                }
                return dataHandler;
            }
        }
        private AnmDataHandler dataHandler = null;

        /// <summary>【设置父级】Set parent</summary>
        /// <param name="parent">【父级】parent</param>
        public AnmLayerRuntime SetParent(Transform parent) {
            transform.SetParent(parent);
            return this;
        }

        /// <summary>【设置方位数据】Set transform props</summary>
        /// <param name="parent">【父级】Transform parent</param>
        /// <param name="localPosition">【本地坐标】Local position</param>
        /// <param name="localRotation">【本地旋转】Local rotation</param>
        /// <param name="localScale">【本地缩放】Local scale</param>
        public AnmLayerRuntime SetTransform(Vector3 localPosition = default, Quaternion localRotation = default, Vector3 localScale = default) {
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
            return this;
        }

        /// <summary>【设置图层帧的属性】Set frame props</summary>
        /// <param name="layerFrame">【非图集图层帧】Null layer frame</param>
        public virtual AnmLayerRuntime SetFrameProperty(AnmFrame layerFrame) {
            if (layerFrame == null) return this;
            // 设置gameObject的位置、旋转、缩放
            Vector3 pos = new(
                layerFrame.XPosition / 100f,
                layerFrame.YPosition / 100f * -1,
                transform.localPosition.z
                );
            Quaternion rot = Quaternion.Euler(0f, 0f, -layerFrame.Rotation);
            Vector3 scale = new(
                layerFrame.XScale / 100f,
                layerFrame.YScale / 100f,
                1f
                );
            transform.SetLocalPositionAndRotation(pos, rot);
            transform.localScale = scale;
            return this;
        }

        /// <summary>【设置图层名称】Set layer name</summary>
        /// <param name="layerName">【名称】Name</param>
        public AnmLayerRuntime SetLayerName(string layerName) {
            LayerName = layerName;
            return this;
        }

        /// <summary>【设置图层显示名称】Set layer display name</summary>
        /// <param name="displayName">【显示名称】Display name</param>
        public AnmLayerRuntime SetLayerDisplayName(string displayName) {
            gameObject.name = displayName;
            return this;
        }

        /// <summary>【设置图层ID】Set layer id</summary>
        /// <param name="id">【ID序数】</param>
        public AnmLayerRuntime SetLayerId(int id) {
            LayerId = id;
            return this;
        }

        /// <summary>【设置图层隐藏】Set mute</summary>
        /// <param name="isMute">【是否隐藏】Is mute</param>
        public AnmLayerRuntime SetMute(bool isMute) {
            gameObject.SetActive(!isMute);
            return this;
        }

        /// <summary>【切换图层隐藏】Toggle mute</summary>
        public AnmLayerRuntime ToggleMute() {
            gameObject.SetActive(!gameObject.activeSelf);
            return this;
        }
    }
}