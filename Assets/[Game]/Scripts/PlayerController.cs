using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [HideInInspector] public bool move = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        LevelManager.instance.startEvent.AddListener(OnGameStart);
    }

    private void Update()
    {
        HandleMovement();
    }

    private void OnGameStart()
    {
        move = true;
        animator.SetTrigger("Run");
    }

    private void HandleMovement()
    {
        if (!move) return;

        Vector3 movePos = transform.position + transform.forward * moveSpeed;
        transform.position = Vector3.Lerp(transform.position, movePos, Time.deltaTime);
    }
}
