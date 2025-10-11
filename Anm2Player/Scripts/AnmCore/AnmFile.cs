using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections;
using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【Anm2工程文件】Anm2 project file</summary>
    [XmlRoot("AnimatedActor")]
    public class AnmFile
    {
        /// <summary>【工程信息】Informations</summary>
        [XmlElement("Info")] public AnmInfo Info { get; set; }
        /// <summary>【资源使用信息】Content resources</summary>
        [XmlElement("Content")] public AnmContent Content { get; set; }
        /// <summary>【动画信息】Animations</summary>
        [XmlElement("Animations")] public AnmAnimations Animations { get; set; }

        public AnmFile() {
            Info = new AnmInfo();
            Content = new AnmContent();
            Animations = new AnmAnimations();
        }

        #region 根图层帧
        /// <summary>【获取根图层帧】Get root frame</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【根图层帧】Root frame</returns>
        public AnmFrame GetRootFrame(int animationIndex, int frameIndex) {
            if (animationIndex >= Animations.AnimationList.Count) return null;
            AnmAnimation animation = Animations.AnimationList[animationIndex];
            if (animation.RootAnimation == null) return null;
            if (animation.RootAnimation.Frames.Count <= 0) return null;
            if (frameIndex >= animation.RootAnimation.Frames.Count) return null;
            return animation.RootAnimation.GetFrameByFrameIndex(frameIndex);
        }
        /// <summary>【获取根图层帧】Get root frame</summary>
        /// <param name="animationName">【动画名称】Animation name</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【根图层帧】Root frame</returns>
        public AnmFrame GetRootFrame(string animationName, int frameIndex) {
            return GetRootFrame(Animations.GetAnimationIndexByName(animationName), frameIndex);
        }
        #endregion

        #region 图集图层帧
        /// <summary>【获取图集图层帧】Get layer frame</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="layerIndex">【图集图层序数】Layer index</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【图集图层帧】Sheet layer frame</returns>
        public AnmLayerFrame GetLayerFrame(int animationIndex, int layerIndex, int frameIndex) {
            if (animationIndex >= Animations.AnimationList.Count) return null;
            AnmAnimation animation = Animations.AnimationList[animationIndex];
            if (animation.LayerAnimations.Count <= 0) return null;
            foreach (AnmLayerAnimation layerAnimation in animation.LayerAnimations) {
                if (layerAnimation.LayerId == layerIndex) {
                    if (frameIndex >= layerAnimation.GetFrameCount()) return null;
                    return layerAnimation.GetFrameByFrameIndex(frameIndex);
                }
            }
            return null;
        }
        /// <summary>【获取图集图层帧】Get layer frame</summary>
        /// <param name="animationName">【动画名称】Animation name</param>
        /// <param name="layerName">【图集图层名称】Layer name</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【图集图层帧】Sheet layer frame</returns>
        public AnmLayerFrame GetLayerFrame(string animationName, string layerName, int frameIndex) {
            return GetLayerFrame(Animations.GetAnimationIndexByName(animationName), Content.GetLayerIndexByName(layerName), frameIndex);
        }
        /// <summary>【获取图集图层帧】Get layer frame</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="layerName">【图集图层名称】Layer name</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【图集图层帧】Sheet layer frame</returns>
        public AnmLayerFrame GetLayerFrame(int animationIndex, string layerName, int frameIndex) {
            return GetLayerFrame(animationIndex, Content.GetLayerIndexByName(layerName), frameIndex);
        }
        /// <summary>【获取图集图层帧】Get layer frame</summary>
        /// <param name="animationName">【动画名称】Animation name</param>
        /// <param name="layerIndex">【图集图层序数】Layer index</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【图集图层帧】Sheet layer frame</returns>
        public AnmLayerFrame GetLayerFrame(string animationName, int layerIndex, int frameIndex) {
            return GetLayerFrame(Animations.GetAnimationIndexByName(animationName), layerIndex, frameIndex);
        }
        #endregion

        #region 非图集图层帧
        /// <summary>【获取非图集图层帧】Get null frame</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="nullIndex">【非图集图层序数】Null index</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【非图集图层帧】Null layer frame</returns>
        public AnmFrame GetNullFrame(int animationIndex, int nullIndex, int frameIndex) {
            if (animationIndex >= Animations.AnimationList.Count) return null;
            AnmAnimation animation = Animations.AnimationList[animationIndex];
            if (animation.NullAnimations.Count <= 0) return null;
            foreach (AnmNullAnimation nullAnimation in animation.NullAnimations) {
                if (nullAnimation.NullId == nullIndex) {
                    if (frameIndex >= nullAnimation.GetFrameCount()) return null;
                    return nullAnimation.GetFrameByFrameIndex(frameIndex);
                }
            }
            return null;
        }
        /// <summary>【获取非图集图层帧】Get null frame</summary>
        /// <param name="animationName">【动画名称】</param>
        /// <param name="nullName">【非图集图层名称】</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【非图集图层帧】Null layer frame</returns>
        public AnmFrame GetNullFrame(string animationName, string nullName, int frameIndex) {
            return GetNullFrame(Animations.GetAnimationIndexByName(animationName), Content.GetNullIndexByName(nullName), frameIndex);
        }
        /// <summary>【获取非图集图层帧】Get null frame</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="nullName">【非图集图层名称】</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【非图集图层帧】Null layer frame</returns>
        public AnmFrame GetNullFrame(int animationIndex, string nullName, int frameIndex) {
            return GetNullFrame(animationIndex, Content.GetNullIndexByName(nullName), frameIndex);
        }
        /// <summary>【获取非图集图层帧】Get null frame</summary>
        /// <param name="animationName">【动画名称】</param>
        /// <param name="nullIndex">【非图集图层序数】Null index</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【非图集图层帧】Null layer frame</returns>
        public AnmFrame GetNullFrame(string animationName, int nullIndex, int frameIndex) {
            return GetNullFrame(Animations.GetAnimationIndexByName(animationName), nullIndex, frameIndex);
        }
        #endregion

        #region 事件帧
        /// <summary>【检查帧是否触发事件】Check frame triggered</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="eventIndex">【事件序数，-1时为任意】Event index, -1 is any</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【是否触发事件】Is triggered</returns>
        public bool CheckFrameTriggered(int animationIndex, int eventIndex, int frameIndex) {
            if (animationIndex >= Animations.AnimationList.Count) return false;
            AnmAnimation animation = Animations.AnimationList[animationIndex];
            if (animation.Triggers.Count <= 0) return false;
            foreach (AnmTriggerFrame trigger in animation.Triggers) {
                if (trigger.EventId == eventIndex && trigger.AtFrame == frameIndex) {
                    return true;
                }
                if (eventIndex == -1 && trigger.AtFrame == frameIndex) {
                    return true;
                }
            }
            return false;
        }
        /// <summary>【检查帧是否触发事件】Check frame triggered</summary>
        /// <param name="animationName">【动画名称】</param>
        /// <param name="eventName">【事件名称】</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【是否触发事件】Is triggered</returns>
        public bool CheckFrameTriggered(string animationName, string eventName, int frameIndex) {
            return CheckFrameTriggered(Animations.GetAnimationIndexByName(animationName), Content.GetEventIndexByName(eventName), frameIndex);
        }
        /// <summary>【检查帧是否触发事件】Check frame triggered</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="eventName">【事件名称】</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【是否触发事件】Is triggered</returns>
        public bool CheckFrameTriggered(int animationIndex, string eventName, int frameIndex) {
            return CheckFrameTriggered(animationIndex, Content.GetEventIndexByName(eventName), frameIndex);
        }
        /// <summary>【检查帧是否触发事件】Check frame triggered</summary>
        /// <param name="animationName">【动画名称】</param>
        /// <param name="eventIndex">【事件序数，-1时为任意】Event index, -1 is any</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <returns>【是否触发事件】Is triggered</returns>
        public bool CheckFrameTriggered(string animationName, int eventIndex, int frameIndex) {
            return CheckFrameTriggered(Animations.GetAnimationIndexByName(animationName), eventIndex, frameIndex);
        }
        /// <summary>【检查帧是否触发事件】Check frame triggered</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="eventIndex">【事件序数，-1时为任意】Event index, -1 is any</param>
        /// <param name="frameIndex">【帧序数】Frame index</param>
        /// <param name="eventName">【事件名称】</param>
        /// <returns>【是否触发事件】Is triggered</returns>
        public bool CheckFrameTriggered(int animationIndex, int eventIndex, int frameIndex, out string eventName) {
            if (animationIndex >= Animations.AnimationList.Count) {
                eventName = string.Empty; return false;
            }
            AnmAnimation animation = Animations.AnimationList[animationIndex];
            if (animation.Triggers.Count <= 0) {
                eventName = string.Empty; return false;
            }
            foreach (AnmTriggerFrame trigger in animation.Triggers) {
                if (trigger.EventId == eventIndex && trigger.AtFrame == frameIndex) {
                    eventName = Content.Events[trigger.EventId].Name; return true;
                }
                if (eventIndex == -1 && trigger.AtFrame == frameIndex) {
                    eventName = Content.Events[trigger.EventId].Name; return true;
                }
            }
            eventName = string.Empty; return false;
        }
        /// <summary>【在指定动画中查找某事件触发的所有帧数】Find the frame index when event triggered in specified animation</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="eventIndex">【事件序数】Event index</param>
        /// <returns>【触发帧数列表】Triggered frames list</returns>
        public List<int> GetEventTriggeredFrames(int animationIndex, int eventIndex) {
            if (animationIndex >= Animations.AnimationList.Count) return null;
            AnmAnimation animation = Animations.AnimationList[animationIndex];
            if (animation.Triggers.Count <= 0) return null;
            List<int> frames = new();
            for (int i = 0; i < animation.Triggers.Count; i++) {
                if (animation.Triggers[i].EventId == eventIndex) {
                    frames.Add(animation.Triggers[i].AtFrame);
                }
            }
            return frames;
        }
        /// <summary>【在指定动画中查找某事件触发的所有帧数】Find the frame index when event triggered in specified animation</summary>
        /// <param name="animationName">【动画名称】</param>
        /// <param name="eventIndex">【事件序数】Event index</param>
        /// <returns>【触发帧数列表】Triggered frames list</returns>
        public List<int> GetEventTriggeredFrames(string animationName, int eventIndex) {
            return GetEventTriggeredFrames(Animations.GetAnimationIndexByName(animationName), eventIndex);
        }
        /// <summary>【在指定动画中查找某事件触发的所有帧数】Find the frame index when event triggered in specified animation</summary>
        /// <param name="animationIndex">【动画序数】Animation index</param>
        /// <param name="eventName">【事件名称】</param>
        /// <returns>【触发帧数列表】Triggered frames list</returns>
        public List<int> GetEventTriggeredFrames(int animationIndex, string eventName) {
            return GetEventTriggeredFrames(animationIndex, Content.GetEventIndexByName(eventName));
        }
        /// <summary>【在指定动画中查找某事件触发的所有帧数】Find the frame index when event triggered in specified animation</summary>
        /// <param name="animationName">【动画名称】</param>
        /// <param name="eventName">【事件名称】</param>
        /// <returns>【触发帧数列表】Triggered frames list</returns>
        public List<int> GetEventTriggeredFrames(string animationName, string eventName) {
            return GetEventTriggeredFrames(Animations.GetAnimationIndexByName(animationName), Content.GetEventIndexByName(eventName));
        }
        #endregion

        /// <summary>【获取AnmFile的整体复制，而非引用】</summary>
        public AnmFile Copy() {
            return new AnmFile() {
                Info = Info.Copy(),
                Content = Content.Copy(),
                Animations = Animations.Copy()
            };
        }
    }

    #region Info 工程信息
    /// <summary>【Anm2工程信息】Anm2 file informations</summary>
    public class AnmInfo
    {
        /// <summary>【作者】Author</summary>
        [XmlAttribute("CreatedBy")] public string CreatedBy { get; set; }
        /// <summary>【创建日期】Date</summary>
        [XmlAttribute("CreatedOn")] public string CreatedOn { get; set; }
        /// <summary>【版本】Version</summary>
        [XmlAttribute("Version")] public int Version { get; set; }
        /// <summary>【帧率】Frames per second</summary>
        [XmlAttribute("Fps")] public int Fps { get; set; }

        public AnmInfo() {
            CreatedBy = "";
            CreatedOn = "";
            Version = 0;
            Fps = 30;
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmInfo Copy() {
            return new AnmInfo() {
                CreatedBy = CreatedBy,
                CreatedOn = CreatedOn,
                Version = Version,
                Fps = Fps
            };
        }
    }
    #endregion

    #region Content 资源使用信息
    /// <summary>【Anm2资源使用信息】Anm2 file resources</summary>
    public class AnmContent
    {
        /// <summary>【图集列表】Sprite sheets list</summary>
        [XmlArray("Spritesheets"), XmlArrayItem("Spritesheet")] public List<AnmSpritesheet> Spritesheets { get; set; }
        /// <summary>【图集图层列表】Sprite layers list</summary>
        [XmlArray("Layers"), XmlArrayItem("Layer")] public List<AnmLayer> Layers { get; set; }
        /// <summary>【非图集图层列表】Null layers list</summary>
        [XmlArray("Nulls"), XmlArrayItem("Null")] public List<AnmNull> Nulls { get; set; }
        /// <summary>【事件触发列表】Events list</summary>
        [XmlArray("Events"), XmlArrayItem("Event")] public List<AnmEvent> Events { get; set; }

        public AnmContent() {
            Spritesheets = new List<AnmSpritesheet>();
            Layers = new List<AnmLayer>();
            Nulls = new List<AnmNull>();
            Events = new List<AnmEvent>();
        }

        /// <summary>【计算出所有图集图层的排序信息】Bake all layer's order infos</summary>
        public AnmContent BakeLayerOrders() {
            for (int l = 0; l < Layers.Count; l++) {
                Layers[l].ImageOrder = l;
            }
            for (int n = 0; n < Nulls.Count; n++) {
                Nulls[n].ImageOrder = n;
            }
            return this;
        }

        /// <summary>【通过图层序数获取图集序数】Get sprite sheet ID by layer ID</summary>
        /// <param name="layerId">【图层序数】layer ID</param>
        /// <returns>【图集序数】Sprite sheet ID</returns>
        public int GetSpriteSheetIndexByLayerId(int layerId) {
            foreach (AnmLayer layer in Layers) {
                if (layer.Id == layerId)
                    return layer.SpritesheetId;
            }
            return 0;
        }
        /// <summary>【通过图集图层名称获取图层序数】Get layer ID by layer name</summary>
        /// <param name="layerName">【图集图层名称】Layer name</param>
        /// <returns>【图层序数】Layer ID</returns>
        public int GetLayerIndexByName(string layerName) {
            for (int i = 0; i < Layers.Count; i++) {
                if (Layers[i].Name == layerName)
                    return Layers[i].Id;
            }
            return 0;
        }
        /// <summary>【通过图层序数获取图集图层名称】Get layer name by layer ID</summary>
        /// <param name="layerIndex">【图层序数】Layer ID</param>
        /// <returns>【图集图层名称】Layer name</returns>
        public string GetLayerNameByIndex(int layerIndex) {
            for (int i = 0; i < Layers.Count; i++) {
                if (Layers[i].Id == layerIndex)
                    return Layers[i].Name;
            }
            return "";
        }
        /// <summary>【通过非图集图层名称获取图层序数】Get null index by null name</summary>
        /// <param name="nullName">【非图集图层名称】Null name</param>
        /// <returns>【图层序数】Null ID</returns>
        public int GetNullIndexByName(string nullName) {
            for (int i = 0; i < Nulls.Count; i++) {
                if (Nulls[i].Name == nullName)
                    return Nulls[i].Id;
            }
            return 0;
        }
        /// <summary>【通过非图集图层序数获取非图层名称】Get null name by null index</summary>
        /// <param name="nullIndex">【非图集图层序数】Null index</param>
        /// <returns>【图层名称】Null name</returns>
        public string GetNullNameByIndex(int nullIndex) {
            for (int i = 0; i < Nulls.Count; i++) {
                if (Nulls[i].Id == nullIndex)
                    return Nulls[i].Name;
            }
            return "";
        }
        /// <summary>【通过事件名称获取事件序数】Get event ID by event name</summary>
        /// <param name="eventName">【事件名称】Event name</param>
        /// <returns>【事件序数】Event ID</returns>
        public int GetEventIndexByName(string eventName) {
            for (int i = 0; i < Events.Count; i++) {
                if (Events[i].Name == eventName)
                    return Events[i].Id;
            }
            return 0;
        }
        /// <summary>【通过事件序数获取事件名称】Get event name by event ID</summary>
        /// <param name="eventIndex">【事件序数】Event ID</param>
        /// <returns>【事件名称】Event name</returns>
        public string GetEventNameByIndex(int eventIndex) {
            for (int i = 0; i < Events.Count; i++) {
                if (Events[i].Id == eventIndex)
                    return Events[i].Name;
            }
            return "";
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmContent Copy() {
            AnmContent outContent = new() {
                Spritesheets = new List<AnmSpritesheet>(),
                Layers = new List<AnmLayer>(),
                Nulls = new List<AnmNull>(),
                Events = new List<AnmEvent>()
            };
            foreach (AnmSpritesheet ss in Spritesheets) {
                outContent.Spritesheets.Add(ss.Copy());
            }
            foreach (AnmLayer layer in Layers) {
                outContent.Layers.Add(layer.Copy());
            }
            foreach (AnmNull nulls in Nulls) {
                outContent.Nulls.Add(nulls.Copy());
            }
            foreach (AnmEvent evt in Events) {
                outContent.Events.Add(evt.Copy());
            }
            outContent.BakeLayerOrders(); // 计算出所有图集图层的排序信息 | Bake all layer's order infos
            return outContent;
        }
    }
    /// <summary>【Anm2图集】Anm2 Sprite sheet</summary>
    public class AnmSpritesheet
    {
        /// <summary>【图集路径】Path</summary>
        [XmlAttribute("Path")] public string Path { get; set; }
        /// <summary>【图集序数】ID</summary>
        [XmlAttribute("Id")] public int Id { get; set; }

        public AnmSpritesheet() {
            Path = "";
            Id = 0;
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmSpritesheet Copy() {
            return new AnmSpritesheet() {
                Path = Path,
                Id = Id
            };
        }
    }
    /// <summary>【Anm2图集图层】Anm2 Layer</summary>
    public class AnmLayer
    {
        /// <summary>【图层名称】Name</summary>
        [XmlAttribute("Name")] public string Name { get; set; }
        /// <summary>【图层序数】ID</summary>
        [XmlAttribute("Id")] public int Id { get; set; }
        /// <summary>【图集序数】Sprite sheet ID</summary>
        [XmlAttribute("SpritesheetId")] public int SpritesheetId { get; set; }

        public int ImageOrder { get => imageOrder; set => imageOrder = value; }
        private int imageOrder = 0;

        public AnmLayer() {
            Name = "";
            Id = 0;
            SpritesheetId = 0;
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmLayer Copy() {
            return new AnmLayer() {
                Name = Name,
                Id = Id,
                SpritesheetId = SpritesheetId,
                ImageOrder = ImageOrder
            };
        }
    }
    /// <summary>【Anm2非图集图层】Anm2 Null</summary>
    public class AnmNull
    {
        /// <summary>【图层名称】Name</summary>
        [XmlAttribute("Name")] public string Name { get; set; }
        /// <summary>【图层序数】ID</summary>
        [XmlAttribute("Id")] public int Id { get; set; }
        /// <summary>【是否显示矩形】Show rect</summary>
        [XmlAttribute("ShowRect")] public bool ShowRect { get; set; }

        public int ImageOrder { get => imageOrder; set => imageOrder = value; }
        private int imageOrder = 0;

        public AnmNull() {
            Name = "";
            Id = 0;
            ShowRect = false;
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmNull Copy() {
            return new AnmNull() {
                Name = Name,
                Id = Id,
                ShowRect = ShowRect,
                ImageOrder = ImageOrder
            };
        }
    }
    /// <summary>【Anm2事件】Anm2 event</summary>
    public class AnmEvent
    {
        /// <summary>【事件名称】Name</summary>
        [XmlAttribute("Name")] public string Name { get; set; }
        /// <summary>【事件序数】ID</summary>
        [XmlAttribute("Id")] public int Id { get; set; }

        public AnmEvent() {
            Name = "";
            Id = 0;
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmEvent Copy() {
            return new AnmEvent() {
                Name = Name,
                Id = Id
            };
        }
    }
    #endregion

    #region Animation 动画片段
    /// <summary>【Anm2动画片段列表】Anm2 animation clips list</summary>
    public class AnmAnimations
    {
        /// <summary>【默认动画片段】Default animation clip</summary>
        [XmlAttribute("DefaultAnimation")] public string DefaultAnimation { get; set; }
        /// <summary>【动画列表】Animations list</summary>
        [XmlElement("Animation")] public List<AnmAnimation> AnimationList { get; set; }

        public AnmAnimations() {
            DefaultAnimation = "";
            AnimationList = new List<AnmAnimation>();
        }

        /// <summary>【获取默认动画片段名称】Get default animation name</summary>
        public string GetDefaultAnimationName() {
            return DefaultAnimation;
        }

        /// <summary>【获取默认动画片段索引】Get default animation ID</summary>
        public int GetDefaultAnimationIndex() {
            for (int i = 0; i < AnimationList.Count; i++) {
                if (AnimationList[i].Name == DefaultAnimation) return i;
            }
            return 0;
        }

        /// <summary>【获取默认动画片段】Get default animation clip</summary>
        public AnmAnimation GetDefaultAnimation() {
            return AnimationList[GetDefaultAnimationIndex()];
        }

        /// <summary>【通过动画名称获取动画片段序数】Get animation ID by animation name</summary>
        /// <param name="animationName">【动画名称】Animation name</param>
        /// <returns>【动画片段序数】Animation ID</returns>
        public int GetAnimationIndexByName(string animationName) {
            for (int i = 0; i < AnimationList.Count; i++) {
                if (AnimationList[i].Name == animationName)
                    return i;
            }
            return 0;
        }

        /// <summary>【通过动画片段序数获取动画片段】Get animation name by animation ID</summary>
        /// <param name="animationIndex">【动画片段序数】Animation ID</param>
        /// <returns>【动画名称】Animation name</returns>
        public string GetAnimationNameByIndex(int animationIndex) {
            return AnimationList[animationIndex].Name;
        }

        /// <summary>【通过动画名称获取动画片段】Get animation clip by animation name</summary>
        /// <param name="animationName">【动画名称】Animation name</param>
        /// <returns>【动画片段】Animation clip</returns>
        public AnmAnimation GetAnimationByName(string animationName) {
            for (int i = 0; i < AnimationList.Count; i++) {
                if (AnimationList[i].Name == animationName) {
                    return AnimationList[i];
                }
            }
            return null;
        }

        /// <summary>【通过动画序数获取动画片段】Get animation clip by animation ID</summary>
        /// <param name="animationIndex">【动画片段序数】Animation ID</param>
        /// <returns>【动画片段】Animation clip</returns>
        public AnmAnimation GetAnimationByIndex(int animationIndex) {
            if (animationIndex >= AnimationList.Count) {
                return null;
            }
            return AnimationList[animationIndex];
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmAnimations Copy() {
            AnmAnimations outAnimationList = new() {
                DefaultAnimation = DefaultAnimation,
                AnimationList = new List<AnmAnimation>()
            };
            foreach (var animation in AnimationList) {
                outAnimationList.AnimationList.Add(animation.Copy());
            }
            return outAnimationList;
        }
    }
    /// <summary>【Anm2动画片段】Anm2 Animation clip</summary>
    public class AnmAnimation
    {
        /// <summary>【动画名称】Name</summary>
        [XmlAttribute("Name")] public string Name { get; set; }
        /// <summary>【动画总帧数】Total frame count</summary>
        [XmlAttribute("FrameNum")] public int FrameNum { get; set; }
        /// <summary>【是否循环】Is loop</summary>
        [XmlAttribute("Loop")] public bool Loop { get; set; }
        /// <summary>【根动画】Root animation</summary>
        [XmlElement("RootAnimation")] public AnmRootAnimation RootAnimation { get; set; }
        /// <summary>【图集图层动画列表】Layer animations list</summary>
        [XmlArray("LayerAnimations"), XmlArrayItem("LayerAnimation")] public List<AnmLayerAnimation> LayerAnimations { get; set; }
        /// <summary>【非图集图层动画列表】Null animations list</summary>
        [XmlArray("NullAnimations"), XmlArrayItem("NullAnimation")] public List<AnmNullAnimation> NullAnimations { get; set; }
        /// <summary>【事件触发列表】Trigger frames list</summary>
        [XmlArray("Triggers"), XmlArrayItem("Trigger")] public List<AnmTriggerFrame> Triggers { get; set; }

        public AnmAnimation() {
            Name = "";
            FrameNum = 1;
            Loop = true;
            RootAnimation = new AnmRootAnimation();
            LayerAnimations = new List<AnmLayerAnimation>();
            NullAnimations = new List<AnmNullAnimation>();
            Triggers = new List<AnmTriggerFrame>();
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmAnimation Copy() {
            AnmAnimation outAnimation = new() {
                Name = Name,
                FrameNum = FrameNum,
                Loop = Loop,
                RootAnimation = RootAnimation.Copy(),
                LayerAnimations = new List<AnmLayerAnimation>(),
                NullAnimations = new List<AnmNullAnimation>(),
                Triggers = new List<AnmTriggerFrame>()
            };
            foreach (var layerAnimation in LayerAnimations) {
                outAnimation.LayerAnimations.Add(layerAnimation.Copy());
            }
            foreach (var nullAnimation in NullAnimations) {
                outAnimation.NullAnimations.Add(nullAnimation.Copy());
            }
            foreach (var trigger in Triggers) {
                outAnimation.Triggers.Add(trigger.Copy());
            }
            return outAnimation;
        }
    }
    /// <summary>【Anm2动画片段-根动画】Anm2 root animation</summary>
    public class AnmRootAnimation
    {
        /// <summary>【关键帧列表】Frames</summary>
        [XmlElement("Frame")] public List<AnmFrame> Frames { get; set; }
        /// <summary>【所有帧】All frames</summary>
        public List<AnmFrame> AllFrames {
            get {
                if (allFrames.Count <= 0) {
                    allFrames.Clear();
                    BakeAllFrames();
                }
                return allFrames;
            }
        }
        private readonly List<AnmFrame> allFrames = new(); // 所有帧 | All frames

        public AnmRootAnimation() {
            Frames = new List<AnmFrame>();
        }

        /// <summary>【获取指定帧索引的帧，包括补间生成的帧】Get frame by frame ID, include interpolated frames</summary>
        /// <param name="frameIndex">【帧索引】Frame ID</param>
        /// <returns>【图集图层帧信息】Layer frame</returns>
        public AnmFrame GetFrameByFrameIndex(int frameIndex) {
            if (AllFrames.Count > frameIndex) {
                if (frameIndex < 0) {
                    return AllFrames[0];
                }
                return AllFrames[frameIndex];
            }
            // 如果索引超出范围，则返回最后一帧
            else {
                if (AllFrames.Count > 0) {
                    return AllFrames[AllFrames.Count - 1];
                }
            }
            return null;
        }

        /// <summary>【获取帧数量】Get frame count</summary>
        public int GetFrameCount() {
            return AllFrames.Count;
        }

        /// <summary>【计算出所有帧信息】Bake all frames</summary>
        private void BakeAllFrames() {
            allFrames.Clear();
            // 遍历所有关键帧
            for (int i = 0; i < Frames.Count; i++) {
                // 如果帧间隔等于1，则直接作为普通帧
                if (Frames[i].Delay <= 1) {
                    allFrames.Add(Frames[i]);
                }
                // 如果帧间隔大于1，则需要计算补间帧
                else {
                    // 判断i+1帧是否存在
                    if (i + 1 < Frames.Count) {
                        for (int j = 0; j < Frames[i].Delay; j++) {
                            // 计算补间帧
                            AnmFrame interpFrame = Frames[i].Lerp(Frames[i + 1], (float)j / Frames[i].Delay);
                            allFrames.Add(interpFrame);
                        }
                    }
                    // 如果不存在i+1帧，就把i帧填充Delay-1次
                    else {
                        for (int j = 0; j < Frames[i].Delay; j++) {
                            allFrames.Add(Frames[i]);
                        }
                    }
                }
            }
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmRootAnimation Copy() {
            AnmRootAnimation outRootAnimation = new() {
                Frames = new List<AnmFrame>(),
            };
            foreach (AnmFrame frame in Frames) {
                outRootAnimation.Frames.Add(frame.Copy());
            }
            if (outRootAnimation.allFrames.Count <= 0) {
                outRootAnimation.allFrames.Clear();
                outRootAnimation.BakeAllFrames(); // 缓存所有帧信息 | Cache all frame info
            }
            return outRootAnimation;
        }
    }
    /// <summary>【Anm2动画片段-图集图层动画】Anm2 layer animation</summary>
    public class AnmLayerAnimation
    {
        /// <summary>【图层序数】Layer ID</summary>
        [XmlAttribute("LayerId")] public int LayerId { get; set; }
        /// <summary>【是否可见】Is visible</summary>
        [XmlAttribute("Visible")] public bool Visible { get; set; }
        /// <summary>【关键帧列表】Frames</summary>
        [XmlElement("Frame")] public List<AnmLayerFrame> Frames { get; set; }
        /// <summary>【所有帧】All frames</summary>
        public List<AnmLayerFrame> AllFrames {
            get {
                if (allFrames.Count <= 0) {
                    allFrames.Clear();
                    BakeAllFrames();
                }
                return allFrames;
            }
        }
        private List<AnmLayerFrame> allFrames = new(); // 所有帧 | All frames

        public AnmLayerAnimation() {
            LayerId = 0;
            Visible = true;
            Frames = new List<AnmLayerFrame>();
        }

        /// <summary>【获取指定帧索引的帧，包括补间生成的帧】Get frame by frame ID, include interpolated frames</summary>
        /// <param name="frameIndex">【帧索引】Frame ID</param>
        /// <returns>【图集图层帧信息】Layer frame</returns>
        public AnmLayerFrame GetFrameByFrameIndex(int frameIndex) {
            if (AllFrames.Count > frameIndex) {
                if (frameIndex < 0) {
                    return AllFrames[0];
                }
                return AllFrames[frameIndex];
            }
            // 如果索引超出范围，则返回最后一帧
            else {
                if (AllFrames.Count > 0) {
                    return AllFrames[AllFrames.Count - 1];
                }
            }
            return null;
        }

        /// <summary>【获取帧数量】Get frame count</summary>
        public int GetFrameCount() {
            return AllFrames.Count;
        }

        /// <summary>【计算出所有帧信息】Bake all frames</summary>
        private void BakeAllFrames() {
            allFrames.Clear();
            // 遍历所有关键帧
            for (int i = 0; i < Frames.Count; i++) {
                // 如果帧间隔等于1，则直接作为普通帧
                if (Frames[i].Delay <= 1) {
                    allFrames.Add(Frames[i]);
                }
                // 如果帧间隔大于1，则需要计算补间帧
                else {
                    // 判断i+1帧(下一帧)是否存在
                    if (i + 1 < Frames.Count) {
                        for (int j = 0; j < Frames[i].Delay; j++) {
                            // 计算补间帧
                            AnmLayerFrame interpFrame = Frames[i];
                            if (Frames[i].Interpolated) {
                                interpFrame = Frames[i].Lerp(Frames[i + 1], (float)j / Frames[i].Delay);
                            }
                            allFrames.Add(interpFrame);
                        }
                    }
                    // 如果不存在i+1帧(下一帧)，就把i帧填充Delay-1次
                    else {
                        for (int j = 0; j < Frames[i].Delay; j++) {
                            allFrames.Add(Frames[i]);
                        }
                    }
                }
            }
        }
        
        /// <summary>【复制类】Copy type</summary>
        public AnmLayerAnimation Copy() {
            AnmLayerAnimation outLayerAnimation = new() {
                LayerId = LayerId,
                Visible = Visible,
                Frames = new List<AnmLayerFrame>()
            };
            foreach (AnmLayerFrame frame in Frames) {
                outLayerAnimation.Frames.Add(frame.Copy());
            }
            if (outLayerAnimation.allFrames.Count <= 0) {
                outLayerAnimation.allFrames.Clear();
                outLayerAnimation.BakeAllFrames(); // 缓存所有帧信息 | Cache all frame info
            }
            return outLayerAnimation;
        }
    }
    /// <summary>【Anm2动画片段-非图集图层动画】Anm2 null animation</summary>
    public class AnmNullAnimation
    {
        /// <summary>【图层序数】Null ID</summary>
        [XmlAttribute("NullId")] public int NullId { get; set; }
        /// <summary>【是否可见】Is visible</summary>
        [XmlAttribute("Visible")] public bool Visible { get; set; }
        /// <summary>【关键帧列表】Frames</summary>
        [XmlElement("Frame")] public List<AnmFrame> Frames { get; set; }
        /// <summary>【所有帧】All frames</summary>
        public List<AnmFrame> AllFrames {
            get {
                if (allFrames.Count <= 0) {
                    allFrames.Clear();
                    BakeAllFrames();
                }
                return allFrames;
            }
        }
        private readonly List<AnmFrame> allFrames = new(); // 所有帧 | All frames

        public AnmNullAnimation() {
            NullId = 0;
            Visible = true;
            Frames = new List<AnmFrame>();
        }

        /// <summary>【获取指定帧索引的帧，包括补间生成的帧】Get frame by frame ID, include interpolated frames</summary>
        /// <param name="frameIndex">【帧索引】Frame ID</param>
        /// <returns>【非图集图层帧信息】Null frame</returns>
        public AnmFrame GetFrameByFrameIndex(int frameIndex) {
            if (AllFrames.Count > frameIndex) {
                if (frameIndex < 0) {
                    return AllFrames[0];
                }
                return AllFrames[frameIndex];
            }
            // 如果索引超出范围，则返回最后一帧
            else {
                if (AllFrames.Count > 0) {
                    return AllFrames[AllFrames.Count - 1];
                }
            }
            return null;
        }

        /// <summary>【获取帧数量】Get frame count</summary>
        public int GetFrameCount() {
            return AllFrames.Count;
        }

        /// <summary>【计算出所有帧信息】Bake all frames</summary>
        private void BakeAllFrames() {
            allFrames.Clear();
            // 遍历所有关键帧
            for (int i = 0; i < Frames.Count; i++) {
                // 如果帧间隔等于1，则直接作为普通帧
                if (Frames[i].Delay <= 1) {
                    allFrames.Add(Frames[i]);
                }
                // 如果帧间隔大于1，则需要计算补间帧
                else {
                    // 判断i+1帧(下一帧)是否存在
                    if (i + 1 < Frames.Count) {
                        for (int j = 0; j < Frames[i].Delay; j++) {
                            // 计算补间帧
                            AnmFrame interpFrame = Frames[i];
                            if (Frames[i].Interpolated) {
                                interpFrame = Frames[i].Lerp(Frames[i + 1], (float)j / Frames[i].Delay);
                            }
                            allFrames.Add(interpFrame);
                        }
                    }
                    // 如果不存在i+1帧(下一帧)，就把i帧填充Delay-1次
                    else {
                        for (int j = 0; j < Frames[i].Delay; j++) {
                            allFrames.Add(Frames[i]);
                        }
                    }
                }
            }
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmNullAnimation Copy() {
            AnmNullAnimation outNullAnimation = new() {
                NullId = NullId,
                Visible = Visible,
                Frames = new List<AnmFrame>()
            };
            foreach (var frame in Frames) {
                outNullAnimation.Frames.Add(frame.Copy());
            }
            if (outNullAnimation.allFrames.Count <= 0) {
                outNullAnimation.allFrames.Clear();
                outNullAnimation.BakeAllFrames(); // 缓存所有帧信息 | Cache all frame info
            }
            return outNullAnimation;
        }
    }
    /// <summary>【Anm2动画帧-通用】Anm2 animation frame (general)</summary>
    public class AnmFrame
    {
        /// <summary>【位置偏移X】Position X</summary>
        [XmlAttribute("XPosition")] public float XPosition { get; set; }
        /// <summary>【位置偏移Y】Position Y</summary>
        [XmlAttribute("YPosition")] public float YPosition { get; set; }
        /// <summary>【缩放X】Scale X</summary>
        [XmlAttribute("XScale")] public float XScale { get; set; }
        /// <summary>【缩放Y】Scale Y</summary>
        [XmlAttribute("YScale")] public float YScale { get; set; }
        /// <summary>【帧持续时间】Frame duration</summary>
        [XmlAttribute("Delay")] public int Delay { get; set; }
        /// <summary>【是否可见】Is visible</summary>
        [XmlAttribute("Visible")] public bool Visible { get; set; }
        /// <summary>【正片叠底色R】Red tint</summary>
        [XmlAttribute("RedTint")] public int RedTint { get; set; }
        /// <summary>【正片叠底色G】Green Tint</summary>
        [XmlAttribute("GreenTint")] public int GreenTint { get; set; }
        /// <summary>【正片叠底色B】Blue tint</summary>
        [XmlAttribute("BlueTint")] public int BlueTint { get; set; }
        /// <summary>【正片叠底色A】Alpha tint</summary>
        [XmlAttribute("AlphaTint")] public int AlphaTint { get; set; }
        /// <summary>【叠加色R】Red offset</summary>
        [XmlAttribute("RedOffset")] public int RedOffset { get; set; }
        /// <summary>【叠加色G】Green Offset</summary>
        [XmlAttribute("GreenOffset")] public int GreenOffset { get; set; }
        /// <summary>【叠加色B】Blue Offset</summary>
        [XmlAttribute("BlueOffset")] public int BlueOffset { get; set; }
        /// <summary>【Z轴旋转】Z rotation</summary>
        [XmlAttribute("Rotation")] public float Rotation { get; set; }
        /// <summary>【是否补间】Is interpolated</summary>
        [XmlAttribute("Interpolated")] public bool Interpolated { get; set; }

        public AnmFrame() {
            XPosition = 0; YPosition = 0;
            XScale = 1; YScale = 1;
            Delay = 1; Visible = true;
            RedTint = 255; GreenTint = 255; BlueTint = 255; AlphaTint = 255;
            RedOffset = 0; GreenOffset = 0; BlueOffset = 0;
            Rotation = 0; Interpolated = false;
        }

        /// <summary>【生成插值动画帧-通用】Make lerp animation frame (general)</summary>
        /// <param name="target">【目标动画帧-通用】Target frame</param>
        /// <param name="delta">【插值，0~1】Lerp delta</param>
        /// <returns>【插值后的动画帧-通用】Lerp frame</returns>
        public AnmFrame Lerp(AnmFrame target, float delta) {
            AnmFrame frame = new() {
                XPosition = Mathf.Lerp(XPosition, target.XPosition, delta),
                YPosition = Mathf.Lerp(YPosition, target.YPosition, delta),
                XScale = Mathf.Lerp(XScale, target.XScale, delta),
                YScale = Mathf.Lerp(YScale, target.YScale, delta),
                Delay = 1, // 插值帧的Delay设为1
                Visible = this.Visible,
                RedTint = Mathf.RoundToInt(Mathf.Lerp(RedTint, target.RedTint, delta)),
                GreenTint = Mathf.RoundToInt(Mathf.Lerp(GreenTint, target.GreenTint, delta)),
                BlueTint = Mathf.RoundToInt(Mathf.Lerp(BlueTint, target.BlueTint, delta)),
                AlphaTint = Mathf.RoundToInt(Mathf.Lerp(AlphaTint, target.AlphaTint, delta)),
                RedOffset = Mathf.RoundToInt(Mathf.Lerp(RedOffset, target.RedOffset, delta)),
                GreenOffset = Mathf.RoundToInt(Mathf.Lerp(GreenOffset, target.GreenOffset, delta)),
                BlueOffset = Mathf.RoundToInt(Mathf.Lerp(BlueOffset, target.BlueOffset, delta)),
                Rotation = Mathf.Lerp(Rotation, target.Rotation, delta),
                Interpolated = false // 插值帧的Interpolated设为false
            };
            return frame;
        }

        /// <summary>【测试输出】Test output print</summary>
        public override string ToString() {
            return $"[Frame] Position:({XPosition}, {YPosition}), Scale:({XScale}, {YScale}), Delay:{Delay}, Visible:{Visible}, Tint:({RedTint}, {GreenTint}, {BlueTint}, {AlphaTint}), Offset:({RedOffset}, {GreenOffset}, {BlueOffset}), Rotation:{Rotation}, Interpolated:{Interpolated}";
        }
        
        /// <summary>【复制类】Copy type</summary>
        public AnmFrame Copy() {
            return new AnmLayerFrame() {
                XPosition = XPosition, YPosition = YPosition,
                XScale = XScale, YScale = YScale,
                Delay = Delay, Visible = Visible,
                RedTint = RedTint, GreenTint = GreenTint, BlueTint = BlueTint, AlphaTint = AlphaTint,
                RedOffset = RedOffset, GreenOffset = GreenOffset, BlueOffset = BlueOffset,
                Rotation = Rotation, Interpolated = Interpolated
            };
        }
    }
    /// <summary>【Anm2动画帧-图集图层】Anm2 animation frame (Sprite sheet layers)</summary>
    public class AnmLayerFrame : AnmFrame
    {
        /// <summary>【锚点偏移X】Pivot offset X</summary>
        [XmlAttribute("XPivot")] public float XPivot { get; set; }
        /// <summary>【锚点偏移Y】Pivot offset Y</summary>
        [XmlAttribute("YPivot")] public float YPivot { get; set; }
        /// <summary>【裁切坐标X】Crop rect (top left) position X</summary>
        [XmlAttribute("XCrop")] public float XCrop { get; set; }
        /// <summary>【裁切坐标Y】Crop rect (top left) position Y</summary>
        [XmlAttribute("YCrop")] public float YCrop { get; set; }
        /// <summary>【裁切宽度】Crop rect width</summary>
        [XmlAttribute("Width")] public float Width { get; set; }
        /// <summary>【裁切高度】Crop rect height</summary>
        [XmlAttribute("Height")] public float Height { get; set; }

        public AnmLayerFrame() {
            XPivot = 0; YPivot = 0;
            XCrop = 0; YCrop = 0;
            Width = 0; Height = 0;
            XPosition = 0; YPosition = 0;
            XScale = 1; YScale = 1;
            Delay = 1; Visible = true;
            RedTint = 255; GreenTint = 255; BlueTint = 255; AlphaTint = 255;
            RedOffset = 0; GreenOffset = 0; BlueOffset = 0;
            Rotation = 0; Interpolated = false;
        }

        /// <summary>【生成插值动画帧-图集图层】Make lerp animation frame (Sprite sheet layers)</summary>
        /// <param name="target">【目标动画帧-图集图层】Target frame</param>
        /// <param name="delta">【插值，0~1】Lerp delta</param>
        /// <returns>【插值后的动画帧-图集图层】Lerp frame</returns>
        public AnmLayerFrame Lerp(AnmLayerFrame target, float delta) {
            AnmLayerFrame frame = new() {
                XPivot = XPivot, // 锚点不允许插值
                YPivot = YPivot,
                XCrop = XCrop, // 裁切坐标不允许插值
                YCrop = YCrop,
                Width = Width, // 裁切宽高不允许插值
                Height = Height,
                XPosition = Mathf.Lerp(XPosition, target.XPosition, delta),
                YPosition = Mathf.Lerp(YPosition, target.YPosition, delta),
                XScale = Mathf.Lerp(XScale, target.XScale, delta),
                YScale = Mathf.Lerp(YScale, target.YScale, delta),
                Delay = 1, // 插值帧的Delay设为1
                Visible = this.Visible,
                RedTint = Mathf.RoundToInt(Mathf.Lerp(RedTint, target.RedTint, delta)),
                GreenTint = Mathf.RoundToInt(Mathf.Lerp(GreenTint, target.GreenTint, delta)),
                BlueTint = Mathf.RoundToInt(Mathf.Lerp(BlueTint, target.BlueTint, delta)),
                AlphaTint = Mathf.RoundToInt(Mathf.Lerp(AlphaTint, target.AlphaTint, delta)),
                RedOffset = Mathf.RoundToInt(Mathf.Lerp(RedOffset, target.RedOffset, delta)),
                GreenOffset = Mathf.RoundToInt(Mathf.Lerp(GreenOffset, target.GreenOffset, delta)),
                BlueOffset = Mathf.RoundToInt(Mathf.Lerp(BlueOffset, target.BlueOffset, delta)),
                Rotation = Mathf.Lerp(Rotation, target.Rotation, delta),
                Interpolated = false // 插值帧的Interpolated设为false
            };
            return frame;
        }

        /// <summary>【测试输出】Test output print</summary>
        public override string ToString() {
            return $"[Layer Frame] Pivot:({XPivot}, {YPivot}), Crop:({XCrop}, {YCrop}), Size:({Width}, {Height}), Position:({XPosition}, {YPosition}), Scale:({XScale}, {YScale}), Delay:{Delay}, Visible:{Visible}, Tint:({RedTint}, {GreenTint}, {BlueTint}, {AlphaTint}), Offset:({RedOffset}, {GreenOffset}, {BlueOffset}), Rotation:{Rotation}, Interpolated:{Interpolated}";
        }

        /// <summary>【复制类】Copy type</summary>
        public new AnmLayerFrame Copy() {
            return new AnmLayerFrame() {
                XPivot = XPivot, YPivot = YPivot,
                XCrop = XCrop, YCrop = YCrop,
                Width = Width, Height = Height,
                XPosition = XPosition, YPosition = YPosition,
                XScale = XScale, YScale = YScale,
                Delay = Delay, Visible = Visible,
                RedTint = RedTint, GreenTint = GreenTint, BlueTint = BlueTint, AlphaTint = AlphaTint,
                RedOffset = RedOffset, GreenOffset = GreenOffset, BlueOffset = BlueOffset,
                Rotation = Rotation, Interpolated = Interpolated
            };
        }
    }
    /// <summary>【Anm2动画帧-事件图层】Anm2 animation frame (Trigger layer)</summary>
    public class AnmTriggerFrame
    {
        /// <summary>【事件序数】Event ID</summary>
        [XmlAttribute("EventId")] public int EventId { get; set; }
        /// <summary>【帧数序数】Frame ID</summary>
        [XmlAttribute("AtFrame")] public int AtFrame { get; set; }

        public AnmTriggerFrame() {
            EventId = 0;
            AtFrame = 0;
        }

        /// <summary>【复制类】Copy type</summary>
        public AnmTriggerFrame Copy() {
            return new AnmTriggerFrame() {
                EventId = EventId,
                AtFrame = AtFrame
            };
        }
    }
    #endregion
}