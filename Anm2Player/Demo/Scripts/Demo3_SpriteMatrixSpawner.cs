using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player.Demos
{
    [RequireComponent(typeof(AnmFileRuntime))]
    public class Demo3_SpriteMatrixSpawner : MonoBehaviour
    {
        [SerializeField] public Vector3Int spawnCount = new(3, 3, 3); // XYZ三轴的生成数量 | XYZ axis's count
        [SerializeField] public Vector3 spawnOrigin = Vector3.zero; // 世界空间中的生成原点 | World space origin
        [SerializeField] public Vector3 objectGap = new(1f, 1f, 1f); // 相邻两个物体之间的生成距离 | Gap between two objects
        [SerializeField] public Vector3 singlePosition = Vector3.zero; // 物体的位置 | Object's position
        [SerializeField] public Quaternion singleRotation = Quaternion.identity; // 物体的旋转 | Object's rotation
        [SerializeField] public Vector3 singleScale = Vector3.one; // 物体的缩放 | Object's scale
        [Range(0.1f, 3.0f)]
        [SerializeField] public float animationSpeed = 0.75f; // 动画播放速度 | Animation playback speed
        
        [SerializeField] public Button btn_PreCache = null; // 预缓存按钮 | PreCache button
        [SerializeField] public Button btn_ClearCache = null; // 清空缓存按钮 | ClearCache button
        [SerializeField] public Button btn_Spawn = null; // 生成按钮 | Spawn button
        [SerializeField] public Button btn_Clear = null; // 清空按钮 | Clear button

        [InfoBox("这里可以将File Path Type设置为Component来测试组件数据输入\nHere you can set 'File Path Type' to 'Component' to test component data input", EInfoBoxType.Normal)]
        [SerializeField] public LIBRARY.FilePathType filePathType = LIBRARY.FilePathType.StreamingAssets; // 文件路径类型 | File path type
        [HideIf("__isComponent")]
        [SerializeField] public string filePath = ""; // 文件路径 | File path
        [ShowIf("__isComponent")]
        [SerializeField] public AnmFileRuntime anmFileRuntime = null;
        private bool __isComponent => filePathType == LIBRARY.FilePathType.Component; // 是否为组件模式 | Is component mode

        void Start() {
            // 按钮：预缓存 | PreCache
            if (btn_PreCache != null) {
                btn_PreCache.onClick.AddListener(() => {
                    // 使用提前缓存的方式将动画文件缓存到管理器中 | Cache the Anm animation file in the cache manager in advance,
                    // 这样AnmSprite组件在加载时，会优先从缓存管理器中查找 | So that the AnmSprite component will prioritize looking up from the cache manager when loading
                    // 当文件路径类型为StreamingAssets, AppData或Custom时，传递AnmCachePath数列作为参数
                    // When the file path type is "StreamingAssets", "AppData", or "Custom", pass the AnmCachePath list as parameter
                    // *    缓存后，可通过AnmCachePath.GetKey()作为字典的key键来查找
                    // *    After caching, "AnmCachePath.GetKey()" can be used as the key to look up in a dictionary
                    AnmCacheManager.Ins.PreCacheFiles(new List<AnmCachePath>() { // 预缓存文件 | PreCache files
                        new(filePathType, filePath),
                        // ...
                        // ...
                    });
                    // 当文件路径类型为Component时，传递AnmFileRuntimeRaw数列作为参数
                    // When the file path type is "Component", pass the AnmFileRuntimeRaw list as parameter
                    // *    缓存后，可通过AnmFileRuntimeRaw.GetKey()或AnmFileRuntime.GetKey()作为字典的key键来查找
                    // *    After caching, "AnmFileRuntimeRaw.GetKey()" or "AnmFileRuntime.GetKey()" can be used as a key to look up in the dictionary.
                    AnmCacheManager.Ins.PreCacheFiles(new List<AnmFileRuntimeRaw>() {
                        new(anmFileRuntime.anmFile, anmFileRuntime.anmSpriteSheets), // 也可以写成 | Also: new(anmFileRuntime)
                        // ...
                        // ...
                    });
                });
            }
            // 按钮：清空缓存 | ClearCache
            if (btn_ClearCache != null) {
                btn_ClearCache.onClick.AddListener(() => {
                    AnmCacheManager.Ins.TryRemoveCacheFile(new AnmCachePath(filePathType, filePath).GetKey()); // 删除单个缓存 | Remove single cache
                    AnmCacheManager.Ins.TryRemoveCacheFile(anmFileRuntime.GetKey()); // 删除单个缓存 | Remove single cache
                    // AnmCacheManager.Ins.ClearCacheFiles(); // 清空所有缓存 | Clear all cache
                });
            }
            // 按钮：生成动画物体 | Spawn animation objects
            if (btn_Spawn != null) {
                btn_Spawn.onClick.AddListener(Button_Spawn);
                btn_Clear.interactable = true;
            }
            // 按钮：清空动画物体 | Clear animation objects
            if (btn_Clear != null) {
                btn_Clear.onClick.AddListener(Button_Clear);
                btn_Clear.interactable = false;
            }
        }

        public void Button_Spawn() {
            btn_Spawn.interactable = false;
            btn_Clear.interactable = true;
            //
            List<string> animationNames = new() {
                "Idle", "Run", "Jump", "Melee",
            };
            // 生成物体矩阵 | Init game objects matrix
            for (int x = 0; x < spawnCount.x; x++) {
                for (int y = 0; y < spawnCount.y; y++) {
                    for (int z = 0; z < spawnCount.z; z++) {
                        // 计算当前物体的位置 | Calculate current object's position
                        Vector3 position = spawnOrigin + new Vector3(
                            x * objectGap.x,
                            y * objectGap.y,
                            z * objectGap.z
                        );
                        // 实例化物体 | Instantiate game object
                        GameObject instance = new($"AnmSprite {x}_{y}_{z}");
                        instance.transform.SetParent(transform);
                        instance.transform.SetPositionAndRotation(position + singlePosition, singleRotation);
                        instance.transform.localScale = singleScale;
                        // 配置动画组件 | Configure animation component
                        AnmSprite sprite = instance.AddComponent<AnmSprite>();
                        if (__isComponent) { // 使用组件进行加载 | Load using component
                            sprite.LoadAnmFile(anmFileRuntime,
                                file: out var file,
                                onLoaded: (anm, sheets) => {
                                    anm.InitAnimation()
                                        .SetCurrentAnimation(animationNames[UnityEngine.Random.Range(0, animationNames.Count)])
                                        .SetPlayBackSpeed(animationSpeed)
                                        .Play();
                                }
                            );
                            // 也可以手动缓存到管理器中 | You can cache to manager manually, too
                            // sprite.SetCache();
                        }
                        else { // 使用路径进行加载 | Load using path
                            sprite.LoadAnmFile(filePathType, filePath,
                                // 当需要在一帧以内生成大量相同动画时，需要保证这里是同步(isAsync=false)的，否则无法缓存到数据
                                // When you need to generate many identical animations within a single frame,
                                //   make sure it is must synchronized here (isAsync=false), otherwise the data cannot be cached
                                isAsync: false,
                                file: out var file,
                                onLoaded: (anm, sheets) => {
                                    anm.InitAnimation()
                                        .SetCurrentAnimation(animationNames[UnityEngine.Random.Range(0, animationNames.Count)])
                                        .SetPlayBackSpeed(animationSpeed)
                                        .Play();
                                }
                            );
                            // 也可以手动缓存到管理器中 | You can cache to manager manually, too
                            // sprite.SetCache();
                        }
                    }
                }
            }
        }

        public void Button_Clear() {
            btn_Spawn.interactable = true;
            btn_Clear.interactable = false;
            // 删除所有子物体 | Delete all child objects
            for (int i = transform.childCount - 1; i >= 0; i--) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}