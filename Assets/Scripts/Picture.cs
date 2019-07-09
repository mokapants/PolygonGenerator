using System.IO;
using UnityEngine;

public class Picture
{
    private Texture2D pictureTex2D;
    public Texture2D PictureTex2D { get { return this.pictureTex2D; } }

    private int width;
    private int height;
    public int Width { get { return this.width; } }
    public int Height { get { return this.height; } }

    /*
    @summary
    画像を読み取るクラス

    @args
    1:色のついた画像のPath
    2:モノクロ画像のPath
    ※PathはAssets/Resources内限定
    */
    public Picture(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        // 画像データの読み込み，colorはとりあえず左目の情報
        pictureTex2D = ReadPng(fileStream);

        // どちらかの画像が欠けていたら実行しない
        if (pictureTex2D != null)
        {
            width = pictureTex2D.width;
            height = pictureTex2D.height;
        }
    }

    public static Texture2D ReadPng(FileStream fileStream)
    {
        byte[] readBinary;
        using(BinaryReader bin = new BinaryReader(fileStream))
        {
            readBinary = bin.ReadBytes((int) bin.BaseStream.Length);
        }

        // 画像が読み込めていなかったらnullを返す
        if (readBinary == null) return null;

        int pos = 16; // 16バイトから開始

        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + readBinary[pos++];
        }

        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + readBinary[pos++];
        }

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(readBinary);

        return texture;
    }

    // モノクロ画像のGrayscale取得
    public float[] GetGrayscale()
    {
        float[] grayscale = new float[width * height];
        Color[] pixels = pictureTex2D.GetPixels();

        int counter = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grayscale[counter] = pixels[counter].grayscale;
                counter++;
            }
        }

        return grayscale;
    }
}