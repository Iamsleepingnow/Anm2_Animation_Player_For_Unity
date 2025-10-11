using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm动画根图层运行时】Runtime: Anm Root Layer</summary>
    [AddComponentMenu("Anm2Player/Anm Root Layer Runtime")]
    public class AnmRootLayerRuntime : AnmLayerRuntime
    {
        /// <summary>【设置图层帧的属性】Set frame props</summary>
        /// <param name="layerFrame">【非图集图层帧】Null layer frame</param>
        public override AnmLayerRuntime SetFrameProperty(AnmFrame layerFrame) {
            if (layerFrame == null) return this;
            float unitScale = 1f / (DataHandler.PixelSizePerMeter / 100f);
            // 设置gameObject的位置、旋转、缩放
            Vector3 pos = new(
                layerFrame.XPosition / 100f * unitScale,
                layerFrame.YPosition / 100f * -1 * unitScale,
                transform.localPosition.z
                );
            Quaternion rot = Quaternion.Euler(0f, 0f, -layerFrame.Rotation);
            Vector3 scale = new(
                layerFrame.XScale / 100f * unitScale,
                layerFrame.YScale / 100f * unitScale,
                1f
                );
            transform.SetLocalPositionAndRotation(pos, rot);
            transform.localScale = scale;
            return this;
        }
    }
}