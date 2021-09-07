using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour {
    [HideInInspector] public Camera cam;

    #region Variables
    public CameraUpdateType updateType;

    public Transform positionTarget;
    public Transform rotatitonTarget;

    public bool follow = false;
    public bool rotate = false;
    public bool rotateAllAxis = false;
    public bool rotateWithTarget = false;
    public bool rotateAnimation = false;
    public bool offset = false;

    public float xDistance;
    public float yDistance;
    public float zDistance;

    public float angle;
    public float positionSmoothness;
    public float rotationSmoothness;
    public float fovSmoothness;

    public float shakeMagnitude;
    public float fov = 60f;

    public float rotateAnimationSpeed = 2f;
    public float rotateAnimationMinValue = 40f;
    public float rotateAnimationMaxValue = 120f;

    public Vector3 offsetVector;
    public bool localOffset = true;
    [Tooltip("Additional offset vector based on amount of collected items")]
    public bool additionalOffsetEnabled = true;
    public float maxAdditionalOffsetMagnitude = 3f;

    private Vector3 additionalOffset;
    private Vector3 refVelocity;
    #endregion

    #region Singleton

    public static CameraManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        cam = GetComponentInChildren<Camera>();
    }

    #endregion

    private void Start()
    {
        LevelManager.instance.endGameEvent.AddListener(OnGameOver);
        LevelManager.instance.onLevelLoaded.AddListener(() => {
            LevelManager.instance.level.player.onPicked.AddListener(() => {
                additionalOffset.z -= 0.05f;
                if (additionalOffset.magnitude > maxAdditionalOffsetMagnitude)
                    additionalOffset = additionalOffset.normalized * maxAdditionalOffsetMagnitude;
            });
            LevelManager.instance.level.player.onPlaced.AddListener(() => {
                additionalOffset.z += 0.05f;
                if (additionalOffset.magnitude < 0f)
                    additionalOffset = Vector3.zero;
            });
        });
    }

    private void FixedUpdate()
    {
        if (updateType == CameraUpdateType.Fixed)
        {
            CameraFocus();
        }
    }
    
    private void LateUpdate()
    {
        if (updateType == CameraUpdateType.Late)
        {
            CameraFocus();
        }
    }

    private void Update()
    {
        if (updateType == CameraUpdateType.Normal)
        {
            CameraFocus();
        }
    }

    #region Focus Components

    public void SetTarget(Transform _targetPosition, Transform _targetRotation = null)
    {
        positionTarget = _targetPosition;
        rotatitonTarget = _targetRotation ? _targetRotation : _targetPosition;
    }

    public void SetValues(float _mXDistance = 0f, float _mYDistance = 0f, float _mZDistance = 0f, float _mAngle = 0f)
    {
        xDistance = _mXDistance;
        yDistance = _mYDistance;
        zDistance = _mZDistance;
        angle = _mAngle;
    }

    public void SetSmooths(float positionSmooth = 1f, float rotationSmooth = 5f, float fovSmooth = 2f)
    {
        positionSmoothness = positionSmooth;
        rotationSmoothness = rotationSmooth;
        fovSmoothness = fovSmooth;
    }

    public void SetOffset(Vector3 _offset)
    {
        offset = true;
        offsetVector = _offset;
    }

    public void SetSpecs(bool _follow = true, bool _rotate = true)
    {
        follow = _follow;
        rotate = _rotate;
    }

    public void SetSpecs(bool _rotateAllAxis = false, bool _rotateAnimation = false, bool _offset = false)
    {
        rotateAllAxis = _rotateAllAxis;
        rotateAnimation = _rotateAnimation;
        offset = _offset;
    }

    public void SetSpecs(bool _follow = true, bool _rotate = true, bool _rotateAllAxis = false, bool _rotateAnimation = false, bool _offset = false, bool _rotateWithTarget = false)
    {
        follow = _follow;
        rotate = _rotate;
        rotateAllAxis = _rotateAllAxis;
        rotateAnimation = _rotateAnimation;
        offset = _offset;
        rotateWithTarget = _rotateWithTarget;
    }

    #endregion

    #region CameraPhysics
    public void CameraShake(float duration = 0.25f, float _shakeMagnitude = 0.1f, bool _turbilance = false)
    {
        shakeMagnitude = _shakeMagnitude;
        StartCoroutine(ShakeCoroutine(duration));
    }

    public void RotateAnimation()
    {
        angle += 1 * Time.fixedDeltaTime * rotateAnimationSpeed;
    }

    public void UpdateCameraShake(float _shakeMagnitude = 0.1f, bool _turbilance = false, float turbilanceSpeed = 1f)
    {
        float camerashakeoffsetx = Random.value * _shakeMagnitude * 2 - _shakeMagnitude;
        float camerashakeoffsety = Random.value * _shakeMagnitude * 2 - _shakeMagnitude;
        Vector3 cameraintermadiateposition = transform.position;
        cameraintermadiateposition.x += camerashakeoffsetx;
        cameraintermadiateposition.y += camerashakeoffsety;
        transform.position = cameraintermadiateposition;
    }

    IEnumerator ShakeCoroutine(float _duration)
    {
        float time = Time.time + _duration;

        while (time > Time.time)
        {
            float camerashakeoffsetx = Random.value * shakeMagnitude * 2 - shakeMagnitude;
            float camerashakeoffsety = Random.value * shakeMagnitude * 2 - shakeMagnitude;
            Vector3 cameraintermadiateposition = transform.position;

            cameraintermadiateposition.x += camerashakeoffsetx;
            cameraintermadiateposition.y += camerashakeoffsety;

            transform.position = cameraintermadiateposition;

            yield return null;
        }
    }

    private void CameraFocus()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, Time.fixedDeltaTime * fovSmoothness * 3f);

        if (!positionTarget) return;

        Vector3 worldposition = (Vector3.forward * xDistance) + (Vector3.up * yDistance) + (Vector3.right * zDistance);
        Vector3 rotatedvector = Quaternion.AngleAxis(angle + rotatitonTarget.localEulerAngles.y, Vector3.up) * worldposition;
        Vector3 flattargetposition = positionTarget.position;
        Vector3 finalposition = flattargetposition + rotatedvector;

        if (offset)
        {
            if (localOffset)
            {
                finalposition += positionTarget.right * offsetVector.x;
                finalposition += positionTarget.up * offsetVector.y;
                finalposition += positionTarget.forward * offsetVector.z;
            }
            else
            {
                finalposition += offsetVector;
            }
        }

        if (additionalOffsetEnabled)
        {
            finalposition += positionTarget.right * additionalOffset.x;
            finalposition += positionTarget.up * additionalOffset.y;
            finalposition += positionTarget.forward * additionalOffset.z;
        }

        if (follow) transform.position = Vector3.SmoothDamp(transform.position, finalposition, ref refVelocity, positionSmoothness);
        if (!rotatitonTarget) return;
        if (rotate)
        {
            Vector3 lookPos = rotatitonTarget.position - transform.position;

            if (offset)
            {
                if (localOffset)
                {
                    lookPos += positionTarget.right * offsetVector.x;
                    lookPos += positionTarget.up * offsetVector.y;
                    lookPos += positionTarget.forward * offsetVector.z;
                }
                else
                {
                    lookPos += offsetVector;
                }
            }

            if (!rotateAllAxis) lookPos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSmoothness);
        }

        if (rotateAnimation) {
            RotateAnimation();
        }
    }
    #endregion

    private void OnGameOver(bool success)
    {
        if (success)
        {
            Multiplier multiplier = LevelManager.instance.multiplier;
            Transform newTarget = null;

            if (multiplier != null)
            {
                newTarget = multiplier.transform;
            }
            else
            {
                newTarget = LevelManager.instance.level.finish.transform;
            }

            xDistance = -8.5f;
            yDistance = 5f;
            rotationSmoothness = 3f;
            positionSmoothness = 0.5f;
            SetOffset(new Vector3(0f, 1f, 0f));
            SetTarget(newTarget, newTarget);
        }
        else
        {
            follow = false;
            rotate = false;
            rotateAllAxis = false;
            rotateWithTarget = false;
        }
    }

    private IEnumerator Delay(float _waitTime = 1f, UnityAction onComplete = null)
    {
        yield return new WaitForSeconds(_waitTime);
        onComplete?.Invoke();
    }

    [System.Serializable]
    public enum CameraUpdateType
    {
        Fixed,
        Late,
        Normal,
    }

}