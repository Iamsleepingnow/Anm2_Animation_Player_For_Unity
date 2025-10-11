using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 第三方开源属性扩展 | Third party attribute extension

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【用于调试Anm文件】Debug Anm File</summary>
    [AddComponentMenu("Anm2Player/Anm Debug")]
    public class AnmDebug : MonoBehaviour
    {
        [InfoBox("autoLoadFilePathType与autoLoadFileRelativePath仅适用于自动加载", EInfoBoxType.Normal)]
        [BoxGroup("Auto Load 自动加载")]
        [SerializeField] public bool autoLoadOnAwake = true; // 是否在Awake时自动加载Anm文件 | Whether to load the Anm file automatically in Awake
        [BoxGroup("Auto Load 自动加载")]
        [SerializeField] public AnmFilePathType autoLoadFilePathType = AnmFilePathType.StreamingAssets; // Anm文件路径类型 | Anm file path type

        private bool __autoLoadFilePathTypeIsComponent => autoLoadFilePathType == AnmFilePathType.Component;  // 是否是Component路径 | Is it Component path?
        private bool __autoLoadFilePathTypeIsNotComponent => autoLoadFilePathType != AnmFilePathType.Component; // 是否不是Component路径 | Is itn't Component path?

        [BoxGroup("Auto Load 自动加载"), ShowIf(EConditionOperator.And, "autoLoadOnAwake", "__autoLoadFilePathTypeIsNotComponent"), ResizableTextArea]
        [SerializeField] public string autoLoadFilePath = "./Sample.anm2"; // Anm文件相对路径 | Anm file relative path
        [BoxGroup("Auto Load 自动加载"), ShowIf(EConditionOperator.And, "autoLoadOnAwake", "__autoLoadFilePathTypeIsComponent")]
        [SerializeField] private AnmFileRuntime autoLoadAnmFile = null;
        [BoxGroup("Log 日志")]
        [SerializeField] public bool debugLog = false; // 是否开启日志 | Whether to open log
        [BoxGroup("Localization 本地化")]
        [SerializeField] public bool printChinese = false; // 是否使用中文输出 | Whether to use Chinese output
        [Foldout("Info 信息")]
        [ResizableTextArea]
        [SerializeField] public string anmFileInfo = "";
        [Foldout("Content 内容")]
        [ResizableTextArea]
        [SerializeField] public string anmFileContent = "";
        [Foldout("AnimationNames 动画名称")]
        [ResizableTextArea]
        [SerializeField] public string anmFileAnimationNames = "";
        [Foldout("Animation 动画")]
        [ResizableTextArea]
        [SerializeField] public string anmFileAnimation = "";

        [Button("Load Manually 手动加载")]
        private void DebugLoadAnmFile() {
            AnmFile anmFile;
            string fullPath = LIBRARY.ConcatPathByPathType(autoLoadFilePathType, autoLoadFilePath);
            if (fullPath != LIBRARY.PATH_TYPE_COMPONENT_KEYWORD) {
                if (!System.IO.File.Exists(fullPath)) return;
                anmFile = LIBRARY.XMLRead<AnmFile>(LIBRARY.ConcatPathByPathType(autoLoadFilePathType, autoLoadFilePath));
                if (anmFile == null) return;
            }
            else {
                if (autoLoadAnmFile == null) return;
                anmFile = autoLoadAnmFile.anmFile.XMLRead<AnmFile>();
                if (anmFile == null) return;
            }
            if (printChinese) {
                DebugAnimFileUseChinese(anmFile, out string _anmFileInfo, out string _anmFileContent, out string _anmFileAnimation);
                if (debugLog) {
                    Debug.Log($"{_anmFileInfo}");
                    Debug.Log($"{_anmFileContent}");
                    Debug.Log($"{anmFileAnimationNames}");
                }
            }
            else {
                DebugAnimFileUseEnglish(anmFile, out string _anmFileInfo, out string _anmFileContent, out string _anmFileAnimation);
                if (debugLog) {
                    Debug.Log($"{_anmFileInfo}");
                    Debug.Log($"{_anmFileContent}");
                    Debug.Log($"{anmFileAnimationNames}");
                }
            }
        }
        [Button("Clear Infos 清除信息")]
        private void ClearDebugInfos() {
            anmFileInfo = "";
            anmFileContent = "";
            anmFileAnimationNames = "";
            anmFileAnimation = "";
        }

        void Awake() {
            if (autoLoadOnAwake) {
                DebugLoadAnmFile();
            }
        }

        /// <summary>【加载Anm文件】Load Anm file</summary>
        /// <param name="pathType">【文件路径类型】Path type</param>
        /// <param name="path">【文件相对路径】path</param>
        public void LoadAnmFile(AnmFilePathType pathType, string path) {
            autoLoadOnAwake = false;
            autoLoadFilePathType = pathType;
            autoLoadFilePath = path;
            DebugLoadAnmFile();
        }

        /// <summary>【加载Anm文件】Load Anm file</summary>
        /// <param name="file">【文件】File</param>
        /// <param name="useChinese">【使用中文】Use Chinese</param>
        /// <param name="useDebugLog">【使用日志】Use debug</param>
        public void LoadAnmFile(AnmFile file, bool useChinese, bool useDebugLog) {
            if (useChinese) {
                DebugAnimFileUseChinese(file, out string _anmFileInfo, out string _anmFileContent, out string _anmFileAnimation);
                if (useDebugLog) {
                    Debug.Log($"{_anmFileInfo}");
                    Debug.Log($"{_anmFileContent}");
                    Debug.Log($"{anmFileAnimationNames}");
                }
            }
            else {
                DebugAnimFileUseEnglish(file, out string _anmFileInfo, out string _anmFileContent, out string _anmFileAnimation);
                if (useDebugLog) {
                    Debug.Log($"{_anmFileInfo}");
                    Debug.Log($"{_anmFileContent}");
                    Debug.Log($"{anmFileAnimationNames}");
                }
            }
        }

        private void DebugAnimFileUseEnglish(AnmFile file, out string fileInfo, out string fileContent, out string fileAnimation) {
            anmFileInfo = "";
            anmFileInfo += "--- Infomations ---";
            AnmInfo info = file.Info;
            anmFileInfo += $"\nAuthor：{info.CreatedBy} \nCreate date：{info.CreatedOn} \nVersion：{info.Version} \nFrame rate：{info.Fps}";
            fileInfo = anmFileInfo;
            //
            anmFileContent = "";
            anmFileContent += "--- Contents ---";
            for (int i = 0; i < file.Content.Spritesheets.Count; i++) {
                AnmSpritesheet sheet = file.Content.Spritesheets[i];
                anmFileContent += $"\n[Sprite sheet] {i + 1}. Path：{sheet.Path} | Id：{sheet.Id}";
            }
            for (int i = 0; i < file.Content.Layers.Count; i++) {
                AnmLayer layer = file.Content.Layers[i];
                anmFileContent += $"\n[Layer] {i + 1}. Name：{layer.Name} | Id：{layer.Id} | Sprite sheet Id：{layer.SpritesheetId}";
            }
            for (int i = 0; i < file.Content.Nulls.Count; i++) {
                AnmNull nullObject = file.Content.Nulls[i];
                anmFileContent += $"\n[Null] {i + 1}. Name：{nullObject.Name} | Id：{nullObject.Id} | Show rect：{nullObject.ShowRect}";
            }
            for (int i = 0; i < file.Content.Events.Count; i++) {
                AnmEvent eventObject = file.Content.Events[i];
                anmFileContent += $"\n[Event] {i + 1}. Name：{eventObject.Name} | Id：{eventObject.Id}";
            }
            fileContent = anmFileContent;
            //
            anmFileAnimationNames = "";
            anmFileAnimation = "";
            anmFileAnimation += "--- Animations ---";
            AnmAnimations animations = file.Animations;
            anmFileAnimation += $"\nDefault：{animations.DefaultAnimation}";
            for (int i = 0; i < animations.AnimationList.Count; i++) {
                AnmAnimation animation = animations.AnimationList[i];
                anmFileAnimationNames += $"ID: {i} NAME: {animation.Name}\n";
                anmFileAnimation += $"\n    --- Clip{i + 1}. Name：{animation.Name} | Frame count：{animation.FrameNum} | Is loop：{animation.Loop} ---";
                anmFileAnimation += "\n    |Root Animation|";
                AnmRootAnimation rootAnimation = animation.RootAnimation;
                for (int a = 0; a < rootAnimation.Frames.Count; a++) {
                    AnmFrame frame = rootAnimation.Frames[a];
                    anmFileAnimation += $"\n    Frame{a + 1}. Position：({frame.XPosition},{frame.YPosition}) | Scale：({frame.XScale},{frame.YScale}) | Delay：{frame.Delay} | Visible：{frame.Visible} | Tint：({frame.RedTint},{frame.GreenTint},{frame.BlueTint},{frame.AlphaTint}) | Offset：({frame.RedOffset},{frame.GreenOffset},{frame.BlueOffset}) | Rotation：{frame.Rotation} | Interpolated：{frame.Interpolated}";
                }
                anmFileAnimation += "\n    |Layer Animations|";
                List<AnmLayerAnimation> layerAnimations = animation.LayerAnimations;
                for (int a = 0; a < layerAnimations.Count; a++) {
                    AnmLayerAnimation layerAnimation = layerAnimations[a];
                    anmFileAnimation += $"\n        [Layer] Id：{layerAnimation.LayerId} | Visible：{layerAnimation.Visible}";
                    for (int b = 0; b < layerAnimation.Frames.Count; b++) {
                        AnmLayerFrame frame = layerAnimation.Frames[b];
                        anmFileAnimation += $"\n        Frame{a + 1}. Position：({frame.XPosition},{frame.YPosition}) | Pivot：({frame.XPivot},{frame.YPivot}) | Crop：({frame.XCrop},{frame.YCrop}) | Crop size：({frame.Width},{frame.Height}) | Scale：({frame.XScale},{frame.YScale}) | Delay：{frame.Delay} | Visible：{frame.Visible} | Tint：({frame.RedTint},{frame.GreenTint},{frame.BlueTint},{frame.AlphaTint}) | Offset：({frame.RedOffset},{frame.GreenOffset},{frame.BlueOffset}) | Rotation：{frame.Rotation} | Interpolated：{frame.Interpolated}";
                    }
                }
                anmFileAnimation += "\n    |Null Animations|";
                List<AnmNullAnimation> nullAnimations = animation.NullAnimations;
                for (int a = 0; a < nullAnimations.Count; a++) {
                    AnmNullAnimation nullAnimation = nullAnimations[a];
                    anmFileAnimation += $"\n        [Null] Id：{nullAnimation.NullId} | Visible：{nullAnimation.Visible}";
                    for (int b = 0; b < nullAnimation.Frames.Count; b++) {
                        AnmFrame frame = nullAnimation.Frames[b];
                        anmFileAnimation += $"\n        Frame{a + 1}. Position：({frame.XPosition},{frame.YPosition}) | Scale：({frame.XScale},{frame.YScale}) | Delay：{frame.Delay} | Visible：{frame.Visible} | Tint：({frame.RedTint},{frame.GreenTint},{frame.BlueTint},{frame.AlphaTint}) | Offset：({frame.RedOffset},{frame.GreenOffset},{frame.BlueOffset}) | Rotation：{frame.Rotation} | Interpolated：{frame.Interpolated}";
                    }
                }
                anmFileAnimation += "\n    |Triggers|";
                List<AnmTriggerFrame> triggerFrames = animation.Triggers;
                for (int a = 0; a < triggerFrames.Count; a++) {
                    AnmTriggerFrame triggerFrame = triggerFrames[a];
                    anmFileAnimation += $"\n        [Trigger] Id：{triggerFrame.EventId} | At frame：{triggerFrame.AtFrame}";
                }
            }
            fileAnimation = anmFileAnimation;
        }

        private void DebugAnimFileUseChinese(AnmFile file, out string fileInfo, out string fileContent, out string fileAnimation) {
            anmFileInfo = "";
            anmFileInfo += "--- 基础信息 ---";
            AnmInfo info = file.Info;
            anmFileInfo += $"\n作者：{info.CreatedBy} \n创建日期：{info.CreatedOn} \n版本：{info.Version} \n帧率：{info.Fps}";
            fileInfo = anmFileInfo;
            //
            anmFileContent = "";
            anmFileContent += "--- 资源信息 ---";
            for (int i = 0; i < file.Content.Spritesheets.Count; i++) {
                AnmSpritesheet sheet = file.Content.Spritesheets[i];
                anmFileContent += $"\n[图集] {i + 1}. 路径：{sheet.Path} | 图集序数：{sheet.Id}";
            }
            for (int i = 0; i < file.Content.Layers.Count; i++) {
                AnmLayer layer = file.Content.Layers[i];
                anmFileContent += $"\n[图集图层] {i + 1}. 图层名称：{layer.Name} | 图层序数：{layer.Id} | 图集序数：{layer.SpritesheetId}";
            }
            for (int i = 0; i < file.Content.Nulls.Count; i++) {
                AnmNull nullObject = file.Content.Nulls[i];
                anmFileContent += $"\n[非图集图层] {i + 1}. 图层名称：{nullObject.Name} | 图层序数：{nullObject.Id} | 是否显示矩形：{nullObject.ShowRect}";
            }
            for (int i = 0; i < file.Content.Events.Count; i++) {
                AnmEvent eventObject = file.Content.Events[i];
                anmFileContent += $"\n[事件] {i + 1}. 事件名称：{eventObject.Name} | 事件序数：{eventObject.Id}";
            }
            fileContent = anmFileContent;
            //
            anmFileAnimation = "";
            anmFileAnimation += "--- 动画信息 ---";
            AnmAnimations animations = file.Animations;
            anmFileAnimation += $"\n默认动画：{animations.DefaultAnimation}";
            for (int i = 0; i < animations.AnimationList.Count; i++) {
                AnmAnimation animation = animations.AnimationList[i];
                anmFileAnimation += $"\n    --- 动画片段{i + 1}. 动画名称：{animation.Name} | 动画帧数：{animation.FrameNum} | 是否循环：{animation.Loop} ---";
                anmFileAnimation += "\n    |根动画|";
                AnmRootAnimation rootAnimation = animation.RootAnimation;
                for (int a = 0; a < rootAnimation.Frames.Count; a++) {
                    AnmFrame frame = rootAnimation.Frames[a];
                    anmFileAnimation += $"\n    帧{a + 1}. 位置：({frame.XPosition},{frame.YPosition}) | 缩放：({frame.XScale},{frame.YScale}) | 持续时间：{frame.Delay} | 可见：{frame.Visible} | 正片叠底：({frame.RedTint},{frame.GreenTint},{frame.BlueTint},{frame.AlphaTint}) | 叠加色：({frame.RedOffset},{frame.GreenOffset},{frame.BlueOffset}) | 旋转：{frame.Rotation} | 是否补间：{frame.Interpolated}";
                }
                anmFileAnimation += "\n    |图集图层动画|";
                List<AnmLayerAnimation> layerAnimations = animation.LayerAnimations;
                for (int a = 0; a < layerAnimations.Count; a++) {
                    AnmLayerAnimation layerAnimation = layerAnimations[a];
                    anmFileAnimation += $"\n        [图集图层] 排序：{layerAnimation.LayerId} | 是否可见：{layerAnimation.Visible}";
                    for (int b = 0; b < layerAnimation.Frames.Count; b++) {
                        AnmLayerFrame frame = layerAnimation.Frames[b];
                        anmFileAnimation += $"\n        帧{a + 1}. 位置：({frame.XPosition},{frame.YPosition}) | 锚点：({frame.XPivot},{frame.YPivot}) | 裁切坐标：({frame.XCrop},{frame.YCrop}) | 裁切长宽：({frame.Width},{frame.Height}) | 缩放：({frame.XScale},{frame.YScale}) | 持续时间：{frame.Delay} | 可见：{frame.Visible} | 正片叠底：({frame.RedTint},{frame.GreenTint},{frame.BlueTint},{frame.AlphaTint}) | 叠加色：({frame.RedOffset},{frame.GreenOffset},{frame.BlueOffset}) | 旋转：{frame.Rotation} | 是否补间：{frame.Interpolated}";
                    }
                }
                anmFileAnimation += "\n    |非图集图层动画|";
                List<AnmNullAnimation> nullAnimations = animation.NullAnimations;
                for (int a = 0; a < nullAnimations.Count; a++) {
                    AnmNullAnimation nullAnimation = nullAnimations[a];
                    anmFileAnimation += $"\n        [非图集图层] 排序：{nullAnimation.NullId} | 是否可见：{nullAnimation.Visible}";
                    for (int b = 0; b < nullAnimation.Frames.Count; b++) {
                        AnmFrame frame = nullAnimation.Frames[b];
                        anmFileAnimation += $"\n        帧{a + 1}. 位置：({frame.XPosition},{frame.YPosition}) | 缩放：({frame.XScale},{frame.YScale}) | 持续时间：{frame.Delay} | 可见：{frame.Visible} | 正片叠底：({frame.RedTint},{frame.GreenTint},{frame.BlueTint},{frame.AlphaTint}) | 叠加色：({frame.RedOffset},{frame.GreenOffset},{frame.BlueOffset}) | 旋转：{frame.Rotation} | 是否补间：{frame.Interpolated}";
                    }
                }
                anmFileAnimation += "\n    |事件触发时间轴|";
                List<AnmTriggerFrame> triggerFrames = animation.Triggers;
                for (int a = 0; a < triggerFrames.Count; a++) {
                    AnmTriggerFrame triggerFrame = triggerFrames[a];
                    anmFileAnimation += $"\n        [事件触发] 事件序数：{triggerFrame.EventId} | 帧数序数：{triggerFrame.AtFrame}";
                }
            }
            fileAnimation = anmFileAnimation;
        }
    }
}