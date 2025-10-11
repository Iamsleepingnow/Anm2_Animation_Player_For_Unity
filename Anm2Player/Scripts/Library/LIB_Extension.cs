using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【全局综合库】Global Library for Anm2Player</summary>
    public static partial class LIBRARY
    {
        #region Extension 扩展方法
        /// <summary>【复制列表】Copy list</summary>
        /// <typeparam name="T">【列表类型】List type</typeparam>
        /// <param name="list">【列表】List</param>
        /// <returns>【复制后的列表】Copied list</returns>
        public static List<T> Copy<T>(this List<T> list) {
            List<T> newList = new();
            foreach (var item in list) {
                newList.Add(item);
            }
            return newList;
        }

        /// <summary>【判断向量是否相等】Is color equal</summary>
        /// <param name="vec">【向量】Vector</param>
        /// <param name="r">【R】</param>
        /// <param name="g">【G】</param>
        /// <param name="b">【B】</param>
        /// <param name="a">【A】</param>
        /// <returns>【是否全等】Is equal</returns>
        public static bool IsEqual(this Vector4 vec, float r, float g, float b, float a) {
            return vec.x == r && vec.y == g && vec.z == b && vec.w == a;
        }

        /// <summary>【判断颜色是否相等】Is color equal</summary>
        /// <param name="col">【颜色】Color</param>
        /// <param name="r">【R】</param>
        /// <param name="g">【G】</param>
        /// <param name="b">【B】</param>
        /// <param name="a">【A】</param>
        /// <returns>【是否全等】Is equal</returns>
        public static bool IsEqual(this Color col, float r, float g, float b, float a) {
            return col.r == r && col.g == g && col.b == b && col.a == a;
        }

        /// <summary>【获取缓存文件的Key】Get key in cache dictionary</summary>
        /// <param name="cachePath">【缓存路径】Cache path</param>
        public static string GetKey(this AnmCachePath cachePath) {
            return cachePath.PathConcat();
        }

        /// <summary>【获取缓存文件的Key】Get key in cache dictionary</summary>
        /// <param name="fileRuntime">【动画文件运行时】Animation file runtime</param>
        public static string GetKey(this AnmFileRuntimeRaw fileRuntime) {
            if (fileRuntime.anmFile == null) {
                return string.Empty;
            }
            return fileRuntime.anmFile.name;
        }

        /// <summary>【获取缓存文件的Key】Get key in cache dictionary</summary>
        /// <param name="fileRuntime">【动画文件运行时】Animation file runtime</param>
        public static string GetKey(this AnmFileRuntime fileRuntime) {
            if (fileRuntime.anmFile == null) {
                return string.Empty;
            }
            return fileRuntime.anmFile.name;
        }
        #endregion
    }
}