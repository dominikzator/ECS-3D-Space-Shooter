using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void DrawLine(Vector3 startPos, Vector3 endPos, float width, Color colour)
    {
        GameObject line = new GameObject("Line_ " + startPos.ToString() + "_" + endPos.ToString());
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = colour;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(startPos.x, startPos.y, startPos.z));
        lineRenderer.SetPosition(1, new Vector3(endPos.x, endPos.y, endPos.z));
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }
}
