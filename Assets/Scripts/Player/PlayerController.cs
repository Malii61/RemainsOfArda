using Cinemachine;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public static PlayerController Instance { get; private set; }

    public event EventHandler OnPlayerDie;
    public event EventHandler OnPlayerSpawned;

    [SerializeField] private Camera cam;
    [SerializeField] private CinemachineVirtualCamera fpsCam;
    [SerializeField] private CinemachineVirtualCamera tpsCam;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private GameObject CinemachineCameraTarget;
    [SerializeField] internal float sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] private Animator animator;
    private EnergyBar energyBar;
    public float mouseSensitivity;
    private bool grounded;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    internal Rigidbody rb;
    private PhotonView PV;
    [SerializeField] internal GameObject player;
    //interactions
    private bool interacted;
    private I_Interactable lastInteracted;
    private bool facedAlready;

    private Vector3 stopPl = Vector3.zero;
    //tps cam rotation
    private float _cinemachineTargetYaw = 0;
    private float _cinemachineTargetPitch = 0;
    private float TopClamp = 70.0f;
    private float BottomClamp = -30.0f;
    private float CameraAngleOverride = 0.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        if (PV.IsMine && Instance == null)
            Instance = this;
        //PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            //ChangeMesh(pickedMesh);
            energyBar = EnergyBar.Instance;
            gameObject.AddComponent(Type.GetType("Role_" + ChooseRole.pickedRole));
            animator.Play("Idle");
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            OnPlayerSpawned?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Destroy(cam.gameObject);
            Destroy(fpsCam.gameObject);
            Destroy(tpsCam.gameObject);
            Destroy(rb);
        }
    }
    void Update()
    {
        if (!PV.IsMine || Job.IsJobCanvasActive || DiarySystem.Instance.IsDiaryPanelActive())
        {
            if (PV.IsMine)
            {
                moveAmount = new Vector3(0, 0, 0);
                energyBar.ChangeEnergy(energy.hideAndIncrease);
                if (Job.IsJobCanvasActive)
                    cinemachineBrain.enabled = false;
            }
            return;
        }
        else
            cinemachineBrain.enabled = true;

        CamCheck();
        Move();
        Jump();
        HandleInteractions();
    }
    private void OnDestroy()
    {
        OnPlayerDie?.Invoke(this, EventArgs.Empty);
    }
    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public Animator GetAnimator()
    {
        return animator;
    }
    public Camera GetCamera()
    {
        return cam;
    }
    public Transform GetPlayer()
    {
        return player.transform;
    }
    private void HandleInteractions()
    {
        Ray ray = cam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        float interactDistance = fpsCam.isActiveAndEnabled ? 5 : 10;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, interactDistance))
        {
            if (raycastHit.transform.TryGetComponent(out I_Interactable interactableObj))
            {
                //interactable object has found
                interacted = true;
                lastInteracted = interactableObj;
                if (!facedAlready)
                {
                    interactableObj.OnFaced();
                    facedAlready = true;
                }
                if (GameInput.Instance.GetInputActions().Interactions.Interact.triggered)
                {
                    interactableObj.Interact();
                }
            }
            else
            {
                if (interacted && lastInteracted != null)
                {
                    lastInteracted.OnInteractEnded();
                    interacted = false;
                    lastInteracted = null;
                }
                facedAlready = false;
            }
        }
        else
        {
            if (interacted && lastInteracted != null)
            {
                lastInteracted.OnInteractEnded();
                interacted = false;
                lastInteracted = null;
            }
            facedAlready = false;
        }

    }

    private void CamCheck()
    {
        if (EventSystem.current.currentSelectedGameObject)
            return;
        if (TimeStage.Instance.currentStage == Stage.action || TimeStage.Instance.currentStage == Stage.ready)
        {
            fpsCam.enabled = true;
            tpsCam.enabled = false;
        }
        else if (GameInput.Instance.GetInputActions().Interactions.ChangeCam.triggered)
        {
            fpsCam.enabled = !fpsCam.isActiveAndEnabled;
            tpsCam.enabled = !tpsCam.isActiveAndEnabled;
        }

    }

    void Move()
    {
        if (EventSystem.current.currentSelectedGameObject)
        {
            moveAmount = stopPl;
            return;
        }
        Vector2 movement = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(movement.x, 0, movement.y);
        moveDir = cam.transform.forward * moveDir.z + cam.transform.right * moveDir.x;
        moveDir.y = 0f;
        if (GameInput.Instance.GetInputActions().Player.Sprint.IsPressed() && !energyBar.playerTired)
        {
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * sprintSpeed, ref smoothMoveVelocity, smoothTime);
            if (moveDir == Vector3.zero)
            {
                energyBar.ChangeEnergy(energy.increase);
                animator?.SetBool("isRunning", false);
                animator?.SetBool("isWalking", false);
            }
            else
            {
                energyBar.ChangeEnergy(energy.decrease);
                animator?.SetBool("isRunning", true);
            }
        }
        else
        {
            moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * walkSpeed, ref smoothMoveVelocity, smoothTime);
            energyBar.ChangeEnergy(energy.increase);
            if (moveDir == Vector3.zero)
                animator?.SetBool("isWalking", false);
            else
                animator?.SetBool("isWalking", true);

            animator?.SetBool("isRunning", false);
        }

        //Rotation
        if (tpsCam.isActiveAndEnabled)
        {
            if (movement != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, rotation, Time.deltaTime * 4f);
            }
        }
        else if (fpsCam.isActiveAndEnabled)
        {
            Quaternion rotation = Quaternion.Euler(0f, cinemachineBrain.transform.eulerAngles.y, 0f);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, rotation, Time.deltaTime * 4f);
        }
        CameraRotation();


        if (transform.position.y < -10f) // Die if you fall out of the world
        {
            Die();
        }
    }

    private void CameraRotation()
    {
        _cinemachineTargetYaw += GameInput.Instance.GetMouseLook().x * mouseSensitivity;
        _cinemachineTargetPitch += -GameInput.Instance.GetMouseLook().y * mouseSensitivity;
        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, Time.deltaTime * 4f);

    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    void Jump()
    {
        if (grounded && GameInput.Instance.GetInputActions().Player.Jump.triggered)
        {
            if (EventSystem.current.currentSelectedGameObject)
            {
                animator?.SetBool("isJumping", false);
                return;
            }
            animator?.SetBool("isJumping", true);
        }
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    void Die()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "alive", false } });
    }
    public AudioSource GetAudioSource()
    {
        return GetComponent<AudioSource>();
    }
}