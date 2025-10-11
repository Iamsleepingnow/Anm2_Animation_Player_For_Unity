using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension
using System.Linq;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm动画图集纹理】Anm Animation Sprite sheet texture</summary>
    [System.Serializable]
    public class AnmSpriteSheetTexture
    {
        /// <summary>【图集ID】Sprite sheet ID</summary>
        public int spriteSheetId = 0;
        /// <summary>【贴图】Sprite sheet texture</summary>
        [ShowAssetPreview(128, 128)]
        public Texture2D texture = null;
    }

    /// <summary>
    /// <para>【Anm动画着色器质量类型】</para>
    /// <para>Lit_Transparent: 有光照+梯度透明 | Lit + Transparent</para>
    /// <para>Lit_Opaque: 有光照+不透明 | Lit + Opaque</para>
    /// <para>Unlit_Transparent: 无光照+梯度透明 | Unlit + Transparent</para>
    /// <para>Unlit_Opaque: 无光照+不透明 | Unlit + Opaque</para>
    /// </summary>
    public enum AnmShaderQuality
    {
        /// <summary>【有光照+梯度透明】Lit + Transparent</summary>
        Lit_Transparent = 0,
        /// <summary>【有光照+不透明】Lit + Opaque</summary>
        Lit_Opaque = 1,
        /// <summary>【无光照+梯度透明】Unlit + Transparent</summary>
        Unlit_Transparent = 2,
        /// <summary>【无光照+不透明】Unlit + Opaque</summary>
        Unlit_Opaque = 3,
    }

    /// <summary>【Anm动画播放器】Anm Animation Player</summary>
    [AddComponentMenu("Anm2Player/Anm Sprite")]
    public class AnmSprite : AnmLayerRuntime, IAnmAnimatable
    {
        private bool IsAppPlaying => Application.isPlaying; // 是否运行中 | Is the application running?

        [Tooltip("\n是否在Awake时自动加载Anm文件\nIs the Anm file automatically loaded when Awake?\n")]
        [BoxGroup("Auto Load 自动加载")]
        [SerializeField] private bool autoLoadOnAwake = false; // 是否在Awake时自动加载Anm文件 | Is the Anm file automatically loaded when Awake?

        [Tooltip("\n是否在Awake时自动播放，否则暂停\nIs the playback automatically loaded when Awake? Otherwise pause\n")]
        [BoxGroup("Auto Load 自动加载"), ShowIf("autoLoadOnAwake")]
        [SerializeField] private bool autoPlay = false; // 是否在Awake时自动播放 | Is the playback automatically loaded when Awake?

        [Tooltip("\n自动播放前的延迟帧数\nDelay frames before auto playback\n")]
        [BoxGroup("Auto Load 自动加载"), ShowIf(EConditionOperator.And, "autoLoadOnAwake", "autoPlay"), MinValue(0)]
        [SerializeField] private int autoPlayDelayFrames = 1; // 自动播放的延迟帧数 | Auto play frame delay

        [Tooltip("\n是否在Awake时自动缓存\nIs auto cache when Awake?\n")]
        [BoxGroup("Auto Load 自动加载"), ShowIf("autoLoadOnAwake")]
        [SerializeField] private bool autoCache = false; // 是否自动缓存 | Is the cache automatically loaded?

        [Tooltip("\nAnm文件路径类型:\nAnm file path type:" +
            "\n - StreamingAssets: \n[Assets/StreamingAssets] [<BuildPath>/<ProjectName>_Data/StreamingAssets]" +
            "\n - AppData: \n[C:/Users/<UserName>/AppData/LocalLow/<CompanyName>/<ProjectName>]" +
            "\n - Custom: \n[<CustomPath>]" +
            "\n - Component: \n[<Use'AnmFileRuntime'ComponentInstead>]\n")]
        [BoxGroup("Auto Load 自动加载"), ShowIf("autoLoadOnAwake")]
        [SerializeField] private AnmFilePathType autoLoadFilePathType = AnmFilePathType.StreamingAssets; // Anm文件路径类型 | Anm file path type

        [Tooltip("\nAnm文件路径，若autoLoadFilePathType为Custom，则使用绝对路径，否则使用相对路径\nAnm file path, if autoLoadFilePathType is Custom, use absolute path; otherwise relative path\n")]
        [InfoBox("若FilePathType为Custom，则使用绝对路径，否则使用相对路径\nIf FilePathType is Custom, use absolute path; otherwise relative path", EInfoBoxType.Normal)]
        [BoxGroup("Auto Load 自动加载"), ShowIf(EConditionOperator.And, "autoLoadOnAwake", "__autoLoadFilePathTypeIsNotComponent")]
        [SerializeField] private string autoLoadFilePath = ".\\Sample.anm2"; // Anm文件路径 | Anm file path
        private bool __autoLoadFilePathTypeIsNotComponent => autoLoadFilePathType != AnmFilePathType.Component; // 是否不是Component路径 | Is itn't Component path?

        [Tooltip("\nAnm文件组件\nAnm file component\n")]
        [BoxGroup("Auto Load 自动加载"), ShowIf(EConditionOperator.And, "autoLoadOnAwake", "__autoLoadFilePathTypeIsComponent")]
        [SerializeField] private AnmFileRuntime autoLoadAnmFile = null; // Anm文件 | Anm file
        private bool __autoLoadFilePathTypeIsComponent => autoLoadFilePathType == AnmFilePathType.Component;  // 是否是Component路径 | Is it Component path?

        private TextAsset autoLoadAnmFileAsset = null; // Anm文件的文本文件 | Anm file text file

        [Tooltip("\n当自动加载Anm文件时，是否自动查找现有图层\nShould existing layers be automatically searched for?\n")]
        [InfoBox("若FindLayers开启，优先使用当前存在于子集的图层\nIf FindLayers is enabled, it will prioritize using layers that already exist in the subset", EInfoBoxType.Normal)]
        [BoxGroup("Auto Load 自动加载"), ShowIf("autoLoadOnAwake")]
        [SerializeField] private bool autoFindLayers = false; // 是否自动查找现有图层 | Whether to automatically initialize layers

        [Tooltip("\n当初始化动画时所应用的着色器质量\nShader quality applied when initializing the animation\n")]
        [BoxGroup("Auto Load 自动加载"), ShowIf("autoLoadOnAwake")]
        [SerializeField] private AnmShaderQuality autoLoadShaderQuality = AnmShaderQuality.Lit_Transparent; // 材质质量 | Material quality

        [Tooltip("\n自动播放的动画序数\nAuto animation index\n")]
        [InfoBox("动画序数为-1时代表默认\nAnimation index = -1 means default", EInfoBoxType.Normal)]
        [BoxGroup("Auto Load 自动加载"), ShowIf("autoLoadOnAwake"), MinValue(-1)]
        [SerializeField] private int autoLoadAnimationIndex = -1; // 自动播放的动画序数 | Auto animation index

        [Tooltip("\n自动播放的起始帧序列\nAuto play start frame\n")]
        [BoxGroup("Auto Load 自动加载"), ShowIf("autoLoadOnAwake"), MinValue(0)]
        [SerializeField] private int autoLoadStartFrame = 0; // 自动播放的帧数 | Auto play frame

        [Button("加载 Load"), ShowIf("IsAppPlaying")]
        private void __LoadSpriteSheets() {
            if (autoLoadFilePathType != AnmFilePathType.Component) {
                LoadAnmFile(autoLoadFilePathType, autoLoadFilePath, true, out _, (anm, _) => {
                    anm.InitAnimation(autoFindLayers, autoLoadAnimationIndex, true, autoLoadShaderQuality)
                        .SetCurrentFrameIndex(DEFAULT_FRAME_INDEX);
                });
            }
            else {
                LoadAnmFile(autoLoadAnmFile.anmFile, autoLoadAnmFile.anmSpriteSheets, out _, (anm, _) => {
                    anm.InitAnimation(autoFindLayers, autoLoadAnimationIndex, true, autoLoadShaderQuality)
                        .SetCurrentFrameIndex(DEFAULT_FRAME_INDEX);
                });
            }
        }
        [Button("播放 Play"), ShowIf("IsAppPlaying")]
        private void __PlayAnimation() {
            Play();
        }
        [Button("暂停 Pause"), ShowIf("IsAppPlaying")]
        private void __PauseAnimation() {
            Pause();
        }
        [Button("停止 Stop"), ShowIf("IsAppPlaying")]
        private void __StopAnimation() {
            SetCurrentAnimation(autoLoadAnimationIndex, 0, true);
            Stop();
        }
        [Button("清除 Clear"), ShowIf("IsAppPlaying")]
        private void __ClearAnimation() {
            Stop();
            Clear();
        }

        private const string BASE_LAYER_NAME = "*Base*"; // 主图层名称 | Base layer name
        private const string ROOT_LAYER_NAME = "*Root*"; // 根图层名称 | Root layer name
        private const int DEFAULT_FRAME_INDEX = 0; // 默认帧序数 | Default frame index

        private const string DEBUG_WARNING_PATH_NOT_FOUND = "[Anm2Player] 加载Anm文件失败，path路径不存在 | Failed to load Anm file, path is not exist: "; // 路径不存在 | Path is not exist
        private const string DEBUG_WARNING_FILE_INCORRECT = "[Anm2Player] 加载Anm文件失败，文件格式不正确 | Failed to load Anm file, file format is incorrect: "; // 文件格式不正确 | File format is incorrect
        private const string DEBUG_WARNING_TEXTASSET_EMPTY = "[Anm2Player] 加载Anm文件失败，TextAsset为空 | Failed to load Anm file, TextAsset is empty"; // TextAsset为空 | TextAsset is empty
        private const string DEBUG_WARNING_ANMFILERUNTIME_EMPTY = "[Anm2Player] 加载Anm文件失败，AnmFileRuntime组件为空 | Failed to load Anm file, AnmFileRuntime component is empty"; // AnmFileRuntime组件为空 | AnmFileRuntime component is empty

        /// <summary>【是否已初始化 | Whether initialized】</summary>
        public bool IsInitialized {
            get => isInitialized;
            private set => isInitialized = value;
        }
        private bool isInitialized = false;

        /// <summary>【播放速度】Playback speed</summary>
        public float PlayBackSpeed {
            get => playBackSpeed;
            set {
                if (IsAppPlaying) {
                    playBackSpeed = value;
                    SetPlayBackSpeed(playBackSpeed);
                }
            }
        }
        [Tooltip("\n播放速度，默认为1，可实时修改\nPlayback speed, default 1, real-time editable\n")]
        [BoxGroup("Playing 播放"), MinValue(0.01f), OnValueChanged("__DebugSetPlayBackSpeed")]
        [SerializeField] private float playBackSpeed = 1f; // 播放速度 | Playback speed
        private void __DebugSetPlayBackSpeed() => SetPlayBackSpeed(playBackSpeed);

        /// <summary>【是否倒放】Is reversed</summary>
        public bool IsReversed {
            get => isReversed;
            set => isReversed = value;
        }
        [Tooltip("\n播放方向，勾选为倒放，可实时修改\nPlay direction, checked is for reverse, real-time editable\n")]
        [BoxGroup("Playing 播放")]
        [SerializeField] private bool isReversed = false; // 播放方向 | Play direction

        /// <summary>【是否暂停】Is paused</summary>
        public bool IsPaused {
            get => isPaused;
            set {
                isPaused = value;
                if (IsAppPlaying) {
                    if (value == true)
                        Pause();
                    else
                        Play();
                }
            }
        }
        [Tooltip("\n暂停播放，可实时修改\nPause play, real-time editable\n")]
        [BoxGroup("Playing 播放"), ShowIf("IsAppPlaying"), OnValueChanged("__DebugSetIsPaused")]
        [SerializeField] private bool isPaused = true; // 暂停播放 | Pause play
        private void __DebugSetIsPaused() => IsPaused = isPaused;

        /// <summary>【是否使用弃帧等级表】Frame dropping levels</summary>
        public bool UseFrameDroppingLevels {
            get => useFrameDroppingLevels;
            set {
                useFrameDroppingLevels = value;
                if (useFrameDroppingLevels == true) {
                    __DebugSetFrameDroppingLevel();
                }
            }
        }
        [Tooltip("\n是否使用弃帧等级表\nUse frame dropping levels config?\n")]
        [BoxGroup("Timer 计时器"), OnValueChanged("__DebugSetFrameDroppingLevel")]
        [SerializeField] private bool useFrameDroppingLevels = false;

        /// <summary>【帧率】Frame rate</summary>
        public float FrameRate {
            get => UseFrameDroppingLevels ? FDL_frameRate : frameRate;
            set {
                if (UseFrameDroppingLevels) {
                    __DebugSetFrameDroppingLevel();
                }
                frameRate = Mathf.Clamp(value, 0f, 300f);
                if (IsAppPlaying) {
                    SetOverrideFrameRate(frameRate);
                }
            }
        }
        [Tooltip("\n帧速率重写，可实时修改\nFrame rate override, real-time editable\n")]
        [InfoBox("当帧率为0时视为默认值\nWhen the frame rate is 0, it is considered the default value", EInfoBoxType.Normal)]
        [BoxGroup("Timer 计时器"), OnValueChanged("__DebugSetFrameRate"), MinValue(0f), MaxValue(300f), HideIf("UseFrameDroppingLevels")]
        [SerializeField] private float frameRate = 0f; // 帧率 | Frame rate
        private float FDL_frameRate = 0f; // 弃帧等级表帧率 | Frame rate of frame dropping levels config
        private void __DebugSetFrameRate() => FrameRate = frameRate;

        /// <summary>【跳帧数】Jump frames</summary>
        public int JumpFrames {
            get => UseFrameDroppingLevels ? FDL_jumpFrames : jumpFrames;
            set {
                if (UseFrameDroppingLevels) {
                    __DebugSetFrameDroppingLevel();
                }
                jumpFrames = Mathf.Max(0, value);
            }
        }
        [Tooltip("\n跳帧帧数\nFrame jump number\n")]
        [InfoBox("每次更新时需要跳过的帧数，默认为0\nNumber of frames to skip on each update, default is 0", EInfoBoxType.Normal)]
        [BoxGroup("Timer 计时器"), MinValue(0), HideIf("UseFrameDroppingLevels")]
        [SerializeField] private int jumpFrames = 0; // 跳帧数 | Jump frames
        private int FDL_jumpFrames = 0; // 弃帧等级表跳帧数 | Frame jump number of frame dropping levels config

        [Tooltip("\n跳帧等级表\nFrame dropping levels config\n")]
        [InfoBox("跳帧等级表\nFrame dropping levels config", EInfoBoxType.Normal)]
        [BoxGroup("Timer 计时器"), ShowIf("UseFrameDroppingLevels"), OnValueChanged("__DebugSetFrameDroppingLevel")]
        [SerializeField] private AnmFrameDroppingLevels frameDroppingLevels = null;

        /// <summary>【跳帧等级】Frame dropping level</summary>
        public int FrameDroppingLevel {
            get => frameDroppingLevel;
            set => frameDroppingLevel = Mathf.Max(value, 0);
        }
        [Tooltip("\n跳帧等级\nFrame dropping level\n")]
        [BoxGroup("Timer 计时器"), ShowIf("UseFrameDroppingLevels"), MinValue(0), OnValueChanged("__DebugSetFrameDroppingLevel")]
        [SerializeField] private int frameDroppingLevel = 0;
        private void __DebugSetFrameDroppingLevel() {
            if (!IsAppPlaying) return;
            if (frameDroppingLevels != null) {
                frameDroppingLevels.GetFrameDroppingLevel(frameDroppingLevel, out FDL_jumpFrames, out FDL_frameRate); // 获取帧率等级 | Get frame rate level
            }
            SetOverrideFrameRate(FrameRate);
        }

        /// <summary>【当前加载的Anm文件】Current loaded Anm file</summary>
        [HideInInspector] public AnmFile CurrentAnmFile = new(); // 当前加载的Anm文件 | Current loaded Anm file
        /// <summary>【读取的图集列表】Sprite sheet list</summary>
        [Foldout("Sprite Sheets 读取的图集")]
        [SerializeField, ReadOnly, ReorderableList] public List<AnmSpriteSheetTexture> LoadedSpriteSheets = new(); // 图集列表 | Sprite sheet list
        /// <summary>【创建的子集根图层】Root layer</summary>
        [Foldout("Spawned Layers 创建的子集图层")]
        [SerializeField, ReadOnly] public AnmRootLayerRuntime SpawnedRootLayer = null; // 根图层 | Root layer
        /// <summary>【创建的子集图集图层列表】Sub-sprite sheet layer list</summary>
        [Foldout("Spawned Layers 创建的子集图层")]
        [SerializeField, ReadOnly, ReorderableList] public List<AnmSpriteLayerRuntime> SpawnedSpriteLayers = new(); // 子图集图层列表 | Sub-sprite sheet layer list
        /// <summary>【创建的子集非图集图层列表】Sub-null layer list</summary>
        [Foldout("Spawned Layers 创建的子集图层")]
        [SerializeField, ReadOnly, ReorderableList] public List<AnmNullLayerRuntime> SpawnedNullLayers = new(); // 子非图集图层列表 | Sub-null layer list

        /// <summary>【当前播放的动画片段索引】Current animation clip index</summary>
        private int currentAnimationIndex = 0; // 当前播放的动画片段索引 | Current animation clip index
        /// <summary>【当前播放的帧序数】Current frame index</summary>
        private int currentFrameIndex = DEFAULT_FRAME_INDEX; // 当前播放的帧序数 | Current frame index
        /// <summary>【当前帧事件是否正在被触发】Current frame event is being triggered</summary>
        private bool isCurrentFrameTriggering = false; // 当前帧事件是否正在被触发 | Current frame event is being triggered

        /// <summary>【当前动画描述】Current animation description</summary>
        private AnmAnimation CurrentAnimationDesc {
            get {
                if (currentAnimationDesc == null) {
                    currentAnimationDesc = CurrentAnmFile.Animations.GetAnimationByIndex(currentAnimationIndex);
                }
                return currentAnimationDesc;
            }
            set {
                currentAnimationDesc = value;
            }
        }
        private AnmAnimation currentAnimationDesc = null;

        #region Events 事件
        /// <summary>【动画加载完成事件】Animation loading completed event</summary>
        [Foldout("Events 事件")]
        [SerializeField] public UnityEvent OnFileLoaded = new(); // 加载完成事件 | Loading completed event

        /// <summary>【帧更新事件】Frame update event</summary>
        [Foldout("Events 事件")]
        [SerializeField] public UnityEvent<float> OnFrameUpdate = new(); // 帧更新事件 | Frame update event

        /// <summary>【事件触发事件】Event triggered event</summary>
        [Foldout("Events 事件")]
        [SerializeField] public UnityEvent<string> OnEventTriggered = new(); // 事件触发事件 | Event triggered event

        /// <summary>【动画开始事件】Animation started event</summary>
        [Foldout("Events 事件")]
        [SerializeField] public UnityEvent<string, int> OnAnimationStarted = new(); // 动画开始事件 | Animation started event

        /// <summary>【动画完成事件】Animation completed event</summary>
        [Foldout("Events 事件")]
        [SerializeField] public UnityEvent<string, int, bool> OnAnimationCompleted = new(); // 动画完成事件 | Animation completed event

        [Foldout("Events 事件")]
        [SerializeField] public UnityEvent<string, int> OnAnimationChanged = new(); // 动画切换事件 | Animation changed event

        [Foldout("Events 事件")]
        [SerializeField] public UnityEvent<int> OnFrameIndexChanged = new(); // 帧序数切换事件 | Frame index changed event
        #endregion

        [Button("查看文件信息 View Anm info")]
        private void __AddAnmFileDebugComponent() {
            AnmDebug debug = gameObject.GetComponentAroundOrAdd<AnmDebug>();
            debug.LoadAnmFile(autoLoadFilePathType, autoLoadFilePath);
        }

        void Awake() {
            // 自动加载文件
            void autoLoadAndPlay() {
                if (autoLoadOnAwake) {
                    if (autoLoadFilePathType != AnmFilePathType.Component) {
                        LoadAnmFile(autoLoadFilePathType, autoLoadFilePath, true, out _, (anm, _) => {
                            anm.InitAnimation(autoFindLayers, autoLoadAnimationIndex, true, autoLoadShaderQuality)
                                .SetCurrentFrameIndex(DEFAULT_FRAME_INDEX)
                                .SetCurrentFrameIndex(autoLoadStartFrame);
                            if (autoPlay) {
                                anm.SetReverse(IsReversed).Play();
                            }
                            if (autoCache) {
                                anm.SetCache();
                            }
                        });
                    }
                    else {
                        LoadAnmFile(autoLoadAnmFile, out _, (anm, _) => {
                            anm.InitAnimation(autoFindLayers, autoLoadAnimationIndex, true, autoLoadShaderQuality)
                                .SetCurrentFrameIndex(DEFAULT_FRAME_INDEX)
                                .SetCurrentFrameIndex(autoLoadStartFrame);
                            if (autoPlay) {
                                anm.SetReverse(IsReversed).Play();
                            }
                            if (autoCache) {
                                anm.SetCache();
                            }
                        });
                    }
                }
            }
            if (autoPlayDelayFrames <= 0) {
                autoLoadAndPlay(); // 不延迟加载 | No delay loading
            }
            else {
                LIBRARY.AnmTASK.Delay(autoPlayDelayFrames, () => { // 延迟1帧加载 | Delay 1 frame to load
                    autoLoadAndPlay();
                });
            }
            SetFrameDroppingLevel(frameDroppingLevel); // 设置帧等级 | Set frame dropping level
        }

        void OnDestroy() {
            AnmTimerManager.Ins.UnregisterAnmTimer(this); // 取消注册动画计时器 | Unregister animation timer
            OnFileLoaded.RemoveAllListeners(); // 移除所有监听器 | Remove all listeners
            OnFrameUpdate.RemoveAllListeners();
            OnEventTriggered.RemoveAllListeners();
            OnAnimationStarted.RemoveAllListeners();
            OnAnimationCompleted.RemoveAllListeners();
            OnAnimationChanged.RemoveAllListeners();
            OnFrameIndexChanged.RemoveAllListeners();
        }

        void OnPostRender() {
            isCurrentFrameTriggering = false; // 恢复帧事件触发标识符 | Restore frame event triggering identifier
        }

        #region 图集 Sprite sheets
        /// <summary>【加载Anm文件】Load Anm File</summary>
        /// <param name="pathType">【文件路径类型】Path type</param>
        /// <param name="path">【文件路径】path</param>
        /// <param name="isAsync">【是否异步加载】Is async loading</param>
        /// <param name="file">【加载的Anm文件信息】Loaded anm info file</param>
        /// <param name="setFrameProps">【是否设置帧属性】Is set frame properties</param>
        /// <param name="initAnimationIndex">【使用哪一个序数的动画作为初始化动画(-1为默认)】Use which animation ID as init animation (-1 is default)</param>
        /// <param name="onLoaded">【图集加载完成后执行】Event on sprite sheets loaded</param>
        /// <returns>【Anm文件】File</returns>
        public AnmSprite LoadAnmFile(AnmFilePathType pathType, string path, bool isAsync, out AnmFile file, UnityAction<AnmSprite, List<AnmSpriteSheetTexture>> onLoaded = null) {
            if (path == string.Empty) {
                Debug.LogError(DEBUG_WARNING_PATH_NOT_FOUND + path);
                file = null; return null;
            }
            string fullPath = LIBRARY.ConcatPathByPathType(pathType, path);
            AnmFile anmFile = null;
            // 当使用组件内部数据时
            if (fullPath != LIBRARY.PATH_TYPE_COMPONENT_KEYWORD) {
                void updatePath() {
                    if (autoLoadFilePathType != pathType) {
                        autoLoadFilePathType = pathType; // 更新路径 | Update path
                    }
                    if (autoLoadFilePath != path) {
                        autoLoadFilePath = path; // 更新路径 | Update path
                    }
                }
                bool aimingCache = false; // 是否命中缓存 | Hit cache or not
                AnmFile outFile = null;
                List<AnmSpriteSheetTexture> outSpriteSheets = null;
                if (AnmCacheManager.Ins != null) {
                    if (AnmCacheManager.Ins.TryGetCacheFile(new AnmCachePath(pathType, path), out outFile, out outSpriteSheets)) {
                        aimingCache = true;
                    }
                }
                if (aimingCache) { // 先尝试从缓存中获取 | Try to get from cache first
                    anmFile = outFile;
                    CurrentAnmFile = outFile;
                    updatePath(); // 更新路径 | Update path
                    LoadedSpriteSheets.Clear();
                    for (int oss = 0; oss < outSpriteSheets.Count; oss++) {
                        LoadedSpriteSheets.Add(outSpriteSheets[oss]);
                    }
                    LIBRARY.AnmTASK.Delay(1, () => {
                        onLoaded?.Invoke(this, LoadedSpriteSheets.Copy()); onLoaded = null;
                        OnFileLoaded.Invoke();
                    });
                }
                else { // 缓存中没有时 | When cache is empty
                    if (!System.IO.File.Exists(fullPath)) {
                        Debug.LogError(DEBUG_WARNING_PATH_NOT_FOUND + fullPath);
                        file = null; return null;
                    }
                    anmFile = LIBRARY.XMLRead<AnmFile>(fullPath);
                    if (anmFile == null) {
                        Debug.LogError(DEBUG_WARNING_FILE_INCORRECT + fullPath);
                        file = null; return null;
                    }
                    anmFile.Content.BakeLayerOrders(); // 计算出所有图集图层的排序信息 | Bake all layer's order infos
                    CurrentAnmFile = anmFile;
                    file = anmFile;
                    updatePath(); // 更新路径 | Update path
                    //
                    List<AnmSpritesheet> anmSpriteSheets = anmFile.Content.Spritesheets;
                    string anmFilesFolder = System.IO.Path.GetDirectoryName(path); // 获取文件所在目录 | Get the directory where the file is located
                    if (isAsync) {
                        LoadAnmTexturesAsync(anmSpriteSheets, pathType, anmFilesFolder, _ => { onLoaded?.Invoke(this, LoadedSpriteSheets.Copy()); onLoaded = null; OnFileLoaded.Invoke(); });
                    }
                    else {
                        LoadAnmTextures(anmSpriteSheets, pathType, anmFilesFolder, _ => { onLoaded?.Invoke(this, LoadedSpriteSheets.Copy()); onLoaded = null; OnFileLoaded.Invoke(); });
                    }
                }
                file = anmFile; return this;
            }
            file = null; return null;
        }

        /// <summary>【加载Anm文件】Load Anm File</summary>
        /// <param name="anmFileAsset">【Anm的XML文本文件】Anm XML text asset</param>
        /// <param name="anmTextures">【Anm所需的图集纹理列表】Anm sprite sheet textures list</param>
        /// <param name="file">【加载的Anm文件信息】Loaded anm info file</param>
        /// <param name="setFrameProps">【是否设置帧属性】Is set frame properties</param>
        /// <param name="initAnimationIndex">【使用哪一个序数的动画作为初始化动画(-1为默认)】Use which animation ID as init animation (-1 is default)</param>
        /// <param name="onLoaded">【图集加载完成后执行】Event on sprite sheets loaded</param>
        /// <returns>【Anm文件】File</returns>
        public AnmSprite LoadAnmFile(TextAsset anmFileAsset, List<Texture2D> anmTextures, out AnmFile file, UnityAction<AnmSprite, List<AnmSpriteSheetTexture>> onLoaded = null) {
            if (anmFileAsset == null) {
                Debug.LogError(DEBUG_WARNING_TEXTASSET_EMPTY);
                file = null; return null;
            }
            autoLoadAnmFileAsset = anmFileAsset;
            autoLoadFilePathType = AnmFilePathType.Component;
            AnmFile anmFile = null;
            bool aimingCache = false; // 是否命中缓存 | Hit cache or not
            AnmFile outFile = null;
            List<AnmSpriteSheetTexture> outSpriteSheets = null;
            if (AnmCacheManager.Ins != null) {
                if (AnmCacheManager.Ins.TryGetCacheFile(anmFileAsset.name, out outFile, out outSpriteSheets)) {
                    aimingCache = true;
                }
            }
            if (aimingCache) { // 先尝试从缓存中获取 | Try to get from cache first
                anmFile = outFile;
                CurrentAnmFile = outFile;
                LoadedSpriteSheets.Clear();
                for (int oss = 0; oss < outSpriteSheets.Count; oss++) {
                    LoadedSpriteSheets.Add(outSpriteSheets[oss]);
                }
                onLoaded?.Invoke(this, LoadedSpriteSheets.Copy()); onLoaded = null;
                OnFileLoaded.Invoke();
            }
            else { // 缓存中没有时 | When cache is empty
                anmFile = anmFileAsset.XMLRead<AnmFile>();
                if (anmFile == null) {
                    Debug.LogError(DEBUG_WARNING_FILE_INCORRECT);
                    file = null; return null;
                }
                anmFile.Content.BakeLayerOrders(); // 计算出所有图集图层的排序信息 | Bake all layer's order infos
                CurrentAnmFile = anmFile;
                file = anmFile;
                List<AnmSpritesheet> anmSpriteSheets = anmFile.Content.Spritesheets;
                LoadedSpriteSheets.Clear();
                foreach (var sheet in anmSpriteSheets) {
                    string sheetTexName = System.IO.Path.GetFileNameWithoutExtension(sheet.Path); // 获取图集文件名
                    foreach (var t in anmTextures) {
                        if (t == null) {
                            continue;
                        }
                        if (t.name == sheetTexName) {
                            t.SetWrapMode(TextureWrapMode.Clamp)
                                .SetFilterMode(FilterMode.Point)
                                .SetMipmap();
                            LoadedSpriteSheets.Add(new AnmSpriteSheetTexture() {
                                spriteSheetId = sheet.Id,
                                texture = t
                            });
                        }
                    }
                }
                onLoaded?.Invoke(this, LoadedSpriteSheets.Copy());
                onLoaded = null;
                OnFileLoaded.Invoke();
            }
            file = anmFile; return this;
        }

        /// <summary>【加载Anm文件】Load Anm File</summary>
        /// <param name="anmFileConponent">【Anm文件组件】Anm file component</param>
        /// <param name="file">【加载的Anm文件信息】Loaded anm info file</param>
        /// <param name="setFrameProps">【是否设置帧属性】Is set frame properties</param>
        /// <param name="initAnimationIndex">【使用哪一个序数的动画作为初始化动画(-1为默认)】Use which animation ID as init animation (-1 is default)</param>
        /// <param name="onLoaded">【图集加载完成后执行】Event on sprite sheets loaded</param>
        /// <returns>【Anm文件】File</returns>
        public AnmSprite LoadAnmFile(AnmFileRuntime anmFileConponent, out AnmFile file, UnityAction<AnmSprite, List<AnmSpriteSheetTexture>> onLoaded = null) {
            if (anmFileConponent == null) {
                Debug.LogError(DEBUG_WARNING_ANMFILERUNTIME_EMPTY);
                file = null; return null;
            }
            if (anmFileConponent.anmFile == null) {
                Debug.LogError(DEBUG_WARNING_TEXTASSET_EMPTY);
                file = null; return null;
            }
            return LoadAnmFile(anmFileConponent.anmFile, anmFileConponent.anmSpriteSheets, out file, onLoaded);
        }

        /// <summary>【同步加载Anm图集贴图】Sync load textures</summary>
        /// <param name="sheets">【Anm图集列表】Sprite sheet list</param>
        /// <param name="pathType">【文件路径类型】Path type</param>
        /// <param name="rootFolder">【anm2文件根路径】Root folder path</param>
        /// <param name="onLoaded">【图集加载完成后执行】Callback when sprite sheets loaded</param>
        private void LoadAnmTextures(List<AnmSpritesheet> sheets, AnmFilePathType pathType, string rootFolder, UnityAction<List<AnmSpriteSheetTexture>> onLoaded = null) {
            List<string> paths = new();
            foreach (AnmSpritesheet ss in sheets) {
                paths.Add(System.IO.Path.Combine(rootFolder, ss.Path));
            }
            List<Texture2D> textures = LIBRARY.LoadImageFiles(pathType, paths);
            LoadedSpriteSheets.Clear();
            foreach (Texture2D texture in textures) {
                texture.SetWrapMode(TextureWrapMode.Clamp)
                    .SetFilterMode(FilterMode.Point)
                    .SetMipmap();
                LoadedSpriteSheets.Add(new AnmSpriteSheetTexture() {
                    spriteSheetId = sheets[textures.IndexOf(texture)].Id,
                    texture = texture
                });
            }
            onLoaded?.Invoke(LoadedSpriteSheets);
            OnFileLoaded.Invoke();
        }

        /// <summary>【异步加载Anm图集贴图】Async load textures</summary>
        /// <param name="sheets">【Anm图集列表】Sprite sheet list</param>
        /// <param name="pathType">【文件路径类型】Path type</param>
        /// <param name="rootFolder">【anm2文件根路径】Root folder path</param>
        /// <param name="onLoaded">【图集加载完成后执行】Callback when sprite sheets loaded</param>
        private async void LoadAnmTexturesAsync(List<AnmSpritesheet> sheets, AnmFilePathType pathType, string rootFolder,
        UnityAction<List<AnmSpriteSheetTexture>> onLoaded = null) {
            List<string> paths = new();
            foreach (AnmSpritesheet ss in sheets) {
                paths.Add(System.IO.Path.Combine(rootFolder, ss.Path));
            }
            List<Texture2D> textures = await LIBRARY.LoadImageFilesAsync(pathType, paths);
            LoadedSpriteSheets.Clear();
            foreach (Texture2D texture in textures) {
                texture.SetWrapMode(TextureWrapMode.Clamp)
                    .SetFilterMode(FilterMode.Point)
                    .SetMipmap();
                LoadedSpriteSheets.Add(new AnmSpriteSheetTexture() {
                    spriteSheetId = sheets[textures.IndexOf(texture)].Id,
                    texture = texture
                });
            }
            onLoaded?.Invoke(LoadedSpriteSheets);
            OnFileLoaded.Invoke(); // 触发图集加载完成事件
        }

        /// <summary>【通过id获取图集】Get sprite sheet texture by ID</summary>
        /// <param name="id">【ID序数】ID</param>
        /// <returns>【图集，存在为空情况】Texture, include null</returns>
        public Texture2D GetSpriteSheetById(int id) {
            foreach (AnmSpriteSheetTexture sheet in LoadedSpriteSheets) {
                if (sheet.spriteSheetId == id) { return sheet.texture; }
            }
            return null;
        }
        #endregion

        #region 初始化 Initialize
        /// <summary>【动画初始化】Animation init</summary>
        /// <param name="findChildLayers">【是否查找图层】Whether find layers</param>
        /// <param name="initAnimationIndex">【使用哪一个序数的动画作为初始化动画(-1为默认)】Use which animation ID as init animation (-1 is default)</param>
        /// <param name="setFrameProps">【是否设置帧属性】Is set frame properties</param>
        public AnmSprite InitAnimation(bool findChildLayers = true, int initAnimationIndex = -1, bool setFrameProps = false, AnmShaderQuality shaderQuality = AnmShaderQuality.Lit_Transparent) {
            if (CurrentAnmFile == null) return this;
            IsInitialized = true; // 初始化完成 | Initialized
            AnmTimerManager.Ins.UnregisterAnmTimer(this); // 取消注册动画计时器 | Cancel register animation timer
            AnmTimerManager.Ins.RegisterAnmTimer(this, CurrentAnmFile.Info.Fps, playBackSpeed); // 注册动画计时器 | Register animation timer
            SetOverrideFrameRate(FrameRate); // 默认帧率设置
            IsPaused = AnmTimerManager.Ins.CheckAnimationPaused(this); // 检查动画是否处于暂停状态 | Check if animation is paused
            InitLayers(findChildLayers, initAnimationIndex, shaderQuality); // 图层初始化 | Layers init
            int _initAnimationIndex = initAnimationIndex == -1 ? CurrentAnmFile.Animations.GetDefaultAnimationIndex() : initAnimationIndex;
            AnmAnimation animation = CurrentAnmFile.Animations.GetAnimationByIndex(_initAnimationIndex); // 获取动画片段 | Get animation clip
            SetCurrentAnimation(animation.Name, 0, true, true);
            if (setFrameProps) {
                SetAnimationLayers(animation, DEFAULT_FRAME_INDEX, true); // 设置动画片段的图层属性 | Set animation clip's layer properties
            }
            return this;
        }

        /// <summary>【图层初始化】Layers init</summary>
        /// <param name="findChildLayers">【是否查找子图层】Is find child layers</param>
        /// <param name="initAnimationIndex">【使用哪一个序数的动画作为初始化动画(-1为默认)】Use which animation ID as init animation (-1 is default)</param>
        private void InitLayers(bool findChildLayers = false, int initAnimationIndex = -1, AnmShaderQuality shaderQuality = AnmShaderQuality.Lit_Transparent) {
            if (CurrentAnmFile == null) return;
            initAnimationIndex = initAnimationIndex == -1 ? CurrentAnmFile.Animations.GetDefaultAnimationIndex() : initAnimationIndex;
            AnmAnimation currentAnimation = CurrentAnmFile.Animations.GetAnimationByIndex(initAnimationIndex); // 获取动画片段
            if (currentAnimation == null) {
                Debug.LogError("[AnmSprite] 获取动画片段失败 | Failed to get animation clip");
                return;
            }
            SetLayerName(BASE_LAYER_NAME);
            // --- 根图层初始化 ---
            bool hasChildRootLayer = GetComponentInChildren<AnmRootLayerRuntime>() != null; // 是否有子根图层
            bool findChildRootLayer = hasChildRootLayer ? findChildLayers : false; // 是否查找子根图层
            if (!findChildRootLayer) { // 不查找子图层
                foreach (var layer in GetComponentsInChildren<AnmRootLayerRuntime>()) {
                    DestroyImmediate(layer.gameObject); // 删除现有图层对象
                }
                SpawnedRootLayer = new GameObject("AnmRoot").GetComponentAroundOrAdd<AnmRootLayerRuntime>();
                SpawnedRootLayer.transform.SetParent(transform);
            }
            else { // 查找子图层
                SpawnedRootLayer = GetComponentInChildren<AnmRootLayerRuntime>();
            }
            SpawnedRootLayer.transform.localPosition = Vector3.zero;
            SpawnedRootLayer.transform.localEulerAngles = Vector3.zero;
            SpawnedRootLayer.transform.localScale = Vector3.one;
            SpawnedRootLayer.SetLayerName(ROOT_LAYER_NAME);
            // --- 图集图层初始化 ---
            bool hasChildSpriteLayers = GetComponentsInChildren<AnmSpriteLayerRuntime>().Length > 0; // 是否有子图集图层
            bool findChildSpriteLayers = hasChildSpriteLayers ? findChildLayers : false; // 是否查找子图集图层
            List<AnmLayer> contentLayers = CurrentAnmFile.Content.Layers; // 动画内容图层列表
            SpawnedSpriteLayers.Clear(); // 清空列表
            if (!findChildSpriteLayers) { // 不查找子图层
                foreach (var layer in GetComponentsInChildren<AnmSpriteLayerRuntime>()) {
                    DestroyImmediate(layer.gameObject); // 删除现有图层对象
                }
                foreach (AnmLayer layer in contentLayers) {
                    string layerName = CurrentAnmFile.Content.GetLayerNameByIndex(layer.Id);
                    string layerDisplayName = $"{layer.Id}. {layerName}";
                    AnmSpriteLayerRuntime newLayerObject = new GameObject(layerDisplayName).AddComponent<AnmSpriteLayerRuntime>();
                    newLayerObject.transform.SetParent(SpawnedRootLayer.transform);
                    newLayerObject.gameObject.GetComponentAroundOrAdd<MeshFilter>().mesh = AnmDataHandler.Ins.RectMesh;
                    newLayerObject.gameObject.GetComponentAroundOrAdd<MeshRenderer>().SetSharedMaterials(new() { AnmDataHandler.Ins.anmHiddenMaterialTemp });
                    newLayerObject.SetLayerSortingId(layer.ImageOrder).SetLayerName(layerName).SetLayerDisplayName(layerDisplayName).SetLayerId(layer.Id); // 设置图层属性
                    newLayerObject.SetShaderQuality(shaderQuality); // 设置材质质量 | Set material quality
                    SpawnedSpriteLayers.Add(newLayerObject); // 添加到已生成的图层列表中
                }
            }
            else { // 查找子图层
                gameObject.GetComponentsInChildren<AnmSpriteLayerRuntime>(true, SpawnedSpriteLayers); // 获取子图层对象 | Get child layer objects
                foreach (AnmLayer layer in contentLayers) {
                    string layerName = CurrentAnmFile.Content.GetLayerNameByIndex(layer.Id);
                    string layerDisplayName = $"{layer.Id}. {layerName}";
                    // 设置参数 | Set props
                    for (int sl = 0; sl < SpawnedSpriteLayers.Count; sl++) {
                        if (SpawnedSpriteLayers[sl].LayerName == layerName && SpawnedSpriteLayers[sl].LayerId == layer.Id) {
                            SpawnedSpriteLayers[sl].gameObject.GetComponentAroundOrAdd<MeshFilter>().mesh = AnmDataHandler.Ins.RectMesh;
                            SpawnedSpriteLayers[sl].gameObject.GetComponentAroundOrAdd<MeshRenderer>().SetSharedMaterials(new() { AnmDataHandler.Ins.anmHiddenMaterialTemp });
                            SpawnedSpriteLayers[sl].SetLayerSortingId(layer.ImageOrder).SetLayerName(layerName).SetLayerDisplayName(layerDisplayName).SetLayerId(layer.Id);
                            break;
                        }
                    }
                }
            }
            // --- 非图集图层初始化 ---
            bool hasChildNullLayers = GetComponentsInChildren<AnmNullLayerRuntime>().Length > 0;
            bool findChildNullLayers = hasChildNullLayers ? findChildLayers : false; // 是否查找子非图集图层
            List<AnmNull> contentNulls = CurrentAnmFile.Content.Nulls; // 动画内容非图集图层列表
            SpawnedNullLayers.Clear(); // 清空列表
            if (!findChildNullLayers) { // 不查找子图层
                foreach (var layer in GetComponentsInChildren<AnmNullLayerRuntime>()) {
                    DestroyImmediate(layer.gameObject); // 删除现有图层对象
                }
                foreach (AnmNull layer in contentNulls) {
                    string layerName = CurrentAnmFile.Content.GetNullNameByIndex(layer.Id);
                    string layerDisplayName = $"{layer.Id}. {layerName}";
                    AnmNullLayerRuntime newLayerObject = new GameObject(layerDisplayName).AddComponent<AnmNullLayerRuntime>();
                    newLayerObject.transform.SetParent(SpawnedRootLayer.transform);
                    newLayerObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    newLayerObject.SetLayerName(layerName).SetLayerDisplayName(layerDisplayName).SetLayerId(layer.Id); // 设置图层属性
                    SpawnedNullLayers.Add(newLayerObject); // 添加到已生成的图层列表中
                }
            }
            else { // 查找子图层
                gameObject.GetComponentsInChildren<AnmNullLayerRuntime>(true, SpawnedNullLayers); // 获取子图层对象 | Get child layer objects
                foreach (AnmNull layer in contentNulls) {
                    string layerName = CurrentAnmFile.Content.GetNullNameByIndex(layer.Id);
                    string layerDisplayName = $"{layer.Id}. {layerName}";
                    // 设置参数 | Set props
                    for (int sl = 0; sl < SpawnedNullLayers.Count; sl++) {
                        if (SpawnedNullLayers[sl].LayerName == layerName && SpawnedNullLayers[sl].LayerId == layer.Id) {
                            SpawnedNullLayers[sl].SetLayerName(layerName).SetLayerDisplayName(layerDisplayName).SetLayerId(layer.Id);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>【设置方位数据】Set transform props</summary>
        /// <param name="parent">【父级】Transform parent</param>
        /// <param name="localPosition">【本地坐标】Local position</param>
        /// <param name="localRotation">【本地旋转】Local rotation</param>
        /// <param name="localScale">【本地缩放】Local scale</param>
        public AnmSprite SetTransform(Transform parent, Vector3 localPosition = default, Quaternion localRotation = default, Vector3 localScale = default) {
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            transform.localScale = localScale;
            return this;
        }

        /// <summary>【将Anm文件添加至缓存管理器】</summary>
        public AnmSprite SetCache() {
            if (CurrentAnmFile == null) return this;
            if (AnmCacheManager.Ins == null) {
                new GameObject("AnmCacheManager").AddComponent<AnmCacheManager>();
            }
            if (autoLoadFilePathType == AnmFilePathType.Component) {
                AnmCacheManager.Ins.AddCacheFile(autoLoadAnmFileAsset.name, CurrentAnmFile, LoadedSpriteSheets);
            }
            else {
                AnmCacheManager.Ins.AddCacheFile(new AnmCachePath(autoLoadFilePathType, autoLoadFilePath), CurrentAnmFile, LoadedSpriteSheets);
            }
            return this;
        }
        #endregion

        #region 动画帧更新 Update frames
        /// <summary>【帧更新计算】Frame Update, call by AnmTimerManager</summary>
        public void FrameUpdateEvaluate(bool updateFrameIndex) {
            if (CurrentAnmFile == null) return;
            if (CurrentAnmFile.Animations.AnimationList.Count <= 0) return;
            AnmAnimation currentAnimation = CurrentAnmFile.Animations.GetAnimationByIndex(currentAnimationIndex); // 获取当前动画 | Get current animation
            int currentAnimationFrameCount = currentAnimation.FrameNum; // 获取当前动画总帧数 | Get current animation frame count
            // 帧数更新 | Frame index update
            if (updateFrameIndex) {
                if (!IsReversed) { // 正向播放 | Play forward
                    // 获取当前动画的循环状态 | Get the loop status of the current animation
                    bool isLoop = CurrentAnmFile.Animations.GetAnimationByName(GetCurrentAnimationName()).Loop;
                    if (currentFrameIndex >= currentAnimationFrameCount - 1) { // 当为最后一帧时 | When it's the last frame
                        OnAnimationStarted.Invoke(GetCurrentAnimationName(), GetCurrentAnimationIndex()); // 动画开始 | Animation started
                        SetCurrentFrameIndex(isLoop ? 0 + JumpFrames : 0);
                    }
                    else if (currentFrameIndex < currentAnimationFrameCount - 1 - 1 - JumpFrames) { // 当前于倒数第二帧时 | When before the second-to-last frame
                        SetCurrentFrameIndex(currentFrameIndex + 1 + JumpFrames);
                    }
                    else { // 当为倒数第二帧时 | When at the second-to-last frame
                        OnAnimationCompleted.Invoke(GetCurrentAnimationName(), GetCurrentAnimationIndex(), GetCurrentAnimationDesc().Loop); // 动画完成 | Animation completed
                        if (!isLoop) { Pause(); }
                        SetCurrentFrameIndex(currentAnimationFrameCount - 1);
                        if (!isLoop) { return; }
                    }
                }
                else { // 反向播放 | Play backward
                    // 获取当前动画的循环状态 | Get the loop status of the current animation
                    bool isLoop = CurrentAnmFile.Animations.GetAnimationByName(GetCurrentAnimationName()).Loop;
                    if (currentFrameIndex <= 0) { // 当为第一帧时 | When it's the first frame
                        OnAnimationStarted.Invoke(GetCurrentAnimationName(), GetCurrentAnimationIndex()); // 动画开始 | Animation started
                        SetCurrentFrameIndex(isLoop ? currentAnimationFrameCount - 1 - JumpFrames : currentAnimationFrameCount - 1);
                    }
                    else if (currentFrameIndex > 0 + 1 + JumpFrames) { // 当后于第二帧时 | When after the second frame
                        SetCurrentFrameIndex(currentFrameIndex - 1 - JumpFrames);
                    }
                    else { // 当第二帧时 | When the first frame
                        OnAnimationCompleted.Invoke(GetCurrentAnimationName(), GetCurrentAnimationIndex(), GetCurrentAnimationDesc().Loop); // 动画完成 | Animation completed
                        if (!isLoop) { Pause(); }
                        SetCurrentFrameIndex(0);
                        if (!isLoop) { return; }
                    }
                }
            }
            OnFrameUpdate.Invoke(currentFrameIndex / (float)currentAnimation.FrameNum); // 触发帧更新事件
        }

        /// <summary>【设置动画层帧】</summary>
        /// <param name="animation">【动画】Animation</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <param name="updateBounds">【是否更新边界】Update bounds</param>
        public AnmSprite SetAnimationLayers(AnmAnimation animation, int frameIndex, bool updateBounds) {
            if (CurrentAnmFile == null) return this;
            if (animation == null) return this;
            // 根图层帧更新
            AnmRootAnimation currentRootAnimation = animation.RootAnimation;
            if (SpawnedRootLayer == null) return this;
            SpawnedRootLayer.SetFrameProperty(currentRootAnimation.GetFrameByFrameIndex(frameIndex));
            // 图集图层帧更新
            List<AnmLayerAnimation> currentLayerAnimations = animation.LayerAnimations;
            for (int la = 0; la < currentLayerAnimations.Count; la++) {
                Texture2D sheet = GetSpriteSheetById(CurrentAnmFile.Content.GetSpriteSheetIndexByLayerId(currentLayerAnimations[la].LayerId)); // 获取图集
                for (int sl = 0; sl < SpawnedSpriteLayers.Count; sl++) {
                    if (SpawnedSpriteLayers[sl].LayerId == currentLayerAnimations[la].LayerId) {
                        SpawnedSpriteLayers[sl].SetLayerFrameProperty(sheet, currentLayerAnimations[la]
                            .GetFrameByFrameIndex(frameIndex), currentLayerAnimations[la].Visible, updateBounds)
                            .SetMute((!currentLayerAnimations[la].Visible) || (currentLayerAnimations[la].AllFrames.Count <= 0)); // 获取图层帧数
                    }
                }
            }
            // 非图集图层帧更新
            List<AnmNullAnimation> currentNullAnimations = animation.NullAnimations;
            for (int na = 0; na < currentNullAnimations.Count; na++) {
                for (int sl = 0; sl < SpawnedNullLayers.Count; sl++) {
                    if (SpawnedNullLayers[sl].LayerId == currentNullAnimations[na].NullId) {
                        SpawnedNullLayers[sl].SetFrameProperty(currentNullAnimations[na]
                            .GetFrameByFrameIndex(frameIndex))
                            .SetMute((!currentNullAnimations[na].Visible) || (currentNullAnimations[na].AllFrames.Count <= 0)); // 获取非图集图层帧数
                    }
                }
            }
            // 事件更新
            isCurrentFrameTriggering = CurrentAnmFile.CheckFrameTriggered(currentAnimationIndex, -1, frameIndex, out string triggeredEventName);
            if (triggeredEventName != string.Empty) {
                OnEventTriggered.Invoke(triggeredEventName);
            }
            return this;
        }

        /// <summary>【当前帧刷新但不更新帧数】Update current frame but frame index</summary>
        /// <param name="updateBounds">【是否更新边界】Update bounds</param>
        public AnmSprite EvaluateCurrentFrame(bool updateBounds) {
            if (CurrentAnmFile == null) return this;
            if (CurrentAnmFile.Animations.AnimationList.Count <= 0) return this;
            // AnmAnimation currentAnimation = CurrentAnmFile.Animations.GetAnimationByIndex(currentAnimationIndex); // 获取当前动画 | Get current animation
            if (CurrentAnimationDesc == null) return this;
            SetAnimationLayers(CurrentAnimationDesc, currentFrameIndex, updateBounds); // 设置动画层
            return this;
        }
        #endregion

        #region 播放方法 Playback methods
        /// <summary>【恢复播放】Unpause (play) animation</summary>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        public AnmSprite Play(int delayExecutionFrames = 0) {
            void play() {
                if (!IsInitialized) { return; }
                AnmTimerManager.Ins.ResumeAnimation(this);
                isPaused = false;
                //
                AnmAnimation currentAnimation = CurrentAnmFile.Animations.GetAnimationByIndex(currentAnimationIndex); // 获取当前动画 | Get current animation
                int currentAnimationFrameCount = currentAnimation.FrameNum; // 获取当前动画总帧数 | Get current animation frame count
                //
                if ((GetCurrentFrameIndex() == 0 && !IsReversed) || (GetCurrentFrameIndex() == currentAnimationFrameCount - 1 && IsReversed)) {
                    OnAnimationStarted.Invoke(GetCurrentAnimationName(), GetCurrentAnimationIndex()); // 动画开始 | Animation started
                }
            }
            // 延迟执行 | Delay
            if (delayExecutionFrames == 0) {
                play();
            }
            else if (delayExecutionFrames > 0) {
                LIBRARY.AnmTASK.Delay(delayExecutionFrames, play);
            }
            return this;
        }

        /// <summary>【设置反转播放 | Set Reverse】</summary>
        /// <param name="isReversed">【是否翻转】Is Reversed</param>
        public AnmSprite SetReverse(bool isReversed) {
            if (!IsInitialized) { return this; }
            IsReversed = isReversed;
            return this;
        }

        /// <summary>【设置帧间隔】Set Frame Gaps</summary>
        /// <param name="frameGaps">【帧间隔】Frame gap</param>
        public AnmSprite SetJumpFrames(int frameGaps) {
            if (!IsInitialized) { return this; }
            JumpFrames = frameGaps;
            return this;
        }

        /// <summary>【从头部正向播放】Play forward from start</summary>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        public AnmSprite PlayForwardFromStart(int delayExecutionFrames = 0) {
            if (!IsInitialized) { return this; }
            SetCurrentFrameIndex(0, delayExecutionFrames);
            SetReverse(false);
            Play(delayExecutionFrames);
            return this;
        }

        /// <summary>【从尾部反向播放】Play backward from end</summary>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        public AnmSprite PlayBackwardFromEnd(int delayExecutionFrames = 0) {
            if (!IsInitialized) { return this; }
            SetCurrentFrameIndex(CurrentAnmFile.Animations.GetAnimationByName(GetCurrentAnimationName()).FrameNum - 1, 1);
            SetReverse(true);
            Play(delayExecutionFrames);
            return this;
        }

        /// <summary>【暂停播放】Pause animation</summary>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        public AnmSprite Pause(int delayExecutionFrames = 0) {
            void pause() {
                if (!IsInitialized) { return; }
                AnmTimerManager.Ins.PauseAnimation(this);
                isPaused = true;
            }
            // 延迟执行 | Delay
            if (delayExecutionFrames == 0) {
                pause();
            }
            else if (delayExecutionFrames > 0) {
                LIBRARY.AnmTASK.Delay(delayExecutionFrames, pause);
            }
            return this;
        }

        /// <summary>【停止播放】Stop playing</summary>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        public AnmSprite Stop(int delayExecutionFrames = 0) {
            void stop() {
                if (!IsInitialized) { return; }
                RefreshLayersVisible();
                Pause();
                SetCurrentFrameIndex(DEFAULT_FRAME_INDEX);
                isCurrentFrameTriggering = false; // 恢复帧事件触发标识符
                SetAnimationLayers(CurrentAnmFile.Animations.GetAnimationByIndex(currentAnimationIndex), currentFrameIndex, true);
            }
            // 延迟执行 | Delay
            if (delayExecutionFrames == 0) {
                stop();
            }
            else if (delayExecutionFrames > 0) {
                LIBRARY.AnmTASK.Delay(delayExecutionFrames, stop);
            }
            return this;
        }

        /// <summary>【清除数据】Clear data</summary>
        public AnmSprite Clear() {
            AnmTimerManager.Ins.UnregisterAnmTimer(this); // 取消注册动画计时器 | Cancel register animation timer
            isPaused = true;
            foreach (var layer in GetComponentsInChildren<AnmRootLayerRuntime>()) {
                DestroyImmediate(layer.gameObject); // 删除现有图层对象 | Delete existing layer object
            }
            foreach (var layer in GetComponentsInChildren<AnmSpriteLayerRuntime>()) {
                DestroyImmediate(layer.gameObject); // 删除现有图层对象 | Delete existing layer object
            }
            foreach (var layer in GetComponentsInChildren<AnmNullLayerRuntime>()) {
                DestroyImmediate(layer.gameObject); // 删除现有图层对象 | Delete existing layer object
            }
            currentAnimationIndex = 0; // 恢复默认播放的动画片段索引 | Restore the default animation fragment index
            OnAnimationChanged.Invoke(CurrentAnmFile.Animations.GetDefaultAnimationName(), CurrentAnmFile.Animations.GetDefaultAnimationIndex());
            currentFrameIndex = DEFAULT_FRAME_INDEX; // 恢复帧序数 | Restore frame serial number
            isCurrentFrameTriggering = false; // 恢复帧事件触发标识符 | Restore frame event triggering identifier
            CurrentAnmFile = new();
            LoadedSpriteSheets.Clear();
            SpawnedRootLayer = null;
            SpawnedSpriteLayers.Clear();
            SpawnedNullLayers.Clear();
            IsInitialized = false; // 恢复初始化标识符 | Restore initialization identifier
            return this;
        }
        #endregion

        #region 帧率方法 Frame rate methods
        /// <summary>【设置帧率等级配置】Set frame dropping level config</summary>
        /// <param name="FDL_config">【配置文件】Config file</param>
        public AnmSprite SetFrameDroppingLevelsConfig(AnmFrameDroppingLevels FDL_config = null) {
            frameDroppingLevels = FDL_config == null ? AnmDataHandler.Ins.defaultFrameDroppingLevels : FDL_config;
            return this;
        }

        /// <summary>【设置帧率等级】Set frame rate level</summary>
        /// <param name="level">【等级】Level</param>
        public AnmSprite SetFrameDroppingLevel(int level) {
            UseFrameDroppingLevels = true;
            if (frameDroppingLevels != null) {
                frameDroppingLevels.GetFrameDroppingLevel(level, out FDL_jumpFrames, out FDL_frameRate); // 获取帧率等级 | Get frame rate level
                SetOverrideFrameRate(FDL_frameRate);
                frameDroppingLevel = level;
            }
            return this;
        }

        /// <summary>【设置使用帧率等级】Set use frame rate level</summary>
        /// <param name="isOn">【是否启用】</param>
        public AnmSprite SetUseFrameDroppingLevels(bool isOn = true) {
            UseFrameDroppingLevels = isOn;
            if (isOn && frameDroppingLevels != null) {
                frameDroppingLevels.GetFrameDroppingLevel(frameDroppingLevel, out FDL_jumpFrames, out FDL_frameRate); // 获取帧率等级 | Get frame rate level
            }
            SetOverrideFrameRate(FrameRate);
            return this;
        }

        /// <summary>【设置帧率等级】Set frame rate level</summary>
        /// <param name="FDL_config">【配置文件】Config file</param>
        /// <param name="level">【等级】Level</param>
        /// <param name="isOn">【是否启用】</param>
        public AnmSprite SetFrameDroppingLevel(AnmFrameDroppingLevels FDL_config, int level) {
            if (FDL_config == null) {
                FDL_config = AnmDataHandler.Ins.defaultFrameDroppingLevels;
            }
            SetFrameDroppingLevelsConfig(FDL_config);
            SetFrameDroppingLevel(level);
            return this;
        }
        #endregion

        #region 动画方法 Animation methods
        /// <summary>【获取当前播放动画名称】Get animation name</summary>
        /// <param name="animationIndex">【动画索引】Index</param>
        public string GetCurrentAnimationName() {
            if (CurrentAnmFile == null) return "";
            return CurrentAnmFile.Animations.GetAnimationNameByIndex(currentAnimationIndex);
        }

        /// <summary>【获取当前播放动画序数】Get animation ID</summary>
        /// <returns>【动画索引】Index</returns>
        public int GetCurrentAnimationIndex() {
            return currentAnimationIndex;
        }

        /// <summary>【获取当前播放动画的原始信息】Get animation description info</summary>
        /// <returns>【动画原始信息】Animation desc</returns>
        public AnmAnimation GetCurrentAnimationDesc() {
            if (CurrentAnmFile == null) return null;
            return CurrentAnmFile.Animations.GetAnimationByIndex(currentAnimationIndex);
        }

        /// <summary>【设置当前播放动画】Set animation</summary>
        /// <param name="animationName">【动画名称】Name</param>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        /// <param name="resetCurrentFrame">【是否恢复初始动画进度】Is reset progress</param>
        /// <param name="refreshLayersVisible">【是否刷新所有图集图层的显示状态】Is reset progress</param>
        public AnmSprite SetCurrentAnimation(string animationName, int delayExecutionFrames = 0, bool resetCurrentFrame = true, bool refreshLayersVisible = true) {
            if (CurrentAnmFile == null) return this;
            return SetCurrentAnimation(CurrentAnmFile.Animations.GetAnimationIndexByName(animationName), delayExecutionFrames, resetCurrentFrame, refreshLayersVisible);
        }

        /// <summary>【设置当前播放动画】Set animation</summary>
        /// <param name="animationIndex">【动画索引】Index</param>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        /// <param name="resetCurrentFrame">【是否恢复初始动画进度】Is reset progress</param>
        /// <param name="refreshLayersVisible">【是否刷新所有图集图层的显示状态】Is reset progress</param>
        public AnmSprite SetCurrentAnimation(int animationIndex, int delayExecutionFrames = 0, bool resetCurrentFrame = true, bool refreshLayersVisible = true) {
            void setCurrentAnimation() {
                if (CurrentAnmFile == null || !IsInitialized) return;
                currentAnimationIndex = animationIndex == -1 ? CurrentAnmFile.Animations.GetDefaultAnimationIndex() : animationIndex;
                LayerSortingUpdate(); // 图层排序更新 | Layer sorting update
                LayerTransformReset(resetBound: false); // 图层方位重置 | Layer transform reset
                if (resetCurrentFrame) {
                    SetCurrentFrameIndex(DEFAULT_FRAME_INDEX);
                }
                if (refreshLayersVisible) {
                    RefreshLayersVisible();
                }
                CurrentAnmFile.Animations.GetAnimationByIndex(currentAnimationIndex);//.BakeLayerOrders(); // 计算排序 | Bake all layer's order info
                CurrentAnimationDesc = GetCurrentAnimationDesc();
                EvaluateCurrentFrame(true); // 刷新当前帧 | Refresh current frame
                OnAnimationChanged.Invoke(GetCurrentAnimationName(), GetCurrentAnimationIndex()); // 动画片段切换事件 | Animation fragment changed event
            }
            // 延迟执行 | Delay
            if (delayExecutionFrames == 0) {
                setCurrentAnimation();
            }
            else if (delayExecutionFrames > 0) {
                LIBRARY.AnmTASK.Delay(delayExecutionFrames, setCurrentAnimation);
            }
            return this;
        }

        /// <summary>【刷新所有图集图层的显示状态】</summary>
        private void RefreshLayersVisible() {
            SpawnedSpriteLayers.ForEach(layer => { layer.IsVisible = false; layer.SetMute(true); }); // 刷新所有图集图层的显示状态 | Refresh all spritesheet layers' visibility
        }
        #endregion

        #region 图层方法 Layer methods
        /// <summary>【更新图层排序】Update layer sorting</summary>
        public AnmSprite LayerSortingUpdate() {
            if (CurrentAnmFile == null) return this;
            AnmAnimation currentAnimation = GetCurrentAnimationDesc(); // 获取当前动画片段
            // --- 图集图层顺序更新 ---
            List<AnmLayer> contentLayers = CurrentAnmFile.Content.Layers; // 动画内容图层列表
            List<AnmLayerAnimation> currentAnimlayers = currentAnimation.LayerAnimations; // 当前动画的图集图层列表
            List<AnmLayerAnimation> reversedAnimLayers = new(); // 反向图集图层列表
            for (int i = currentAnimlayers.Count - 1; i >= 0; i--) {
                reversedAnimLayers.Add(currentAnimlayers[i]);
            }
            List<AnmLayer> sortedLayers = new(); // 根据currentAnimlayers的顺序进行重排序的contentLayers
            foreach (AnmLayerAnimation layer in currentAnimlayers) {
                AnmLayer contentLayer = contentLayers.Find(x => x.Id == layer.LayerId);
                if (contentLayer != null) {
                    sortedLayers.Add(contentLayer);
                }
            }
            List<AnmSpriteLayerRuntime> childrenSpriteLayers = gameObject.GetComponentsInChildren<AnmSpriteLayerRuntime>().ToList(); // 获取子图层
            // 根据reversedAnimLayers的ID顺序来对childrenSpriteLayers子图层的SiblingIndex进行排序
            for (int i = 0; i < reversedAnimLayers.Count; i++) {
                AnmSpriteLayerRuntime spriteLayer = childrenSpriteLayers.Find(x => x.LayerId == reversedAnimLayers[i].LayerId);
                if (spriteLayer != null) {
                    spriteLayer.transform.SetSiblingIndex(i);
                    spriteLayer.SetLayerSortingId(reversedAnimLayers.Count - 1 - i);
                }
            }
            // --- 非图集图层顺序更新 ---
            List<AnmNull> contentNulls = CurrentAnmFile.Content.Nulls; // 动画内容非图集图层列表
            List<AnmNullAnimation> currentNullLayers = currentAnimation.NullAnimations; // 当前动画的非图集图层列表
            List<AnmNullAnimation> reversedNullLayers = new(); // 反向非图集图层列表
            for (int i = currentNullLayers.Count - 1; i >= 0; i--) {
                reversedNullLayers.Add(currentNullLayers[i]);
            }
            List<AnmNull> sortedNulls = new(); // 根据currentNullLayers的顺序进行重排序的contentNulls
            foreach (AnmNullAnimation layer in currentNullLayers) {
                AnmNull contentLayer = contentNulls.Find(x => x.Id == layer.NullId);
                if (contentLayer != null) {
                    sortedNulls.Add(contentLayer);
                }
            }
            List<AnmNullLayerRuntime> childrenNullLayers = gameObject.GetComponentsInChildren<AnmNullLayerRuntime>().ToList(); // 获取子非图层
            // 根据reversedNullLayers的ID顺序来对childrenNullLayers子非图层的SiblingIndex进行排序
            for (int i = 0; i < reversedNullLayers.Count; i++) {
                AnmNullLayerRuntime nullLayer = childrenNullLayers.Find(x => x.LayerId == reversedNullLayers[i].NullId);
                if (nullLayer != null) {
                    nullLayer.transform.SetSiblingIndex(reversedNullLayers.Count - 1 - i + childrenSpriteLayers.Count);
                }
            }
            return this;
        }

        /// <summary>【重置图层方位和边界】Reset layer transform and bounds</summary>
        /// <param name="resetZOrder">是否重置Z轴顺序 | Whether to reset Z order</param>
        /// <param name="resetBound">是否重置边界 | Whether to reset bounds</param>
        public AnmSprite LayerTransformReset(bool resetZOrder = false, bool resetBound = false) {
            // 重置子图层的位置旋转缩放
            List<AnmSpriteLayerRuntime> childrenSpriteLayers = gameObject.GetComponentsInChildren<AnmSpriteLayerRuntime>().ToList(); // 获取子图层
            foreach (AnmSpriteLayerRuntime spriteLayer in childrenSpriteLayers) {
                if (resetBound) {
                    spriteLayer.ResetLayerBounds(); // 重置MeshFilter及渲染器的渲染边界
                }
                spriteLayer.transform.localPosition = resetZOrder ? Vector3.zero : new Vector3(0, 0, spriteLayer.transform.localPosition.z);
                spriteLayer.transform.localRotation = Quaternion.identity;
                spriteLayer.transform.localScale = Vector3.zero;
            }
            List<AnmNullLayerRuntime> childrenNullLayers = gameObject.GetComponentsInChildren<AnmNullLayerRuntime>().ToList(); // 获取子非图层
            foreach (AnmNullLayerRuntime nullLayer in childrenNullLayers) {
                nullLayer.transform.localPosition = resetZOrder ? Vector3.zero : new Vector3(0, 0, nullLayer.transform.localPosition.z);
                nullLayer.transform.localRotation = Quaternion.identity;
                nullLayer.transform.localScale = Vector3.zero;
            }
            return this;
        }

        /// <summary>【获取根图层】Get root layer</summary>
        public AnmRootLayerRuntime GetRootLayer() {
            return SpawnedRootLayer;
        }

        /// <summary>【获取Sprite图集图层】Get sprite layer</summary>
        /// <param name="layerName">【图层名称】Name</param>
        public AnmSpriteLayerRuntime GetSpriteLayer(string layerName) {
            for (int i = 0; i < SpawnedSpriteLayers.Count; i++) {
                if (SpawnedSpriteLayers[i].LayerName == layerName) {
                    return SpawnedSpriteLayers[i];
                }
            }
            return null;
        }

        /// <summary>【获取Sprite图集图层】Get sprite layer</summary>
        /// <param name="layerIndex">【图层索引】Index</param>
        public AnmSpriteLayerRuntime GetSpriteLayer(int layerIndex) {
            if (layerIndex < SpawnedSpriteLayers.Count) {
                return SpawnedSpriteLayers[layerIndex];
            }
            return null;
        }

        /// <summary>【获取Null非图集图层】Get null layer</summary>
        /// <param name="layerName">【图层名称】Name</param>
        public AnmNullLayerRuntime GetNullLayer(string layerName) {
            for (int i = 0; i < SpawnedNullLayers.Count; i++) {
                if (SpawnedNullLayers[i].LayerName == layerName) {
                    return SpawnedNullLayers[i];
                }
            }
            return null;
        }

        /// <summary>【获取Null非图集图层】Get null layer</summary>
        /// <param name="layerIndex">【图层索引】Index</param>
        public AnmNullLayerRuntime GetNullLayer(int layerIndex) {
            if (layerIndex < SpawnedNullLayers.Count) {
                return SpawnedNullLayers[layerIndex];
            }
            return null;
        }
        #endregion

        #region 帧方法 Frame methods
        /// <summary>【获取当前帧索引】Get frame index</summary>
        public int GetCurrentFrameIndex() {
            return currentFrameIndex;
        }

        /// <summary>【设置当前帧索引】Set frame index</summary>
        /// <param name="delayExecutionFrames">【延迟执行】Delay frames</param>
        public AnmSprite SetCurrentFrameIndex(int frameIndex, int delayExecutionFrames = 0) {
            void setCurrentFrameIndex() {
                if (!IsInitialized) return;
                currentFrameIndex = frameIndex;
                EvaluateCurrentFrame(false); // 刷新当前帧 | Refresh current frame
                OnFrameIndexChanged.Invoke(currentFrameIndex);
            }
            // 延迟执行 | Delay
            if (delayExecutionFrames == 0) {
                setCurrentFrameIndex();
            }
            else if (delayExecutionFrames > 0) {
                LIBRARY.AnmTASK.Delay(delayExecutionFrames, setCurrentFrameIndex);
            }
            return this;
        }

        /// <summary>【设置重载帧率】Set override frame rate</summary>
        /// <param name="frameRate">【帧速率，0为默认】Fps, 0 is default</param>
        public AnmSprite SetOverrideFrameRate(float frameRate) {
            if (CurrentAnmFile == null) return this;
            AnmTimerManager.Ins.SetAnimationFrameRate(this, frameRate != 0f ? frameRate : CurrentAnmFile.Info.Fps);
            return this;
        }

        /// <summary>【设置为默认帧率】Set frame rate to default</summary>
        public AnmSprite SetFrameRateToDefault() {
            if (CurrentAnmFile == null) return this;
            AnmTimerManager.Ins.SetAnimationFrameRate(this, CurrentAnmFile.Info.Fps);
            return this;
        }
        #endregion

        #region 动画时间方法 Animation time methods
        /// <summary>【获取播放速度】Get speed</summary>
        public float GetPlayBackSpeed() {
            return playBackSpeed;
        }

        /// <summary>【设置播放速度】Set speed</summary>
        /// <param name="speed">【速度乘数】Speed mult</param>
        public AnmSprite SetPlayBackSpeed(float speed) {
            playBackSpeed = speed;
            AnmTimerManager.Ins.SetAnimationSpeed(this, speed);
            return this;
        }

        /// <summary>【获取帧间隔】Get frame delta time</summary>
        public float GetFrameInterval() {
            if (CurrentAnmFile == null) { return 1f / 30f; }
            return 1f / CurrentAnmFile.Info.Fps;
        }
        #endregion

        #region 帧事件方法 Frame event methods
        /// <summary>【检测当前帧是否触发某事件】Detect whether current frame triggered</summary>
        /// <param name="eventIndex">【事件序数】Event index</param>
        /// <returns>【检测结果】</returns>
        public bool CheckEventTriggered(int eventIndex) {
            if (CurrentAnmFile == null) return false;
            if (isCurrentFrameTriggering == false) { return false; }
            return CurrentAnmFile.CheckFrameTriggered(currentAnimationIndex, eventIndex, currentFrameIndex);
        }

        /// <summary>【检测当前帧是否触发某事件】Detect whether current frame triggered</summary>
        /// <param name="eventName">【事件名称】Event name</param>
        /// <returns>【检测结果】</returns>
        public bool CheckEventTriggered(string eventName) {
            if (CurrentAnmFile == null) return false;
            if (isCurrentFrameTriggering == false) { return false; }
            return CurrentAnmFile.CheckFrameTriggered(currentAnimationIndex, eventName, currentFrameIndex);
        }

        /// <summary>【加载完成的事件订阅】Subscribe loaded</summary>
        /// <param name="onLoaded">【事件】Event action [AnmSprite sprite]</param>
        /// <param name="onlyOnce">【是否只执行一次】Only once</param>
        public AnmSprite SubscribeOnFileLoaded(UnityAction<AnmSprite> onLoaded, bool onlyOnce = false) {
            void wrapper() {
                onLoaded(this);
                if (onlyOnce) {
                    OnFileLoaded.RemoveListener(wrapper);
                }
            }
            OnFileLoaded.AddListener(wrapper);
            return this;
        }
        /// <summary>【帧更新事件订阅】Subscribe frame update</summary>
        /// <param name="onUpdate">【事件】Event action  [AnmSprite sprite] [float progress]</param>
        public AnmSprite SubscribeOnFrameUpdate(UnityAction<AnmSprite, float> onUpdate) {
            OnFrameUpdate.AddListener((progress) => onUpdate(this, progress));
            return this;
        }
        /// <summary>【事件触发事件订阅】Subscribe event triggered</summary>
        /// <param name="onTriggered">【事件】Event action [AnmSprite sprite] [string eventName]</param>
        /// <param name="onlyOnce">【是否只执行一次】Only once</param>
        public AnmSprite SubscribeOnEventTriggered(UnityAction<AnmSprite, string> onTriggered, bool onlyOnce = false) {
            void wrapper(string eventName) {
                onTriggered(this, eventName);
                if (onlyOnce) {
                    OnEventTriggered.RemoveListener(wrapper);
                }
            }
            OnEventTriggered.AddListener(wrapper);
            return this;
        }
        /// <summary>【动画开始事件订阅】Subscribe animation started</summary>
        /// <param name="onStarted">【事件】Event action [AnmSprite sprite] [string animationName] [int animationIndex]</param>
        /// <param name="onlyOnce">【是否只执行一次】Only once</param>
        public AnmSprite SubscribeOnAnimationStarted(UnityAction<AnmSprite, string, int> onStarted, bool onlyOnce = false) {
            void wrapper(string animationName, int animationIndex) {
                onStarted(this, animationName, animationIndex);
                if (onlyOnce) {
                    OnAnimationStarted.RemoveListener(wrapper);
                }
            }
            OnAnimationStarted.AddListener(wrapper);
            return this;
        }
        /// <summary>【动画完成事件订阅】Subscribe animation completed</summary>
        /// <param name="onCompleted">【事件】Event action [AnmSprite sprite] [string animationName] [int animationIndex] [bool isLoop]</param>
        /// <param name="onlyOnce">【是否只执行一次】Only once</param>
        public AnmSprite SubscribeOnAnimationCompleted(UnityAction<AnmSprite, string, int, bool> onCompleted, bool onlyOnce = false) {
            void wrapper(string animationName, int animationIndex, bool isLoop) {
                onCompleted(this, animationName, animationIndex, isLoop);
                if (onlyOnce) {
                    OnAnimationCompleted.RemoveListener(wrapper);
                }
            }
            OnAnimationCompleted.AddListener(wrapper);
            return this;
        }
        /// <summary>【动画切换事件订阅】Subscribe animation changed</summary>
        /// <param name="onChanged">【事件】Event action [AnmSprite sprite] [string animationName] [int animationIndex]</param>
        public AnmSprite SubscribeOnAnimationChanged(UnityAction<AnmSprite, string, int> onChanged) {
            OnAnimationChanged.AddListener((animName, animIndex) => onChanged(this, animName, animIndex));
            return this;
        }
        /// <summary>【帧序数切换事件订阅】Subscribe frame index changed</summary>
        /// <param name="onChanged">【事件】Event action [AnmSprite sprite] [int frameIndex]</param>
        public AnmSprite SubscribeOnFrameIndexChanged(UnityAction<AnmSprite, int> onChanged) {
            OnFrameIndexChanged.AddListener((frameIndex) => onChanged(this, frameIndex));
            return this;
        }
        #endregion
    }
}