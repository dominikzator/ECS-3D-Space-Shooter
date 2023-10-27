using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;
    
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float minRandomVelocity;
    [SerializeField] private float maxRandomVelocity;

    public List<Transform> Waypoints => waypoints;
    
    [HideInInspector] public List<float3> Velocities = new List<float3>();
    [HideInInspector] public List<float> SegmentTimes = new List<float>();
    
    private float waypointLineWidth = 0.2f;
    private Color waypointColor = Color.cyan;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < Waypoints.Count - 1; i++)
        {
            var segment = (waypoints[i + 1].transform.position - waypoints[i].transform.position);
            var velocity = segment.normalized * Random.Range(minRandomVelocity, maxRandomVelocity);
            Velocities.Add(velocity);
            SegmentTimes.Add(segment.magnitude/velocity.magnitude);
        }
    }

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
