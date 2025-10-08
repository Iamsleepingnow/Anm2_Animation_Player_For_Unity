# Anm2 Animation Player For Unity

## 介绍 | Intro

本仓库是我为以撒Anm2动画导入到Unity引擎中而制作的兼容小工具。 | This repository is a compatibility utility I made for importing Isaac's Anm2 animations into the Unity engine.

目前已经完善了基础功能，可以以二维独立游戏动画播放器的形式使用。 | The basic functions have been completed and can be used as a 2D inde game animation player.


## 引擎与插件前置 | Engine and Plugins Required

Unity测试版本为`2022.3.43f1c1`，尚未测试其他版本引擎。 | Unity test version is `2022.3.43f1c1`, and other engine versions have not been tested yet.

本项目需要的额外Unity包： | Additional Unity packages required for this project:  
- `Shader Graph` `[14.0.11]`
- `Universal RP` `[14.0.11]` (可选) (Optional)

本项目所使用的第三方插件： | Third-party plugins used in this project:
- `Naughty Attributes` [2.1.4]

注意：`Naughty Attributes`暂未在本仓库中进行分发，所以你需要在[这里](https://github.com/dbrizov/NaughtyAttributes)进行安装。 | Note: `Naughty Attributes` is not distributed in this repository, so you need to install it [here](https://github.com/dbrizov/NaughtyAttributes).


## 特性 | Features

### Anm管理器 | Anm Managers

1. `AnmTimerManager`
	- Anm全局计时器管理器 | Anm global timer manager
	- 场景单例，需要手动实例化在场景中。 | Scene singleton, needs to be manually instantiated in the scene.
	- 负责集中管理所有动画的计时器更新。 | Responsible for centrally managing the timer updates for all animations.

2. `AnmDataHandler`
	- Anm全局数据分发器 | Anm global data distributor
	- 场景单例，需要手动实例化在场景中。 | Scene singleton, needs to be manually instantiated in the scene.
	- 全局数据共享管理器，用于配置全局信息。 | Global Data Sharing Manager for configuring global information.

3. `AnmCacheManager`
	- Anm文件缓存管理器 | Anm file cache manager
	- 场景单例，需要手动实例化在场景中。 | Scene singleton, needs to be manually instantiated in the scene.
	- 将动画文件信息缓存在内存中，避免频繁读取。 | Cache the animation file information in memory to avoid frequent reading.


### Anm播放器 | Anm Players

1. `AnmSprite`
	- Anm基础动画播放器 | Anm basic animation player
	- 用于Anm动画在游戏物体上的播放。 | Used to play Anm animations on game objects.

2. `AnmFileRuntime`
	- Anm文件快捷导入器 | Anm file quick importer
	- 当`AnmSprite`的`AutoLoadFilePath`为`Component`时，可以以`AutoLoadAnmFile`来设置项目中的文件。 | When `AnmSprite`'s `AutoLoadFilePath` is set to `Component`, you can use `AutoLoadAnmFile` to set the file in the project.

3. `AnmRootLayerRuntime`
	- Anm动画的根图层组件 | Anm animation's root layer component
	- 用于设置根图层。 | Set the root layer.

4. `AnmSpriteLayerRuntime`
	- Anm动画的图集图层组件 | Anm animation's sprite sheet layer component
	- 用于设置图集图层。 | Set the sprite sheet layer.

5. `AnmNullLayerRuntime`
	- Anm动画的空图层组件 | Anm animation's null layer component
	- 用于设置空图层。 | Set the null layer.


### 测试 | Test

1. `AnmDebug`
	- Anm文件解析测试组件 | Anm File Parsing Test Component
	- 可以尝试解析Anm2文件中的数据。 | You can try parsing data from Anm2 files.


## 演示场景 | Demo

1. Demo1_SpriteDrive
2. Demo2_Character
3. Demo3_CPUPressure
4. Demo4_CPUPressure2


## 版权声明 | Copyright Notice

本项目为原始档案，作者：Iamsleepingnow。  
This project is the original archive, developed by "Iamsleepingnow".


## 许可证 | License

本项目代码受 [GNU GPL v3.0](https://www.gnu.org/licenses/gpl-3.0.html) 协议保护。 | The code of this project is licensed under the [GNU GPL v3.0](https://www.gnu.org/licenses/gpl-3.0.html) license.

本项目所使用的字体文件`fusion-pixel-12px-proportional-zh_hans.ttf`受 [SIL OPEN FONT LICENSE v1.1](https://openfontlicense.org) 协议保护。Copyright (c) 2022, TakWolf。 | The font file used in this project `fusion-pixel-12px-proportional-zh_hans.ttf` is protected by the [SIL OPEN FONT LICENSE v1.1](https://openfontlicense.org) agreement. Copyright (c) 2022, TakWolf.
