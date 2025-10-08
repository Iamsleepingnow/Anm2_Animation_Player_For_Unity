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