using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【全局综合库】Global Library for Anm2Player</summary>
    public static partial class LIBRARY
    {
        #region Utilities 实用
        /// <summary>【获取某物体上的第一个任意组件，优先自身】Get the first arbitrary component on an object, prioritizing itself</summary>
        /// <typeparam name="T">【类型】Template type</typeparam>
        /// <param name="go">【查询物体】Object</param>
        /// <param name="descend">【是否查询子级，否则查询一层父级】Whether to query the sub-level. Otherwise, query one level up</param>
        /// <returns>【返回获取到的组件】Component</returns>
        public static T GetComponentAround<T>(this GameObject go, bool descend = true) where T : Component {
            bool checkChildren = true;
            bool checkParent = true;
            List<T> selfTemp = new List<T>(go.GetComponents<T>());//自身组件
            if (selfTemp.Count > 0) return selfTemp[0];
            //
            if (checkChildren)//检查子级
            {
                List<T> childTemp = new List<T>(go.GetComponentsInChildren<T>());//携带自身
                if (childTemp.Count > 0) return childTemp[0];
            }
            if (checkParent)//检查父级
            {
                List<T> parentTemp = new List<T>(go.GetComponentsInParent<T>());//携带自身
                if (parentTemp.Count > 0) return parentTemp[0];
            }
            return default;
        }
        /// <summary>【获取某物体上的第一个任意组件，优先自身，否则添加】Get the first arbitrary component on an object, prioritizing itself; otherwise, add</summary>
        /// <typeparam name="T">【类型】Template type</typeparam>
        /// <param name="go">【查询物体】Object</param>
        /// <param name="descend">【是否查询子级，否则查询一层父级】Whether to query the sub-level. Otherwise, query one level up</param>
        /// <returns>【返回获取到的组件】Component</returns>
        public static T GetComponentAroundOrAdd<T>(this GameObject go, bool descend = true) where T : Component {
            return go.GetComponentAround<T>(descend) ?? go.AddComponent<T>();
        }
        #endregion
    }
}