using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm动画缓存】</summary>
    [System.Serializable]
    public class AnmCache
    {
        /// <summary>【Anm动画文件】</summary>
        public AnmFile anmFile = null;
        /// <summary>【Anm动画图集数列】</summary>
        public List<AnmSpriteSheetTexture> anmSpriteSheets = null;
    }

    /// <summary>【Anm动画缓存路径】</summary>
    [System.Serializable]
    public class AnmCachePath
    {
        /// <summary>【Anm动画缓存路径类型】</summary>
        public AnmFilePathType pathType = AnmFilePathType.StreamingAssets;
        /// <summary>【Anm动画缓存路径】</summary>
        public string path = "";

        public AnmCachePath(AnmFilePathType _pathType, string _path) {
            pathType = _pathType;
            path = _path;
        }

        /// <summary>【路径拼接】Path concat</summary>
        public string PathConcat() {
            return $"{(int)pathType}:{path}";
        }
    }

    /// <summary>【Anm缓存管理器】</summary>
    [AddComponentMenu("Anm2Player/Anm Cache Manager")]
    public class AnmCacheManager : AnmSingleton<AnmCacheManager>
    {
        /// <summary>【是否缓存文件引用】</summary>
        public bool CacheFileReference {
            get { return cacheFileReference; }
            set { cacheFileReference = value; }
        }
        [InfoBox("是否缓存文件引用，否则使用文件复制\nCache file reference instead of file copy", EInfoBoxType.Normal)]
        [BoxGroup("Cache Settings 缓存配置")]
        [SerializeField] private bool cacheFileReference = true;

        /// <summary>【是否读取文件引用】</summary>
        public bool ReadFileReference {
            get { return readFileReference; }
            set { readFileReference = value; }
        }
        [InfoBox("是否读取文件引用，否则读取文件复制\nRead file reference instead of file copy", EInfoBoxType.Normal)]
        [BoxGroup("Cache Settings 缓存配置")]
        [SerializeField] private bool readFileReference = true;

        /// <summary>【在初始化时缓存】Cache on Awake</summary>
        [BoxGroup("Auto Cache 自动缓存")]
        [SerializeField] private bool autoCacheOnAwake = true;
        [BoxGroup("Auto Cache 自动缓存")]
        [SerializeField] private List<AnmCachePath> autoCachePaths = new();
        [BoxGroup("Auto Cache 自动缓存")]
        [SerializeField] private List<AnmFileRuntimeRaw> autoCacheFilesRaw = new();
        [BoxGroup("Auto Cache 自动缓存")]
        [SerializeField] private List<AnmFileRuntime> autoCacheFiles = new();

        /// <summary>【缓存的动画文件】</summary>
        public Dictionary<string, AnmCache> CacheFiles = new();

        protected void Awake() {
            if (autoCacheOnAwake) {
                PreCacheFiles(autoCachePaths);
                PreCacheFiles(autoCacheFilesRaw);
                List<AnmFileRuntimeRaw> autoCacheFilesRawTemp = new();
                foreach (var f in autoCacheFiles) {
                    autoCacheFilesRawTemp.Add(f.GetRaw());
                }
                PreCacheFiles(autoCacheFilesRawTemp);
            }
        }

        /// <summary>【添加缓存文件】Try add cache file</summary>
        /// <param name="key">【键：字符串】Key: path</param>
        /// <param name="file">【值：Anm文件】</param>
        /// <param name="spriteSheets">【值：图集数列】</param>
        public void AddCacheFile(string key, AnmFile file, List<AnmSpriteSheetTexture> spriteSheets) {
            if (!CacheFiles.ContainsKey(key)) { // 添加缓存，不允许重复 | Add cache, not allow repeat
                CacheFiles.Add(key, new() { anmFile = CacheFileReference ? file : file.Copy(), anmSpriteSheets = spriteSheets });
            }
        }

        /// <summary>【添加缓存文件】Try add cache file</summary>
        /// <param name="cachePath">【键：路径】Key: path</param>
        /// <param name="file">【值：Anm文件】</param>
        /// <param name="spriteSheets">【值：图集数列】</param>
        public void AddCacheFile(AnmCachePath cachePath, AnmFile file, List<AnmSpriteSheetTexture> spriteSheets) {
            string filePath = cachePath.PathConcat();
            AddCacheFile(filePath, file, spriteSheets);
        }

        /// <summary>【通过路径预先添加缓存文件】Add cache file by paths</summary>
        /// <param name="paths">【路径数列】Paths list</param>
        public void PreCacheFiles(List<AnmCachePath> paths) {
            if (paths == null) return;
            if (paths.Count <= 0) return;
            List<AnmCachePath> pathsTemp = new(); // 创建临时数列 | Create temporary list
            for (int p = 0; p < paths.Count; p++) {
                if (!string.IsNullOrEmpty(paths[p].path) && paths[p].pathType != AnmFilePathType.Component) {
                    pathsTemp.Add(paths[p]);
                }
            }
            AnmSprite tempSprite = new GameObject("__TEMPSPRITE__").AddComponent<AnmSprite>();
            tempSprite.SetParent(transform)
                .SetTransform(Vector3.zero, Quaternion.identity, Vector3.one);
            AnmFile outFile = null;
            int loadIndex = 0;
            for (int p = 0; p < pathsTemp.Count; p++) {
                if (pathsTemp[p].pathType != AnmFilePathType.Component) { // 当路径类型为组件时，则不进行缓存
                    AnmCachePath cachePath = pathsTemp[p];
                    tempSprite.LoadAnmFile(pathsTemp[p].pathType, pathsTemp[p].path, false, out outFile, (anm, sheets) => {
                        if (outFile != null) {
                            AddCacheFile(cachePath, outFile.Copy(), sheets);
                        }
                        loadIndex++;
                        if (loadIndex == pathsTemp.Count) {
                            Destroy(tempSprite.gameObject);
                        }
                    });
                }
            }
        }

        /// <summary>【通过路径预先添加缓存文件】Add cache file by paths</summary>
        /// <param name="paths">【路径数列】Paths list</param>
        public void PreCacheFiles(List<AnmFileRuntimeRaw> files) {
            if (files == null) return;
            if (files.Count <= 0) return;
            List<AnmFileRuntimeRaw> filesTemp = new(); // 创建临时数列 | Create temporary list
            for (int f = 0; f < files.Count; f++) {
                if (files[f].anmFile != null) {
                    filesTemp.Add(files[f]);
                }
            }
            AnmSprite tempSprite = new GameObject("__TEMPSPRITE__").AddComponent<AnmSprite>();
            tempSprite.SetParent(transform)
                .SetTransform(Vector3.zero, Quaternion.identity, Vector3.one);
            AnmFile outFile = null;
            int loadIndex = 0;
            for (int f = 0; f < filesTemp.Count; f++) {
                tempSprite.LoadAnmFile(filesTemp[f].anmFile, filesTemp[f].anmSpriteSheets, out outFile, (anm, sheets) => {
                    if (outFile != null) {
                        AddCacheFile(filesTemp[f].anmFile.name, outFile.Copy(), sheets);
                    }
                    loadIndex++;
                    if (loadIndex == filesTemp.Count) {
                        Destroy(tempSprite.gameObject);
                    }
                });
            }
        }

        /// <summary>【尝试获取缓存文件】Try get cache file</summary>
        /// <param name="cachePath">【键：路径】Key: path</param>
        /// <param name="file">【值：Anm文件】</param>
        /// <param name="spriteSheets">【值：图集数列】</param>
        /// <returns>【是否获取成功】Is success</returns>
        public bool TryGetCacheFile(AnmCachePath cachePath, out AnmFile file, out List<AnmSpriteSheetTexture> spriteSheets) {
            string filePath = cachePath.PathConcat();
            return TryGetCacheFile(filePath, out file, out spriteSheets);
        }

        /// <summary>【尝试获取缓存文件】Try get cache file</summary>
        /// <param name="key">【键：字符串】Key: path</param>
        /// <param name="file">【值：Anm文件】</param>
        /// <param name="spriteSheets">【值：图集数列】</param>
        /// <returns>【是否获取成功】Is success</returns>
        public bool TryGetCacheFile(string key, out AnmFile file, out List<AnmSpriteSheetTexture> spriteSheets) {
            if (CacheFiles.TryGetValue(key, out AnmCache outCache)) { // 尝试获取缓存 | Try get cache
                file = ReadFileReference ? outCache.anmFile : outCache.anmFile.Copy();
                spriteSheets = outCache.anmSpriteSheets;
                return true;
            }
            file = null;
            spriteSheets = null;
            return false;
        }

        /// <summary>【移除缓存文件】Try remove cache file</summary>
        /// <param name="key">【键：字符串】Key: path</param>
        /// <returns>【是否移除成功】Is success</returns>
        public bool TryRemoveCacheFile(string key) {
            if (CacheFiles.TryGetValue(key, out _)) { // 尝试获取缓存 | Try get cache
                return CacheFiles.Remove(key);
            }
            return false;
        }

        /// <summary>【移除缓存文件】Try remove cache file</summary>
        /// <param name="cachePath">【键：路径】Key: path</param>
        /// <returns>【是否移除成功】Is success</returns>
        public bool TryRemoveCacheFile(AnmCachePath cachePath) {
            string filePath = cachePath.PathConcat();
            return TryRemoveCacheFile(filePath);
        }

        /// <summary>【清空缓存】Clear caches</summary>
        public void ClearCacheFiles() {
            CacheFiles.Clear();
        }
    }
}