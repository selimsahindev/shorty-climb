using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Level Colors")]
    public Color backgroundColor;
    public Color fogColor;

    [Space]
    [SerializeField] private Material fogMaterial;

    private void Start()
    {
        SetLevelColors();
    }

    private void SetLevelColors()
    {
        CameraManager.instance.cam.backgroundColor = backgroundColor;
        fogMaterial.color = fogColor;
    }
}
