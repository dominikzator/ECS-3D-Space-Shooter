using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    
    [SerializeField] private Camera camera;
    
    public Camera Camera => camera;

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
}
