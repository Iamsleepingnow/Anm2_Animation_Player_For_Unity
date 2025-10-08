using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;//正则表达式 RE
using System.Xml.Serialization;//XML结构序列化框架 XML

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【全局综合库】Global Library for Anm2Player</summary>
    public static partial class LIBRARY
    {
        #region Text 文本
        /// <summary>
        /// <para>【将文本txtText写入"folderPath\fileName.fileSuffix"中】Write txtText into "folderPath\fileName.fileSuffix"</para>
        /// <para>EX: Global.WriteTextFile("测试文本", $"D:\\TestSample\\Test", "SampleText");</para>
        /// <para>EX: Global.WriteTextFile("测试文本", $"{Application.persistentDataPath}\\Test", "SampleText");</para>
        /// </summary>
        /// <param name="txtText">【写入文本】Text</param>
        /// <param name="folderPath">【写入文件路径】Folder path</param>
        /// <param name="fileName">【写入文件名】File name</param>
        /// <param name="fileExtension">【写入文件后缀】File extension</param>
        public static void WriteTextFile(string txtText, string folderPath, string fileName, string fileExtension = ".txt") {
            string folder = $"{folderPath}";
            //若不存在路径
            if (!Directory.Exists(folder)) {
                DirectoryInfo info = new DirectoryInfo(folder);
                info.Create();
            }
            string path = $"{folder}\\{fileName}{fileExtension}";//文件流创建一个文本文件
            FileStream file = File.Exists(path) ? new FileStream(path, FileMode.Truncate) : new FileStream(path, FileMode.Create);
            byte[] bts = System.Text.Encoding.UTF8.GetBytes(txtText);//文件写入数据流
            file.Write(bts, 0, bts.Length);
            //当数据流存在时
            if (file != null) {
                file.Flush();//清空缓存
                file.Close();//关闭流
                file.Dispose();//销毁资源
            }
        }
        /// <summary>
        /// <para>【将文本txtText写入fullFilePath中】Write txtText into "folderPath\fileName.fileSuffix"</para>
        /// <para>EX: Global.WriteTextFile("测试文本", $"D:\\TestSample\\Test\\SampleText.txt");</para>
        /// <para>EX: Global.WriteTextFile("测试文本", $"{Application.persistentDataPath}\\Test\\SampleText.txt");</para>
        /// </summary>
        /// <param name="txtText">【写入文本】Text</param>
        /// <param name="fullFilePath">【写入文件路径】Full file path</param>
        public static void WriteTextFile(string txtText, string fullFilePath) {
            string fileDirectory = Path.GetDirectoryName(fullFilePath);
            string fileName = Path.GetFileNameWithoutExtension(fullFilePath);
            string fileExtension = Path.GetExtension(fullFilePath);
            WriteTextFile(txtText, fileDirectory, fileName, fileExtension);
        }
        /// <summary>
        /// <para>【将文本txtText写入AppData的childrenPath中】Write txtText into AppData into childrenPath</para>
        /// <para>EX: Global.WriteTextFileToAppData("测试文本", $"Test\\SampleText.txt");</para>
        /// </summary>
        /// <param name="txtText">【写入文本】Text</param>
        /// <param name="childrenPath">【写入文件子路径】Children path</param>
        public static void WriteTextFileToAppData(string txtText, string childrenPath) {
            WriteTextFile(txtText, $"{Application.persistentDataPath}\\{childrenPath}");
        }
        /// <summary>
        /// <para>【读取"folderPath\fileName.fileSuffix"中文本】Read text from "folderPath\fileName.fileSuffix"</para>
        /// <para>EX: print(Global.ReadTextFile($"D:\\TestSample\\Test", "SampleText"));</para>
        /// <para>EX: print(Global.ReadTextFile($"{Application.persistentDataPath}\\Test", "SampleText"));</para>
        /// </summary>
        /// <param name="folderPath">【读取文件路径】Folder path</param>
        /// <param name="fileName">【读取文件名】File name</param>
        /// <param name="fileExtension">【读取文件后缀】File extension</param>
        /// <returns>【读取文件内容】Read file content</returns>
        public static string ReadTextFile(string folderPath, string fileName, string fileExtension = ".txt") {
            string folder = $"{folderPath}";
            if (!Directory.Exists(folder))//若不存在路径
            {
                return string.Empty;
            }
            string path = $"{folder}\\{fileName}{fileExtension}";
            StreamReader reader = null;
            reader = File.OpenText(path);
            string fileRawText = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            return fileRawText;
        }
        /// <summary>
        /// <para>【读取fullFilePath中文本】Read text from fullFilePath</para>
        /// <para>EX: print(Global.ReadTextFile($"D:\\TestSample\\Test\\SampleText.txt"));</para>
        /// <para>EX: print(Global.ReadTextFile($"{Application.persistentDataPath}\\Test\\SampleText.txt"));</para>
        /// </summary>
        /// <param name="fullFilePath">【读取文件路径】Full file path</param>
        /// <returns>【读取文件内容】Read file content</returns>
        public static string ReadTextFile(string fullFilePath) {
            string fileDirectory = Path.GetDirectoryName(fullFilePath);
            string fileName = Path.GetFileNameWithoutExtension(fullFilePath);
            string fileExtension = Path.GetExtension(fullFilePath);
            return ReadTextFile(fileDirectory, fileName, fileExtension);
        }

        public const string PATH_TYPE_COMPONENT_KEYWORD = "path: [//COMPONENT//]"; // 组件路径关键字 | Component path keyword

        /// <summary>【根据文件路径类型来拼合路径字符串】Assemble the path string according to the file path type.</summary>
        /// <param name="pathType">【文件路径类型】</param>
        /// <param name="path">【剩余路径】</param>
        /// <param name="pathType">【文件路径类型】File path type</param>
        /// <param name="path">【剩余路径】Remaining path</param>
        /// <returns>【拼合输出字符串（未进行存在性检测）】Combine output string (without existence check)</returns>
        public static string ConcatPathByPathType(FilePathType pathType, string path) {
            string outPath = pathType switch {
                LIBRARY.FilePathType.StreamingAssets => path == string.Empty ? Application.streamingAssetsPath : System.IO.Path.Combine(Application.streamingAssetsPath, path),
                LIBRARY.FilePathType.AppData => path == string.Empty ? Application.persistentDataPath : System.IO.Path.Combine(Application.persistentDataPath, path),
                LIBRARY.FilePathType.Custom => path,
                LIBRARY.FilePathType.Component => PATH_TYPE_COMPONENT_KEYWORD,
                _ => path == string.Empty ? Application.streamingAssetsPath : System.IO.Path.Combine(Application.streamingAssetsPath, path),
            };
            return outPath;
        }
        #endregion

        #region Texture 图像
        /// <summary>【同步：从指定路径加载Texture2D图像】Synchronize: Load Texture2D image from the specified path</summary>
        /// <param name="pathType">【文件路径类型】File path type</param>
        /// <param name="path">【图像文件相对路径】Image file relative path</param>
        /// <returns>【加载的Texture2D对象，加载失败返回null】</returns>
        public static Texture2D LoadImageFile(FilePathType pathType, string path) {
            string fullPath = ConcatPathByPathType(pathType, path);
            // 如果是Android平台
            if (pathType == FilePathType.StreamingAssets && Application.platform == RuntimePlatform.Android) {
                return LoadImageFromAndroidStreamingAssetsSync(fullPath); // 阻塞进程，同步加载
            }
            if (!File.Exists(fullPath)) {
                Debug.LogError($"[Anm2Player] 图像文件不存在 | Image file is not exist: {fullPath}");
                return null;
            }
            try {
                byte[] imageData = FileReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(2, 2);
                bool success = texture.LoadImage(imageData);
                if (success) {
                    return texture;
                }
                else {
                    Debug.LogError($"[Anm2Player] 无法加载图像 | Can't load image file: {fullPath}");
                    return null;
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"[Anm2Player] 加载图像时出错 | Error when loading image file: {fullPath}\n错误信息 | Error: {e.Message}");
                return null;
            }
        }

        /// <summary>【同步：从指定路径列表加载Texture2D图像列表】Synchronize: Load a list of Texture2D images from the specified path list</summary>
        /// <param name="pathType">【文件路径类型】File path type</param>
        /// <param name="paths">【图像文件相对路径列表】Image file relative path list</param>
        /// <returns>【加载的Texture2D对象列表，失败的加载会对应位置为null】</returns>
        public static List<Texture2D> LoadImageFiles(FilePathType pathType, List<string> paths) {
            List<Texture2D> textures = new List<Texture2D>();
            foreach (string path in paths) {
                textures.Add(LoadImageFile(pathType, path));
            }
            return textures;
        }

        /// <summary>【异步：从指定路径加载Texture2D图像】Asynchronize: Load Texture2D image from the specified path</summary>
        /// <param name="pathType">【文件路径类型】File path type</param>
        /// <param name="path">【图像文件相对路径】Image file relative path</param>
        /// <returns>【加载的Texture2D对象的Task】</returns>
        public static async Task<Texture2D> LoadImageFileAsync(FilePathType pathType, string path) {
            string fullPath = ConcatPathByPathType(pathType, path);
            // 如果是Android平台
            if (pathType == FilePathType.StreamingAssets && Application.platform == RuntimePlatform.Android) {
                return await LoadImageFromAndroidStreamingAssetsAsync(fullPath);
            }
            if (!File.Exists(fullPath)) {
                Debug.LogError($"[Anm2Player] 图像文件不存在 | Image file is not exist: {fullPath}");
                return null;
            }
            try {
                byte[] imageData = await FileReadAllBytesAsync(fullPath);
                Texture2D texture = new Texture2D(2, 2);
                bool success = texture.LoadImage(imageData);
                if (success) {
                    return texture;
                }
                else {
                    Debug.LogError($"[Anm2Player] 无法加载图像 | Can't load image file: {fullPath}");
                    return null;
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"[Anm2Player] 加载图像时出错 | Error when loading image file: {fullPath}\n错误信息 | Error: {e.Message}");
                return null;
            }
        }

        /// <summary>【异步：从指定路径列表加载Texture2D图像列表】Asynchronize: Load a list of Texture2D images from the specified path list</summary>
        /// <param name="pathType">【文件路径类型】File path type</param>
        /// <param name="paths">【图像文件相对路径列表】Image file relative path list</param>
        /// <returns>【加载的Texture2D对象列表的Task，失败的加载会对应位置为null】</returns>
        public static async Task<List<Texture2D>> LoadImageFilesAsync(FilePathType pathType, List<string> paths) {
            List<Texture2D> textures = new List<Texture2D>();
            List<Task<Texture2D>> tasks = new List<Task<Texture2D>>();
            foreach (string path in paths) {
                tasks.Add(LoadImageFileAsync(pathType, path));
            }
            Texture2D[] results = await Task.WhenAll(tasks); // 等待所有任务完成
            textures.AddRange(results); // 将结果添加到列表
            return textures;
        }

        /// <summary>【同步读取文件字节】Synchronize file byte reading</summary>
        /// <param name="path">【文件路径】File path</param>
        /// <returns>【字节数组】Byte array</returns>
        private static byte[] FileReadAllBytes(string path) {
            return File.ReadAllBytes(path);
        }

        /// <summary>【异步读取文件字节】Asynchronize file byte reading</summary>
        /// <param name="path">【文件路径】File path</param>
        /// <returns>【字节数组的异步Task】Byte array task</returns>
        private static async Task<byte[]> FileReadAllBytesAsync(string path) {
            using (FileStream sourceStream = new FileStream(path,
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true)) {
                byte[] buffer = new byte[sourceStream.Length];
                await sourceStream.ReadAsync(buffer, 0, (int)sourceStream.Length);
                return buffer;
            }
        }

        /// <summary>【Android平台兼容：同步加载StreamingAssets路径下的图像】Android platform compatibility: Synchronously load images under the StreamingAssets path</summary>
        /// <param name="path">【文件路径】File path</param>
        /// <returns>【Texture2D图像】Texture</returns>
        private static Texture2D LoadImageFromAndroidStreamingAssetsSync(string path) {
            using (UnityWebRequest www = UnityWebRequest.Get(path)) {
                www.SendWebRequest();
                // 等待请求完成（阻塞主线程）
                while (!www.isDone) { }
                if (www.result != UnityWebRequest.Result.Success) {
                    Debug.LogError($"加载图像失败(Failed to load image file): {path}\n错误(Error): {www.error}");
                    return null;
                }
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(www.downloadHandler.data)) {
                    return texture;
                }
                else {
                    Debug.LogError($"无法解码图像(Can't decode image file): {path}");
                    return null;
                }
            }
        }

        /// <summary>【Android平台兼容：异步加载StreamingAssets路径下的图像】Android platform compatibility: Asynchronously load images under the StreamingAssets path</summary>
        /// <param name="path">【文件路径】File path</param>
        /// <returns>【Texture2D图像的异步Task(Data task)】Texture data task</returns>
        private static async Task<Texture2D> LoadImageFromAndroidStreamingAssetsAsync(string path) {
            using (UnityWebRequest www = UnityWebRequest.Get(path)) {
                var operation = www.SendWebRequest(); // 发送请求并等待完成
                while (!operation.isDone) {
                    await Task.Yield();
                }
                if (www.result != UnityWebRequest.Result.Success) {
                    Debug.LogError($"加载图像失败(Failed to load image file): {path}\n错误(Error): {www.error}");
                    return null;
                }
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(www.downloadHandler.data)) {
                    return texture;
                }
                else {
                    Debug.LogError($"无法解码图像(Can't decode image file): {path}");
                    return null;
                }
            }
        }
        #endregion

        #region XML 流读写
        /// <summary>【写入XML流】Write XML stream</summary>
        /// <typeparam name="T">【泛型】Template</typeparam>
        /// <param name="target">【目标类型物体】Target type object</param>
        /// <param name="fullFilePath">【完整文件路径】Full path</param>
        public static void XMLWrite<T>(this T target, string fullFilePath) {
            using (FileStream stream = new(fullFilePath, FileMode.Create, FileAccess.Write)) {
                XmlSerializer serializer = new(typeof(T));
                serializer.Serialize(stream, target);
            }
            string text = ReadTextFile(fullFilePath);
            Regex regex = new(" xmlns:.*?\"\\>");
            string regText = regex.Replace(text, ">");
            WriteTextFile(regText, fullFilePath);
        }
        /// <summary>【从XML流中读取】Read from XML stream</summary>
        /// <typeparam name="T">【泛型】Template</typeparam>
        /// <param name="fullFilePath">【完整文件路径】Full path</param>
        /// <returns>【提取的类型物体】</returns>
        public static T XMLRead<T>(this string fullFilePath) {
            using (FileStream stream = new(fullFilePath, FileMode.Open, FileAccess.Read)) {
                XmlSerializer serializer = new(typeof(T));
                T t = (T)serializer.Deserialize(stream);
                return t;
            }
        }
        /// <summary>【从XML流中读取（TextAsset版本）】Read from XML stream (TextAsset version)</summary>
        /// <typeparam name="T">【泛型】Template</typeparam>
        /// <param name="fileAsset">【TextAsset文件资源】TextAsset file asset</param>
        /// <returns>【提取的类型物体】Extracted type object</returns>
        public static T XMLRead<T>(this TextAsset fileAsset) {
            // 将TextAsset的文本内容转换为字节数组，并使用UTF-8编码
            byte[] byteArray = Encoding.UTF8.GetBytes(fileAsset.text);
            using (MemoryStream stream = new(byteArray)) {
                XmlSerializer serializer = new(typeof(T));
                T t = (T)serializer.Deserialize(stream);
                return t;
            }
        }
        #endregion

        #region Enumeration 枚举
        /// <summary>
        /// <para>【保存路径类型】File Path Type</para>
        /// <para>SteamingAsset: 地址：工程目录/Assets/StreamingAssets，打包目录/工程_Data/StreamingAssets</para>
        /// <para>AppData: 地址：C:/Users/用户名/AppData/LocalLow/工程公司名/工程名</para>
        /// <para>Custom: 自定义地址</para>
        /// <para>Component: 组件加载的数据</para>
        /// <para>SteamingAsset: Path：*Project path*/Assets/StreamingAssets，*Build path*/*Project name*_Data/StreamingAssets</para>
        /// <para>AppData: Path：C:/Users/*User name*/AppData/LocalLow/*Company name*/*Project name*</para>
        /// <para>Custom: Custom path</para>
        /// <para>Component: Component loaded data</para>
        /// </summary>
        public enum FilePathType
        {
            /// <summary>【地址：工程目录/Assets/StreamingAssets，打包目录/工程_Data/StreamingAssets】Path：*Project path*/Assets/StreamingAssets，*Build path*/*Project name*_Data/StreamingAssets</summary>
            StreamingAssets = 0,
            /// <summary>【地址：C:/Users/用户名/AppData/LocalLow/工程公司名/工程名】Path：C:/Users/*User name*/AppData/LocalLow/*Company name*/*Project name*</summary>
            AppData = 1,
            /// <summary>【自定义地址】Custom path</summary>
            Custom = 2,
            /// <summary>【使用组件内部加载的数据】Use component loaded data inside</summary>
            Component = 3,
        }
        #endregion
    }
}