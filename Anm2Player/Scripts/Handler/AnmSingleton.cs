using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【单例基类】Singleton base</summary>
    /// <typeparam name="T">【需要设置为单例的类型】Type that need to set as singleton</typeparam>
    public abstract class AnmSingleton<T> : MonoBehaviour
    {
        private static T ins;
        public static T Ins {
            get {
                ins ??= (T)(FindObjectOfType(typeof(T)) as object);
                return ins;
            }
        }
    }
}