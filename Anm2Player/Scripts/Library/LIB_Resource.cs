using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iamsleepingnow.Anm2Player
{
    /// <summary>【全局综合库】Global Library for Anm2Player</summary>
    public static partial class LIBRARY
    {
        #region Resource 资源
        /// <summary>【设置Texture2D过滤模式】Set Tecture2D filter mode</summary>
        /// <param name="texture">【贴图】Texture2D</param>
        /// <param name="mode">【过滤模式】Filter mode</param>
        public static Texture2D SetFilterMode(this Texture2D texture, FilterMode mode = FilterMode.Point) {
            texture.filterMode = mode;
            return texture;
        }

        /// <summary>【设置Texture2D wrapping模式】Set Texture2D wrapping mode</summary>
        /// <param name="texture">【贴图】Texture2D</param>
        /// <param name="mode">【wrapping模式】Wrapping mode</param>
        public static Texture2D SetWrapMode(this Texture2D texture, TextureWrapMode mode = TextureWrapMode.Clamp) {
            texture.wrapMode = mode;
            return texture;
        }

        /// <summary>【设置贴图Mipmap】Set Texture2D mipmap</summary>
        /// <param name="texture">【贴图】Texture2D</param>
        /// <param name="levelCount">【等级数量】Level count</param>
        /// <param name="defaultLevel">【默认等级】Requested level</param>
        /// <param name="bias">【偏移等级】Mipmap bias</param>
        public static Texture2D SetMipmap(this Texture2D texture, int levelCount = 5, int defaultLevel = 1, int bias = 0) {
            texture.ClearMinimumMipmapLevel();
            texture.ClearRequestedMipmapLevel();
            texture.minimumMipmapLevel = levelCount;
            texture.requestedMipmapLevel = defaultLevel;
            texture.mipMapBias = bias;
            return texture;
        }

        /// <summary>【创建一个四边形网格】Create a quad mesh</summary>
        /// <param name="scale">【缩放】Mesh scale</param>
        /// <returns>【网格】Mesh</returns>
        public static Mesh CreateQuadMesh(Vector2 scale) {
            Mesh mesh = new() {
                name = "New Quad"
            };
            List<Vector3> vertexList = new();
            List<Vector3> normalList = new();
            List<Vector2> uvList = new();
            List<int> triangleList = new();
            // 添加顶点
            vertexList.Add(new Vector3(-scale.x * 0.5f, -scale.y * 0.5f, 0f));   // 左下
            vertexList.Add(new Vector3(scale.x * 0.5f, -scale.y * 0.5f, 0f));    // 右下  
            vertexList.Add(new Vector3(-scale.x * 0.5f, scale.y * 0.5f, 0f));    // 左上
            vertexList.Add(new Vector3(scale.x * 0.5f, scale.y * 0.5f, 0f));     // 右上
            // 添加法线
            normalList.Add(new Vector3(0f, 0f, -1f));
            // 添加UV
            uvList.Add(new Vector2(0f, 0f));                // 左下 - uv0
            uvList.Add(new Vector2(1f, 0f));                // 右下 - uv1  
            uvList.Add(new Vector2(0f, 1f));                // 左上 - uv2
            uvList.Add(new Vector2(1f, 1f));                // 右上 - uv3
            // 第一个三角形: f 2/1/1 3/2/1 1/3/1
            triangleList.Add(0);
            triangleList.Add(2);
            triangleList.Add(1);
            // 第二个三角形: f 2/1/1 4/4/1 3/2/1
            triangleList.Add(2);
            triangleList.Add(3);
            triangleList.Add(1);
            // 设置网格数据
            mesh.vertices = vertexList.ToArray();
            mesh.triangles = triangleList.ToArray();
            mesh.uv = uvList.ToArray();
            // 设置法线
            Vector3[] vertexNormals = new Vector3[vertexList.Count];
            for (int i = 0; i < vertexNormals.Length; i++) {
                vertexNormals[i] = normalList[0];
            }
            mesh.normals = vertexNormals;
            mesh.RecalculateBounds();
            return mesh;
        }
        #endregion
    }
}