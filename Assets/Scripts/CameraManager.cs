using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    
    [SerializeField] private Camera camera;

    private float3 shipPos;
    private float3 shipForwardVec;
    private float3 shipRightVec;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        camera.transform.forward = shipForwardVec;
        camera.transform.position = shipPos;
        camera.transform.position += -(Vector3)shipForwardVec * 10f;
        camera.transform.RotateAround(shipPos, shipRightVec, 45);
    }

    public void AlignCamera(float3 pos, float3 forward, float3 right)
    {
        shipPos = pos;
        shipForwardVec = forward;
        shipRightVec = right;
    }
}
