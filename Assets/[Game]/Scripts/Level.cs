using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform laddersContainer;

    [Space]
    [Header("Level Colors")]
    public Color backgroundColor;
    public Color fogColor;

    [Space]
    [SerializeField] private Material fogMaterial;

    [HideInInspector] public Finish finish;

    private void Start()
    {
        finish = GetComponentInChildren<Finish>();

        //Delete Later
        LevelManager.instance.level = this;

        SetLevelColors();
    }

    private void SetLevelColors()
    {
        CameraManager.instance.cam.backgroundColor = backgroundColor;
        fogMaterial.color = fogColor;
    }
}
