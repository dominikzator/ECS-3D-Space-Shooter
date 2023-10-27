using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(GlobalValuesScriptableObject),menuName = nameof(GlobalValuesScriptableObject))]
public class GlobalValuesScriptableObject : ScriptableObject
{
    [SerializeField] private float shootingIntervalInSeconds;
    
    public float ShootingIntervalInSeconds => shootingIntervalInSeconds;
}
