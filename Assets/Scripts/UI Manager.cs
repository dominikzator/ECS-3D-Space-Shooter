using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI asteroidsHitText;
    [SerializeField] private TextMeshProUGUI missesText;

    public bool NeedsUpdate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (!NeedsUpdate)
        {
            return;
        }
        
        SetAsteroidsHitText(World.Instance.AsteroidsHit);
        SetMissesText(World.Instance.Misses);
        NeedsUpdate = false;
    }


    public void SetProgressText(float percentValue)
    {
        progressText.text = $"Progress: {percentValue}%";
    }
    public void SetAsteroidsHitText(int asteroidsHit)
    {
        asteroidsHitText.text = $"Asteroids Hit: {asteroidsHit}";
    }

    public void SetMissesText(int misses)
    {
        missesText.text = $"Misses: {misses}";
    }
}
