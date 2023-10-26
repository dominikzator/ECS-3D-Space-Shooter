using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI progressText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetProgressText(float percentValue) //percent as float value from 0 to 1
    {
        Debug.Log("SetProgressText");
        progressText.text = $"Progress: {percentValue * 100f}%";
    }
}
