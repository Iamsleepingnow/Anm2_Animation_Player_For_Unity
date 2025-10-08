using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【全局综合库】Global Library for Anm2Player</summary>
    public static partial class LIBRARY
    {
        #region Task 外部协程
        /// <summary>【外部协程】</summary>
        public static class AnmTASK
        {
            //内部类
            private class TaskBehaviour : MonoBehaviour { }
            private static TaskBehaviour task;

            //静态构造函数
            static AnmTASK()
            {
                GameObject gameObj = new GameObject("AnmTaskManager");
                GameObject.DontDestroyOnLoad(gameObj);
                task = gameObj.AddComponent<TaskBehaviour>();
            }

            /// <summary>【开始协程】</summary>
            /// <param name="routine">【协程】</param>
            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                if (routine == null)
                    return null;
                return task.StartCoroutine(routine);
            }

            /// <summary>【中止协程】</summary>
            /// <param name="routine">【协程】</param>
            public static void StopCoroutine(ref Coroutine routine)
            {
                if (routine != null)
                {
                    task.StopCoroutine(routine);
                    routine = null;
                }
            }

            /// <summary>【按秒数延迟】</summary>
            /// <param name="delaySeconds">【秒】</param>
            /// <param name="action">【事件】</param>
            public static Coroutine Delay(float delaySeconds, Action action)
            {
                if (action == null || task == null)
                    return null;
                return task.StartCoroutine(StartDelayToInvokeBySecond(action, delaySeconds));
            }
            /// <summary>【按帧数延迟】</summary>
            /// <param name="delaySeconds">【帧】</param>
            /// <param name="action">【事件】</param>
            public static Coroutine Delay(int delayFrames, Action action)
            {
                if (action == null || task == null)
                    return null;
                return task.StartCoroutine(StartDelayToInvokeByFrame(action, delayFrames));
            }

            private static IEnumerator StartDelayToInvokeBySecond(Action action, float delaySeconds)
            {
                if (delaySeconds > 0)
                    yield return new WaitForSeconds(delaySeconds);
                else
                    yield return null;
                action?.Invoke();
            }

            private static IEnumerator StartDelayToInvokeByFrame(Action action, int delayFrames)
            {
                if (delayFrames > 1)
                {
                    for (int i = 0; i < delayFrames; i++)
                    {
                        yield return null;
                    }
                }
                else
                    yield return null;
                action?.Invoke();
            }
        }
        #endregion
    }
}