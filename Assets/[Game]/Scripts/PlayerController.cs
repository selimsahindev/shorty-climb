using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Transform carryingPosition;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    [HideInInspector] public bool move = false;
    [HideInInspector] public Stack<Timber> timbers = new Stack<Timber>();

    private bool isJumping = false;
    private Animator animator;
    private GameObject ladderPrefab;
    private List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();
    private List<Collider> ragdollColliders = new List<Collider>();

    private void Start()
    {
        animator = GetComponent<Animator>();
        ladderPrefab = Resources.Load<GameObject>("Ladder");

        ragdollRigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        ragdollColliders.AddRange(GetComponentsInChildren<Collider>());

        DisableRagdoll();

        LevelManager.instance.startEvent.AddListener(OnGameStart);
        LevelManager.instance.endGameEvent.AddListener(OnGameOver);
    }

    private void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate()
    {
        if (!LevelManager.instance.isGameActive)
        {
            return;
        }

        if (!IsOnGround() && !isJumping)
        {
            if (timbers.Count > 0)
            {
                PlaceLadder();
            }
            else
            {
                Jump();
            }
        }
    }

    private void OnGameStart()
    {
        move = true;
        animator.SetTrigger("Run");
    }

    private void OnGameOver(bool success)
    {
        if (success)
        {
            // Win
        }
        else
        {
            move = false;
            ActivateRagdoll();
        }
    }

    private void HandleMovement()
    {
        if (!move) return;

        transform.Rotate(Vector3.up, InputManager.instance.delta.x * rotationSpeed * Time.deltaTime);

        Vector3 movePos = transform.position + transform.forward * moveSpeed;
        transform.position = Vector3.Lerp(transform.position, movePos, Time.deltaTime);
    }

    private void Jump()
    {
        if (isJumping)
        {
            return;
        }

        isJumping = true;

        animator.SetTrigger("Jump");

        StartCoroutine(Delay(0.75f, () => {
            isJumping = false;

            if (!IsOnGround())
            {
                LevelManager.instance.Fail();
            }
        }));
    }

    private void PlaceLadder()
    {
        if (timbers.Count <= 0)
        {
            return;
        }

        Timber timber = timbers.Pop();
        timber.transform.DOScale(0f, 0.15f).OnComplete(() => Destroy(timber.gameObject));

        GameObject ladder = Instantiate(ladderPrefab);
        ladder.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        ladder.transform.rotation = transform.rotation;

        Transform ladderChild = ladder.transform.GetChild(0);
        ladderChild.localScale = Vector3.zero;
        ladderChild.DOScale(1f, 0.05f);

        if (timbers.Count == 0)
        {
            animator.SetTrigger("Run");
        }
    }

    public bool IsOnGround()    
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);

        Debug.DrawRay(transform.position + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.transform.CompareTag("Platform") || hit.transform.CompareTag("Ladder"))
            {
                return true;
            }
        }

        return false;
    }

    private void Pick(Transform timber)
    {
        if (timbers.Count == 0)
        {
            animator.SetTrigger("Carry");
        }

        timber.GetComponent<Collider>().enabled = false;
        timber.SetParent(carryingPosition);
        timber.DOLocalMove(new Vector3(0f, timbers.Count * 0.1f, 0f), 0.15f);

        timbers.Push(timber.GetComponent<Timber>());
    }

    private void ActivateRagdoll()
    {
        animator.enabled = false;

        foreach (Rigidbody limbRigidbody in ragdollRigidbodies)
        {
            limbRigidbody.isKinematic = false;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject != this.gameObject)
            {
                col.enabled = true;
            }
        }
    }

    private void DisableRagdoll()
    {
        foreach (Rigidbody limbRigidbody in ragdollRigidbodies)
        {
            limbRigidbody.isKinematic = true;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject != this.gameObject)
            {
                col.enabled = false;
            }
        }
    }

    private IEnumerator Delay(float time, UnityAction onComplete)
    {
        yield return new WaitForSeconds(time);
        onComplete?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Timber"))
        {
            Pick(other.transform);
        }
    }
}
