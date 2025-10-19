using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【单例基类】Singleton base</summary>
    /// <typeparam name="T">【需要设置为单例的类型】Type that need to set as singleton</typeparam>
    public abstract class AnmSingleton<T> : MonoBehaviour where T : AnmSingleton<T>
    {
        private static T ins;
        public static T Ins { // 单例实例 | Singleton instance
            get {
                if (ins == null || ins.Equals(null)) {
                    if (ins == null) {
                        ins = (T)(FindObjectOfType(typeof(T)) as object);
                    }
                }
                return ins;
            }
        }

        protected virtual void OnDestroy() {
            if (ins == this) {
                ins = null;
            }
        }
    }
}