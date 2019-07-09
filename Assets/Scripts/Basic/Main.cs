using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basic
{
    public class Main : MonoBehaviour
    {
        public static Polygon polygon;
        // Start is called before the first frame update
        void Start()
        {
            polygon = new Polygon(Shader.Find("Diffuse"));
        }

        // Update is called once per frame
        void Update()
        {
            if (!polygon.IsLoadComplete) return;

            Material material = polygon.Material;
            Mesh mesh = polygon.MeshData;
            Vector3[] vector3 = mesh.vertices;
            for (int i = 0; i < 3; i++)
            {
                GameObject sphere = GameObject.Find("Vertex" + i);
                vector3[i] = sphere.transform.position;
            }
            polygon.UpdateVertexPosition(vector3);

            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
        }
    }
}