using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm动画文件运行时(原始)】Raw: Anm file</summary>
    [System.Serializable]
    public class AnmFileRuntimeRaw
    {
        public TextAsset anmFile = null;
        public List<Texture2D> anmSpriteSheets = new();

        public AnmFileRuntimeRaw(AnmFileRuntime fileRuntime) {
            anmFile = fileRuntime.anmFile;
            anmSpriteSheets = fileRuntime.anmSpriteSheets;
        }

        public AnmFileRuntimeRaw(TextAsset textAsset, List<Texture2D> spriteSheets) {
            anmFile = textAsset;
            anmSpriteSheets = spriteSheets;
        }
    }

    /// <summary>【Anm动画文件运行时】Runtime: Anm file</summary>
    [AddComponentMenu("Anm2Player/Anm File Runtime")]
    public class AnmFileRuntime : MonoBehaviour
    {
        [BoxGroup("Sprite sheets 图集"), Label("Anm File (.xml)")]
        [SerializeField] public TextAsset anmFile = null;
        [BoxGroup("Sprite sheets 图集"), Label("Anm Textures (Texture2D)")]
        [SerializeField] public List<Texture2D> anmSpriteSheets = new();

        /// <summary>【获取原始数据】</summary>
        public AnmFileRuntimeRaw GetRaw() {
            return new AnmFileRuntimeRaw(this);
        }
    }
}