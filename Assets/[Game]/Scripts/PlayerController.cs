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
    [SerializeField] private Transform counterPosition;

    [HideInInspector] public bool move = false;
    [HideInInspector] public Stack<Timber> timbers = new Stack<Timber>();

    private bool isJumping = false;
    private Animator animator;
    private GameObject ladderPrefab;
    private List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();
    private List<Collider> ragdollColliders = new List<Collider>();

    [HideInInspector] public UnityEvent onPicked = new UnityEvent();
    [HideInInspector] public UnityEvent onPlaced = new UnityEvent();

    private void Start()
    {
        animator = GetComponent<Animator>();
        ladderPrefab = Resources.Load<GameObject>("Ladder");

        ragdollRigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        ragdollColliders.AddRange(GetComponentsInChildren<Collider>());

        DisableRagdoll();

        LevelManager.instance.startEvent.AddListener(OnGameStart);
        LevelManager.instance.endGameEvent.AddListener(OnGameOver);
        LevelManager.instance.onFinishReached.AddListener(() => moveSpeed *= 1.5f);
    }

    private void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate()
    {
        if (!LevelManager.instance.isGameActive) { return; }

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
            animator.SetTrigger("Dance");
        }
        else
        {
            ActivateRagdoll();
        }

        move = false;
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

        StartCoroutine(Delay(0.65f, () => {
            isJumping = false;

            if (!IsOnGround())
            {
                if (LevelManager.instance.isFinishReached)
                {
                    JumpToEndPlatform();
                }
                else
                {
                    LevelManager.instance.Fail();
                }
            }
        }));
    }

    private void JumpToEndPlatform()
    {
        Multiplier multiplier = LevelManager.instance.multiplier;
        Vector3 jumpPos;

        float animationDuration = 1f;

        if (multiplier != null)
        {
            jumpPos = multiplier.transform.position;
        }
        else
        {
            jumpPos = LevelManager.instance.level.finish.transform.position;
        }
        jumpPos.y = transform.position.y;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(jumpPos, animationDuration));
        seq.Join(transform.DORotate(new Vector3(0f, 180f, 0f), animationDuration));

        LevelManager.instance.Success();
    }

    private void PlaceLadder()
    {
        if (timbers.Count <= 0)
        {
            return;
        }

        Timber timber = timbers.Pop();
        timber.transform.DOScale(0f, 0.15f).OnComplete(() => Destroy(timber.gameObject));

        GameObject ladder = Instantiate(ladderPrefab, LevelManager.instance.level.laddersContainer);
        ladder.transform.position = new Vector3(transform.position.x, -0.05f, transform.position.z);
        ladder.transform.rotation = transform.rotation;

        Transform ladderChild = ladder.transform.GetChild(0);
        ladderChild.localScale = Vector3.zero;
        ladderChild.DOScale(1f, 0.05f).SetEase(Ease.InOutBounce);

        if (timbers.Count == 0)
        {
            animator.SetTrigger("Run");
        }

        onPlaced?.Invoke();
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

        Sequence seq = DOTween.Sequence();
        
        seq.Append(timber.DOLocalMove(new Vector3(0f, timbers.Count * 0.1f, 0f), 0.15f));
        seq.Join(timber.DOLocalRotate(Vector3.zero, 0.15f));

        timbers.Push(timber.GetComponent<Timber>());

        onPicked?.Invoke();
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
