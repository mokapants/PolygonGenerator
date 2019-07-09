using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PictureGenerate
{
    public class Main : MonoBehaviour
    {
        public static Polygon polygon;
        // Start is called before the first frame update
        void Start()
        {
            polygon = new Polygon();
        }

        // Update is called once per frame
        void Update()
        {
            if (!polygon.IsLoadComplete) return;

            Material material = polygon.Material;
            Mesh mesh = polygon.MeshData;
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
        }
    }
}