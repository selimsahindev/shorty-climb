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
    [HideInInspector] public PlayerController player;

    private void Start()
    {
        finish = GetComponentInChildren<Finish>();
        player = GetComponentInChildren<PlayerController>();

        Construct();
    }

    private void Construct()
    {
        SetLevelColors();

        CameraManager.instance.SetTarget(player.transform, player.transform);

        LevelManager.instance.onLevelLoaded?.Invoke();
    }

    private void SetLevelColors()
    {
        CameraManager.instance.cam.backgroundColor = backgroundColor;
        fogMaterial.color = fogColor;
    }
}
