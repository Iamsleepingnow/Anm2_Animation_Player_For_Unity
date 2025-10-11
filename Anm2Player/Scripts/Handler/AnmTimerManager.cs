using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm动画接口】Anm animation interface</summary>
    public interface IAnmAnimatable
    {
        /// <summary>【帧更新计算】Update frames</summary>
        public void FrameUpdateEvaluate(bool updateFrameIndex);
    }

    /// <summary>【帧计时器管理器】Frame Update Timer Manager</summary>
    [AddComponentMenu("Anm2Player/Anm Timer Manager")]
    public class AnmTimerManager : AnmSingleton<AnmTimerManager>
    {
        /// <summary>【动画数据】Animation data struct</summary>
        private class AnimationData
        {
            public float frameDuration = 0.0f; // 每帧持续时间 | Duration of each frame
            public float currentTime = 0.0f; // 当前帧已持续时间 | Duration of current frame
            public float speed = 1f; // 播放速度 | Play speed
            public bool isPaused = true; // 是否暂停 | Is paused

            public override string ToString() {
                return $"frameDuration: {frameDuration}, currentTime: {currentTime}, speed: {speed}, isPaused: {isPaused}";
            }
        }

        // 使用字典存储动画组件和它们的计时信息 | Use Dictionary to store animation components and their timing information
        private Dictionary<IAnmAnimatable, AnimationData> _animationComponents = new();
        
        [ShowNativeProperty] private int animationCount => _animationComponents.Count;

        /// <summary>【最大帧率 | Maximum frame rate】</summary>
        public float MaxFrameRate {
            get => maxFrameRate;
            set {
                maxFrameRate = Mathf.Max(value, 1f);
            }
        }
        [BoxGroup("Frame Rate 帧率"), Min(1f), Label("Max Frame Rate Limit")]
        [SerializeField] private float maxFrameRate = 60f;
        
        // 固定时间步长更新，避免帧率波动影响动画速度 | Use fixed time step to update animations to avoid frame rate fluctuations affecting animation speed
        private float _accumulatedTime = 0f;
        private float TimeStep => 1f / MaxFrameRate; // 更新频率 | update frequency

        void Update() {
            _accumulatedTime += Time.deltaTime;
            // 按固定时间步长更新动画 | Update animations using fixed time step to avoid frame rate fluctuations affecting animation speed
            while (_accumulatedTime >= TimeStep) {
                _accumulatedTime -= TimeStep;
                UpdateAnimations(TimeStep);
            }
        }

        /// <summary>【更新所有动画】Update all animations</summary>
        /// <param name="deltaTime">【每帧的时长】duration seconds per frame</param>
        private void UpdateAnimations(float deltaTime) {
            // 使用值迭代避免GC分配 | Use value iteration to avoid GC allocation
            foreach (var pair in _animationComponents) {
                var animatable = pair.Key;
                var data = pair.Value;
                if (!data.isPaused) {
                    data.currentTime += deltaTime * data.speed;
                    if (data.currentTime >= data.frameDuration) {
                        data.currentTime -= data.frameDuration;
                        animatable.FrameUpdateEvaluate(true); // 更新动画帧 | Update animation frames
                    }
                }
            }
        }
        
        /// <summary>【更新所有动画的像素尺寸】Update pixel size per meter for all animations</summary>
        /// <param name="pixelSizePerMeter">【每米像素数量】Pixel size per meter</param>
        public void UpdatePixelSizePerMeter(float pixelSizePerMeter) {
            if (AnmDataHandler.Ins == null) return;
            AnmDataHandler.Ins.SetPixelSizePerMeter(pixelSizePerMeter);
            foreach (var pair in _animationComponents) {
                pair.Key.FrameUpdateEvaluate(false); // 更新动画帧 | Update animation frames
            }
        }

        /// <summary>【注册动画计时器】Register animation timers</summary>
        /// <param name="animatable">【动画接口】Interface</param>
        /// <param name="frameRate">【每秒帧率】Frame rate</param>
        /// <param name="speed">【速度乘数】Speed mult</param>
        public void RegisterAnmTimer(IAnmAnimatable animatable, float frameRate, float speed = 1f) {
            if (!_animationComponents.ContainsKey(animatable)) {
                _animationComponents.Add(animatable, new AnimationData {
                    frameDuration = 1f / frameRate,
                    speed = speed,
                    currentTime = 0f,
                    isPaused = true
                });
            }
        }

        /// <summary>【注销动画计时器】Unregister animation timers</summary>
        /// <param name="animatable">【动画接口】Interface</param>
        public void UnregisterAnmTimer(IAnmAnimatable animatable) {
            if (_animationComponents.ContainsKey(animatable)) {
                _animationComponents.Remove(animatable);
            }
        }

        /// <summary>【设置动画播放速度】Set play back speed</summary>
        /// <param name="animatable">【动画接口】Interface</param>
        /// <param name="speed">【速度乘数】Speed mult</param>
        public void SetAnimationSpeed(IAnmAnimatable animatable, float speed) {
            if (_animationComponents.TryGetValue(animatable, out AnimationData data)) {
                data.speed = speed;
            }
        }

        /// <summary>【设置动画帧率】Set frame rate</summary>
        /// <param name="animatable">【动画接口】Interface</param>
        /// <param name="frameRate">【每秒帧率】Frame rate</param>
        public void SetAnimationFrameRate(IAnmAnimatable animatable, float frameRate) {
            if (_animationComponents.TryGetValue(animatable, out AnimationData data)) {
                data.frameDuration = 1f / frameRate;
            }
        }

        /// <summary>【暂停动画】Pause animation</summary>
        /// <param name="animatable">【动画接口】Interface</param>
        public void PauseAnimation(IAnmAnimatable animatable) {
            if (_animationComponents.TryGetValue(animatable, out AnimationData data)) {
                data.isPaused = true;
            }
        }

        /// <summary>【恢复动画】Unpause animation</summary>
        /// <param name="animatable">【动画接口】Interface</param>
        public void ResumeAnimation(IAnmAnimatable animatable) {
            if (_animationComponents.TryGetValue(animatable, out AnimationData data)) {
                data.isPaused = false;
            }
        }
        
        /// <summary>【检查动画是否暂停】</summary>
        /// <param name="animatable">【动画接口】Interface</param>
        /// <returns>【是否暂停】Is paused</returns>
        public bool CheckAnimationPaused(IAnmAnimatable animatable) {
            if (_animationComponents.TryGetValue(animatable, out AnimationData data)) {
                return data.isPaused;
            }
            return true;
        }
    }
}