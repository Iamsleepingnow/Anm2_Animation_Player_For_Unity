using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Iamsleepingnow.Anm2Player.Demos
{
    [RequireComponent(typeof(Text))]
    public class Demo_FPSCounter : MonoBehaviour
    {
        private Text Txt_FPSCounter => GetComponent<Text>();

        // 帧率计算相关变量 | FPS calculation related variables
        private int frameCount = 0;
        private float elapsedTime = 0f;
        private float currentFPS = 0f; // 当前帧率 | Current FPS

        [SerializeField] private float updateInterval = 0.5f; // 帧率更新频率（秒） | FPS update interval (seconds)

        void Update() {
            frameCount++;
            elapsedTime += Time.unscaledDeltaTime;
            if (elapsedTime >= updateInterval) {
                CalculateFPS();
            }
        }

        void CalculateFPS() {
            // 计算当前FPS | Calculate current FPS
            currentFPS = frameCount / elapsedTime;
            //
            Txt_FPSCounter.text = $"{Mathf.RoundToInt(currentFPS)} f/s";
            //
            // 重置计数器 | Reset counter
            frameCount = 0;
            elapsedTime = 0f;
        }

        /// <summary>【设置更新间隔】Set update interval</summary>
        /// <param name="interval">【间隔时间】Interval time</param>
        public void SetUpdateInterval(float interval) {
            updateInterval = Mathf.Max(0.1f, interval);
        }
    }
}