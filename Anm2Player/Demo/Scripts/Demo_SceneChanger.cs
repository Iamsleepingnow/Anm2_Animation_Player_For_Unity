using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Iamsleepingnow.Anm2Player.Demos
{
    public class Demo_SceneChanger : MonoBehaviour
    {
        [Scene]
        [SerializeField] public string nextScene;

        /// <summary>【按钮：切换场景】Button: Change Scene</summary>
        public void Button_ChangeScene() {
            if (!string.IsNullOrEmpty(nextScene)) {
                SceneManager.LoadScene(nextScene);
            }
        }

        /// <summary>【按钮：退出游戏】Button: Quit</summary>
        public void Button_QuitScene() {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}