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

    public Polygon()
    {
        isLoadComplete = false;
    }

    /// <summary>
    /// Pictureのデータから3Dに変換するクラス
    /// </summary>
    /// <param name="path">画像のPath</param>
    public Polygon(string path, Shader shader)
    {
        picture = new Picture(path, shader);
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
        int fortriangles = surfaceAmount * 3;
        int[] triangles = new int[fortriangles];
        counter = 0;
        for (int c = 0; c < fortriangles; c += 3)
        {
            triangles[c] = trianglesTwoArray[counter, 0];
            triangles[c + 1] = trianglesTwoArray[counter, 1];
            triangles[c + 2] = trianglesTwoArray[counter, 2];

            counter++;
        }

        // Polygonの法線を設定
        for (counter = 0; counter < vertexAmount; counter++)
        {
            normals[counter] = new Vector3(0, 0, -1);
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
}