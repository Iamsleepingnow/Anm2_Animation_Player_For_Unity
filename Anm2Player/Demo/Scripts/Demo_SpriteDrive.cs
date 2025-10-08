using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Iamsleepingnow.Anm2Player.Demos
{
    public class Demo_SpriteDrive : MonoBehaviour
    {
        [SerializeField] public Slider sld_SetSpeed;
        [SerializeField] public Slider sld_Progress;
        [SerializeField] public Text txt_EventTip;
        [SerializeField] public Text txt_DebugFlow;
        [SerializeField] public bool debugLog = false;

        private void Start() {
            // 刷新画布 | Refresh canvas
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas != null) {
                canvas.gameObject.SetActive(false);
                LIBRARY.AnmTASK.Delay(1, () => canvas.gameObject.SetActive(true));
            }
        }

        AnmSprite sprite = null; // 当前动画组件 | Current animation component

        #region 按钮事件 Button event
        public void Button_LoadAnmFile() { // 加载动画文件 | Load animation file
            if (sprite != null) {
                Button_Clear();
                Destroy(sprite.gameObject);
            }
            // ------------------------------
            // 允许使用链式编程的方式创建动画组件 | You can create animation component in a chain
            sprite = new GameObject("Sprite test") // 创建一个游戏对象 | Create a game object
                .AddComponent<AnmSprite>() // 添加动画组件 | Add animation component
                .SetTransform(transform, Vector3.zero, Quaternion.identity, Vector3.one) // 设置方位数据 | Set transform data
                .LoadAnmFile(LIBRARY.FilePathType.StreamingAssets, // 在StreamingAssets中加载动画文件 | Load animation file from StreamingAssets
                    "Anm2Player/Anm_CharacterTest.anm2", // 填写相对路径 | Fill in relative path
                    true, // 异步加载 | Async load
                    out AnmFile _file, // 返回动画文件对象 | Return animation file object
                    (anm, _) => { // 动画加载完成后执行 | Execute after animation loaded
                        anm.InitAnimation(true, -1, false); // 初始化动画 | Init animation
                    }
                )
                .SubscribeOnFileLoaded(anm => { // 订阅动画文件加载完成事件 | Subscribe to animation file load complete event
                    if (debugLog) { print("文件加载完成 | File load complete"); }
                    SetTextFlow(txt_DebugFlow, $"▸已加载 Loaded", 10);
                }, false)
                .SubscribeOnFrameUpdate((anm, progress) => { // 订阅动画帧更新事件 | Subscribe to animation frame update event
                    SetProgressBar(sld_Progress, progress, $"[{sprite.GetCurrentFrameIndex()}] ");
                })
                .SubscribeOnEventTriggered((anm, eventName) => { // 订阅动画触发器事件 | Subscribe to animation trigger event
                    SetEventTipText(txt_EventTip, "事件 Event: ", eventName, "#cccc22");
                    if (debugLog) { print($"事件 Event: {eventName}"); }
                    SetTextFlow(txt_DebugFlow, $"事件 Event: {eventName}", 10);
                })
                .SubscribeOnAnimationStarted((anm, animName, animIndex) => { // 订阅动画开始播放事件 | Subscribe to animation start playing event
                    if (debugLog) { print($"动画开始播放 | Animation started playing: {animName}, index: {animIndex}"); }
                    SetTextFlow(txt_DebugFlow, $"▸动画起始 Anim start", 10);
                })
                .SubscribeOnAnimationCompleted((anm, animName, animIndex, isLoop) => { // 订阅动画播放完成事件 | Subscribe to animation play complete event
                    if (debugLog) { print($"动画播放完成 | Animation play complete: {animName}, index: {animIndex}, isLoop: {isLoop}"); }
                    SetTextFlow(txt_DebugFlow, $"▸动画结束 Anim end{(isLoop ? " (循环 Loop)" : "")}", 10);
                })
                .SubscribeOnAnimationChanged((anm, animName, animIndex) => { // 订阅动画切换事件 | Subscribe to animation switch event
                    if (debugLog) { print($"动画切换 | Animation changed: {animName}, index: {animIndex}"); }
                })
                .SubscribeOnFrameIndexChanged((anm, frameIndex) => { // 订阅动画帧索引切换事件 | Subscribe to animation frame index switch event
                    SetProgressBar(sld_Progress, frameIndex / Mathf.Max(0.001f, sprite.GetCurrentAnimationDesc().FrameNum), $"[{sprite.GetCurrentFrameIndex()}] ");
                });
            // ------------------------------
        }

        public void Button_Clear() { // 清空动画信息 | Clear animation info
            if (sprite == null) return;
            // ------------------------------
            sprite.Clear(); // 清空动画信息 | Clear animation info
            // ------------------------------
            if (sld_SetSpeed != null) {
                sld_SetSpeed.value = 1f;
                SetSliderHandleText(sld_SetSpeed, 1.ToString("0.00"));
            }
        }

        public void Button_Play() { // 播放动画 | Play animation
            if (sprite == null) return;
            // ------------------------------
            sprite.Play(); // 播放动画 | Play animation
        }

        public void Button_Pause() { // 暂停动画 | Pause animation
            if (sprite == null) return;
            // ------------------------------
            sprite.Pause(); // 暂停动画 | Pause animation
            //sprite.IsPaused = !sprite.IsPaused; // 暂停动画 | Pause animation
        }

        public void Button_Stop() { // 停止动画 | Stop animation
            if (sprite == null) return;
            // ------------------------------
            sprite.Stop(); // 停止动画，恢复动画进度 | Stop animation, restore animation progress
        }

        public void Button_Reverse() { // 反转动画播放方向 | Reverse animation play direction
            if (sprite == null) return;
            // ------------------------------
            sprite.SetReverse(!sprite.IsReversed); // 反转动画播放方向 | Reverse animation play direction
            // sprite.IsReversed = !sprite.IsReversed; // 反转动画播放方向 | Reverse animation play direction
        }

        public void Button_PreviousFrame() { // 上一帧 | Previous frame
            if (sprite == null) return;
            // ------------------------------
            AnmAnimation animationDesc = sprite.GetCurrentAnimationDesc(); // 获取当前动画描述信息 | Get current animation description information
            if (animationDesc == null) return;
            if (sprite.GetCurrentFrameIndex() <= 0) {
                sprite.SetCurrentFrameIndex(animationDesc.FrameNum - 1); // 获取动画总帧数 | Get animation total frame number
                return;
            }
            sprite.SetCurrentFrameIndex(sprite.GetCurrentFrameIndex() - 1);
            // ------------------------------
        }

        public void Button_NextFrame() { // 下一帧 | Next frame
            if (sprite == null) return;
            // ------------------------------
            AnmAnimation animationDesc = sprite.GetCurrentAnimationDesc(); // 获取当前动画描述信息 | Get current animation description information
            if (animationDesc == null) return;
            if (sprite.GetCurrentFrameIndex() >= animationDesc.FrameNum - 1) { // 获取动画总帧数 | Get animation total frame number
                sprite.SetCurrentFrameIndex(0);
                return;
            }
            sprite.SetCurrentFrameIndex(sprite.GetCurrentFrameIndex() + 1);
            // ------------------------------
        }

        public void Button_PlayFromStart() { // 从开头播放动画 | Play animation from start
            if (sprite == null) return;
            // ------------------------------
            sprite.PlayForwardFromStart(); // 从开头播放动画 | Play animation from start
        }

        public void Button_PlayFromEnd() { // 从末尾播放动画(反转方向) | Play animation from end (reverse direction)
            if (sprite == null) return;
            // ------------------------------
            sprite.PlayBackwardFromEnd(); // 从末尾播放动画(反转方向) | Play animation from end (reverse direction)
        }

        public void Slider_SetSpeedValueChanged(float val) { // 设置播放速度 | Set playback speed
            if (sld_SetSpeed == null) return;
            if (sprite != null) {
                if (sprite.IsInitialized) {
                    // ------------------------------
                    sprite.SetPlayBackSpeed(val); // 设置播放速度 | Set playback speed
                    // ------------------------------
                    SetSliderHandleText(sld_SetSpeed, val.ToString("0.00"));
                }
                else {
                    sld_SetSpeed.value = 1f;
                    SetSliderHandleText(sld_SetSpeed, 1.ToString("0.00"));
                }
            }
            else {
                sld_SetSpeed.value = 1f;
                SetSliderHandleText(sld_SetSpeed, 1.ToString("0.00"));
            }
        }

        public void Button_SetAnimation_0() { // 设置动画索引 0 | Set animation index 0
            if (sprite == null) return;
            // ------------------------------
            sprite.SetCurrentAnimation(0); // 设置当前动画为索引为0的动画 | Set current animation to animation with index 0
        }

        public void Button_SetAnimation_1() { // 设置动画索引 1 | Set animation index 1
            if (sprite == null) return;
            // ------------------------------
            sprite.SetCurrentAnimation(1); // 设置当前动画为索引为1的动画 | Set current animation to animation with index 1
        }

        public void Button_SetAnimation_2() { // 设置动画索引 2 | Set animation index 2
            if (sprite == null) return;
            // ------------------------------
            sprite.SetCurrentAnimation(2); // 设置当前动画为索引为2的动画 | Set current animation to animation with index 2
        }

        public void Button_SetAnimation_3() { // 设置动画索引 3 | Set animation index 3
            if (sprite == null) return;
            // ------------------------------
            sprite.SetCurrentAnimation(3); // 设置当前动画为索引为3的动画 | Set current animation to animation with index 3
        }

        public void Button_SetFrameProgress_00() { // 设置播放进度 0% | Set play progress 0%
            if (sprite == null) return;
            int frameCount = sprite.IsInitialized ? sprite.GetCurrentAnimationDesc().FrameNum : 0;
            // ------------------------------
            sprite.SetCurrentFrameIndex(Mathf.RoundToInt(0.0f * frameCount)); // 设置当前帧索引进度为0% | Set current frame index progress to 0%
        }

        public void Button_SetFrameProgress_20() { // 设置播放进度 20% | Set play progress 20%
            if (sprite == null) return;
            int frameCount = sprite.IsInitialized ? sprite.GetCurrentAnimationDesc().FrameNum : 0;
            // ------------------------------
            sprite.SetCurrentFrameIndex(Mathf.RoundToInt(0.2f * frameCount)); // 设置当前帧索引进度为20% | Set current frame index progress to 20%
        }

        public void Button_SetFrameProgress_40() { // 设置播放进度 40% | Set play progress 40%
            if (sprite == null) return;
            int frameCount = sprite.IsInitialized ? sprite.GetCurrentAnimationDesc().FrameNum : 0;
            // ------------------------------
            sprite.SetCurrentFrameIndex(Mathf.RoundToInt(0.4f * frameCount)); // 设置当前帧索引进度为40% | Set current frame index progress to 40%
        }

        public void Button_SetFrameProgress_60() { // 设置播放进度 60% | Set play progress 60%
            if (sprite == null) return;
            int frameCount = sprite.IsInitialized ? sprite.GetCurrentAnimationDesc().FrameNum : 0;
            // ------------------------------
            sprite.SetCurrentFrameIndex(Mathf.RoundToInt(0.6f * frameCount)); // 设置当前帧索引进度为60% | Set current frame index progress to 60%
        }

        public void Button_SetFrameProgress_80() { // 设置播放进度 80% | Set play progress 80%
            if (sprite == null) return;
            int frameCount = sprite.IsInitialized ? sprite.GetCurrentAnimationDesc().FrameNum : 0;
            // ------------------------------
            sprite.SetCurrentFrameIndex(Mathf.RoundToInt(0.8f * frameCount)); // 设置当前帧索引进度为80% | Set current frame index progress to 80%
        }
        #endregion

        #region 界面方法 UI Helper
        private void SetSliderHandleText(Slider slider, string text) { // 设置滑动条文本 | Set slider text
            if (slider != null) {
                if (slider.handleRect != null) {
                    Text textComp = slider.handleRect.GetComponentInChildren<Text>();
                    if (textComp != null) {
                        textComp.text = text;
                    }
                }
            }
        }
        private void SetProgressBar(Slider bar, float value, string prefix) { // 设置进度条 | Set progress bar
            if (bar != null) {
                bar.value = value;
                Text textComp = bar.handleRect.GetComponentInChildren<Text>();
                if (textComp != null) {
                    textComp.text = $"{prefix}{(value * 100f).ToString("0.00")}%";
                }
            }
        }
        private void SetEventTipText(Text text, string prefix, string content, string color16) { // 设置事件提示文本 | Set event tip text
            if (text != null) {
                text.text = $"{prefix}<color={color16}><b>{content}</b></color>";
            }
        }
        private void SetTextFlow(Text text, string addContent, int maxLine) { // 设置文本流 | Set text flow
            List<string> processedLines = new();
            if (!string.IsNullOrEmpty(text.text)) {
                string[] existingLines = text.text.Split(new string[] { "\n" }, System.StringSplitOptions.None);
                foreach (var line in existingLines) {
                    processedLines.Add(line);
                }
            }
            string[] newLines = addContent.Split(new string[] { "\n" }, System.StringSplitOptions.None);
            foreach (var line in newLines) {
                string randomColor = GenerateRandomColor(new(128, 128, 128), new(255, 255, 255));
                processedLines.Add($"<color={randomColor}>{line}</color>");
            }
            while (processedLines.Count > maxLine) {
                processedLines.RemoveAt(0);
            }
            text.text = string.Join("\n", processedLines);
        }
        private string GenerateRandomColor(Color colorMin, Color colorMax) { // 生成随机颜色 | Generate random color
            int r = Mathf.FloorToInt(Random.Range(colorMin.r, colorMax.r));
            int g = Mathf.FloorToInt(Random.Range(colorMin.g, colorMax.g));
            int b = Mathf.FloorToInt(Random.Range(colorMin.b, colorMax.b));
            return $"#{r:X2}{g:X2}{b:X2}".ToLower();
        }
        #endregion
    }
}