using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;

    private float waypointLineWidth = 0.2f;
    private Color waypointColor = Color.yellow;

    private void Start()
    {
        DrawWaypoints();
    }

    private void DrawWaypoints()
    {
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Utils.DrawLine(waypoints[i].position, waypoints[i + 1].position, waypointLineWidth, waypointColor);
        }
    }
}
