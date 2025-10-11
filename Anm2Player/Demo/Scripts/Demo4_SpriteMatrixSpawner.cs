using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player.Demos
{
    [RequireComponent(typeof(AnmFileRuntime))]
    public class Demo4_SpriteMatrixSpawner : MonoBehaviour
    {
        [SerializeField] public Vector3Int spawnCount = new(3, 3, 3); // XYZ三轴的生成数量 | XYZ axis's count
        [SerializeField] public Vector3 spawnOrigin = Vector3.zero; // 世界空间中的生成原点 | World space origin
        [SerializeField] public Vector3 objectGap = new(1f, 1f, 1f); // 相邻两个物体之间的生成距离 | Gap between two objects
        [SerializeField] public Vector3 singlePosition = Vector3.zero; // 物体的位置 | Object's position
        [SerializeField] public Quaternion singleRotation = Quaternion.identity; // 物体的旋转 | Object's rotation
        [SerializeField] public Vector3 singleScale = Vector3.one; // 物体的缩放 | Object's scale
        [SerializeField] public AnmShaderQuality shaderQuality = AnmShaderQuality.Lit_Transparent; // 材质质量 | Material quality
        
        [SerializeField] public Button btn_Spawn = null; // 生成按钮 | Spawn button
        [SerializeField] public Button btn_Clear = null; // 清空按钮 | Clear button

        [InfoBox("这里可以将File Path Type设置为Component来测试组件数据输入\nHere you can set 'File Path Type' to 'Component' to test component data input", EInfoBoxType.Normal)]
        [SerializeField] public AnmFilePathType filePathType = AnmFilePathType.StreamingAssets; // 文件路径类型 | File path type
        [HideIf("__isComponent")]
        [SerializeField] public string filePath = ""; // 文件路径 | File path
        [ShowIf("__isComponent")]
        [SerializeField] public AnmFileRuntime anmFileRuntime = null;
        private bool __isComponent => filePathType == AnmFilePathType.Component; // 是否为组件模式 | Is component mode

        void Start() {
            // 使用提前缓存的方式将动画文件缓存到管理器中 | Cache the Anm animation file in the cache manager in advance,
            AnmCacheManager.Ins.PreCacheFiles(new List<AnmCachePath>() { // 预缓存文件 | PreCache files
                new(filePathType, filePath),
            });
            AnmCacheManager.Ins.PreCacheFiles(new List<AnmFileRuntimeRaw>() {
                new(anmFileRuntime.anmFile, anmFileRuntime.anmSpriteSheets), // 也可以写成 | Also: new(anmFileRuntime)
            });
            // 按钮：生成动画物体 | Spawn animation objects
            if (btn_Spawn != null) {
                btn_Spawn.onClick.AddListener(() => { StartCoroutine(Button_Spawn()); });
            }
            // 按钮：清空动画物体 | Clear animation objects
            if (btn_Clear != null) {
                btn_Clear.onClick.AddListener(Button_Clear);
                btn_Spawn.interactable = true;
                btn_Clear.interactable = false;
            }
        }

        public IEnumerator Button_Spawn() {
            btn_Spawn.interactable = false;
            btn_Clear.interactable = false;
            // 生成物体矩阵 | Init game objects matrix
            for (int x = 0; x < spawnCount.x; x++) {
                yield return new WaitForEndOfFrame();
                for (int y = 0; y < spawnCount.y; y++) {
                    yield return new WaitForEndOfFrame();
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
                                    anm.InitAnimation(shaderQuality: shaderQuality)
                                        .SetCurrentAnimation(0);
                                    AnmAnimation currentAnimation = anm.GetCurrentAnimationDesc(); // 获取当前动画描述 | Get current animation description
                                    anm.SetCurrentFrameIndex(Random.Range(0, currentAnimation.FrameNum)); // 设置随机单帧图像 | Set random single frame image
                                }
                            );
                        }
                        else { // 使用路径进行加载 | Load using path
                            sprite.LoadAnmFile(filePathType, filePath,
                                isAsync: false,
                                file: out var file,
                                onLoaded: (anm, sheets) => {
                                    anm.InitAnimation(shaderQuality: shaderQuality)
                                        .SetCurrentAnimation(0);
                                    AnmAnimation currentAnimation = anm.GetCurrentAnimationDesc(); // 获取当前动画描述 | Get current animation description
                                    anm.SetCurrentFrameIndex(Random.Range(0, currentAnimation.FrameNum)); // 设置随机单帧图像 | Set random single frame image
                                }
                            );
                        }
                    }
                }
            }
            btn_Spawn.interactable = false;
            btn_Clear.interactable = true;
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