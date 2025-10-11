using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【跳帧等级】Frame dropping level</summary>
    [System.Serializable]
    public class AnmFrameDroppingLevel
    {
        [Range(0, 16)]
        public int JumpFrames = 0;
        [Range(0f, 144f)]
        public float FrameRate = 30f;
    }

    /// <summary>【跳帧等级列表】Frame dropping level list</summary>
    [CreateAssetMenu(fileName = "NewAnmFrameDroppingLevels", menuName = "Anm2Player/Anm Frame Dropping Levels", order = 0)]
    public class AnmFrameDroppingLevels : ScriptableObject
    {
        [ResizableTextArea, ReadOnly]
        [SerializeField]
        private string _info = "\n跳帧等级表 | Frame Dropping Levels (FDL)\n"
            + "\n用于描述Anm动画渲染的优化等级。 | A description of the optimization level for Anm animation rendering."
            + "\nJump Frames: 跳帧数，动画更新时跳过的帧数量。 | The number of frames skipped when the animation updates."
            + "\nFrame Rate: 帧率，每秒帧数。 | Frame rate, frames per second.\n"; // 提示信息

        /// <summary>【跳帧等级列表】Frame dropping level list</summary>
        [InfoBox("跳帧等级表\nFrame dropping levels config", EInfoBoxType.Normal), ReorderableList]
        [SerializeField] public List<AnmFrameDroppingLevel> levels = new();

        /// <summary>【获取指定等级的跳帧等级】Get frame dropping level by level</summary>
        /// <param name="level">【等级，数列序数】Level, list index</param>
        /// <param name="jumpFrames">【跳帧数】Jump frames count</param>
        /// <param name="frameRate">【帧率】Frame rate</param>
        public AnmFrameDroppingLevels GetFrameDroppingLevel(int level, out int jumpFrames, out float frameRate) {
            if (levels.Count <= 0) {
                jumpFrames = 0;
                frameRate = 0f;
                return this;
            }
            level = Mathf.Clamp(level, 0, levels.Count - 1);
            jumpFrames = levels[level].JumpFrames;
            frameRate = levels[level].FrameRate;
            return this;
        }
    }
}