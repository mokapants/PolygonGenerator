using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace PictureGenerate
{
    public class GUI : MonoBehaviour
    {
        string path;

        public void Generate()
        {
            path = Application.dataPath + "/Pictures/Neko.png";
            if (!File.Exists(path)) return;

            Main.polygon = new Polygon(path, Shader.Find("Diffuse"));
        }

        public void Grayscale()
        {
            Main.polygon.SetGrayscale();
        }
    }
}