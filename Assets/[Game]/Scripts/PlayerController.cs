using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform carryingPosition;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    [HideInInspector] public bool move = false;
    [HideInInspector] public Queue<Timber> timbers = new Queue<Timber>();

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

        transform.Rotate(Vector3.up, InputManager.instance.delta.x * rotationSpeed * Time.deltaTime);

        Vector3 movePos = transform.position + transform.forward * moveSpeed;
        transform.position = Vector3.Lerp(transform.position, movePos, Time.deltaTime);
    }

    private void Pick(Transform timber)
    {
        if (timbers.Count == 0)
        {
            animator.SetTrigger("Carry");
        }

        timber.SetParent(carryingPosition);
        timber.DOLocalMove(new Vector3(0f, timbers.Count * 0.1f, 0f), 0.15f);

        timbers.Enqueue(timber.GetComponent<Timber>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Timber"))
        {
            Pick(other.transform);
        }
    }
}
