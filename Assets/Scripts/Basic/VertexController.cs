using System.Collections;
using UnityEngine;

namespace Basic
{
    public class VertexController : MonoBehaviour
    {
        Vector3 screenPoint;

        void Update()
        {
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 screenToWorld = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            transform.position = Camera.main.ScreenToWorldPoint(screenToWorld);
        }
    }
}