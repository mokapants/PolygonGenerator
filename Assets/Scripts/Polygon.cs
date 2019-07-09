using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    private Mesh meshData;
    public Mesh MeshData
    {
        get { return this.meshData; }
        set { this.meshData = value; }
    }

    private Material material;
    public Material Material { get { return this.material; } }

    private Picture picture;
    public Picture Picture
    {
        get { return this.picture; }
        set { this.picture = value; }
    }

    private bool isLoadComplete;
    public bool IsLoadComplete
    {
        get { return this.isLoadComplete; }
        set { this.isLoadComplete = value; }
    }

    /// <summary>
    /// 虚無
    /// </summary>
    public Polygon()
    {
        isLoadComplete = false;
    }

    /// <summary>
    /// 3つの頂点からPoygonを生成。
    /// </summary>
    /// <param name="shader">適用するShader</param>
    public Polygon(Shader shader)
    {
        isLoadComplete = false;

        material = new Material(shader);
        material.color = Color.blue;

        Vector3[] vertices = new Vector3[3] { Vector3.zero, Vector3.right, Vector3.up }; // Polygonの頂点
        int[] triangles = new int[3] { 0, 2, 1 }; // Polygonの頂点を結ぶ順番
        Vector3[] normals = new Vector3[3] { Vector3.back, Vector3.back, Vector3.back }; // Polygonの法線

        meshData = new Mesh();
        meshData.vertices = vertices;
        meshData.triangles = triangles;
        meshData.normals = normals;
        meshData.RecalculateBounds();

        isLoadComplete = true;
    }

    /// <summary>
    /// 画像のデータからPolygonを生成。
    /// 頂点数65535以降は表示されない(Unityの仕様)
    /// </summary>
    /// <param name="path">画像のPath</param>
    /// <param name="shader">適用するShader</param>
    public Polygon(string path, Shader shader)
    {
        isLoadComplete = false;

        picture = new Picture(path);

        // Materialの設定
        material = new Material(shader);
        material.mainTexture = picture.PictureTex2D;

        int width = picture.Width;
        int height = picture.Height;

        int vertexAmount = width * height; // Polygonの頂点数
        Vector3[] normals = new Vector3[vertexAmount]; // Polygonの法線
        Vector3[] vertices = new Vector3[vertexAmount]; // Polygonの頂点
        Vector2[] uv = new Vector2[vertexAmount]; // UVをつかって貼るテクスチャ用

        int surfaceAmount = vertexAmount * 2; // Polygonの面数(三角形で1面だから2倍)
        int[, ] trianglesTwoArray = new int[surfaceAmount, 3]; // Polygonの頂点を結ぶ順番

        // Polygonの頂点を設定，UV座標の設定(テクスチャの貼り付け)
        int counter = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 polygonPosition = new Vector3(x, y, 0);
                vertices[counter] = polygonPosition;

                // UV座標の設定(分母の最大が1だから，xをwidthで，yをheightで割る)
                uv[counter] = new Vector2((float) x / (width - 1), (float) y / (height - 1));

                counter++;
            }
        }

        // Polygonの頂点を結ぶ順番を設定
        int polygonVertexNow = 0;
        counter = 0;
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == width - 1)
                {
                    polygonVertexNow++;
                    break;
                }

                /*
                |---------|
                | 左上の ／|
                | 三角 ／  |
                |   ／ 右下|
                | ／ の三角|
                |_________|
                 */
                // 左上側の三角形のPolygon
                trianglesTwoArray[counter, 0] = polygonVertexNow;
                trianglesTwoArray[counter, 1] = polygonVertexNow + width;
                trianglesTwoArray[counter, 2] = polygonVertexNow + width + 1;

                counter++;

                // 右下側の三角形のPolygon
                trianglesTwoArray[counter, 0] = polygonVertexNow;
                trianglesTwoArray[counter, 1] = polygonVertexNow + width + 1;
                trianglesTwoArray[counter, 2] = polygonVertexNow + 1;

                counter++;

                polygonVertexNow++;
            }
        }

        // trianglesTwoArray -> trianglesに変換
        int forTriangles = surfaceAmount * 3;
        int[] triangles = new int[forTriangles];
        counter = 0;
        for (int c = 0; c < forTriangles; c += 3)
        {
            triangles[c] = trianglesTwoArray[counter, 0];
            triangles[c + 1] = trianglesTwoArray[counter, 1];
            triangles[c + 2] = trianglesTwoArray[counter, 2];

            counter++;
        }

        // Polygonの法線を設定
        for (counter = 0; counter < vertexAmount; counter++)
        {
            normals[counter] = Vector3.back;
        }

        meshData = new Mesh();
        meshData.vertices = vertices;
        meshData.triangles = triangles;
        meshData.normals = normals;
        meshData.uv = uv;
        meshData.RecalculateBounds();

        isLoadComplete = true;
    }

    // GrayscaleでPolygonのZ座標を書き換えて起伏を設定
    public void SetGrayscale()
    {
        float[] gray = picture.GetGrayscale();
        int width = picture.Width;
        int height = picture.Height;

        Vector3[] vertices = meshData.vertices; // Polygonの頂点

        int counter = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int irregularity = 25;
                vertices[counter].z = irregularity * gray[counter];
                counter++;
            }
        }

        meshData.vertices = vertices;
        meshData.RecalculateBounds();
    }

    /// <summary>
    /// Polygonの頂点の座標を更新する。
    /// </summary>
    /// <param name="shader">適用するShader</param>
    public void UpdateVertexPosition(Vector3[] vertices)
    {
        meshData.vertices = vertices;
    }
}