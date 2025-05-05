using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class OrthographicCharacterController : MonoBehaviour
{
    [System.Serializable]
    public class TagAudio
    {
        [Tooltip("Exact tag name to match (e.g. \"Grass\", \"Wood\", etc.)")]
        public string tag;
        [Tooltip("One or more AudioClips to choose from when walking on this tag.")]
        public AudioClip[] clips;
    }

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravity = -20f;
    public Transform playerModel;

    [Header("Air Control")]
    [Range(0f, 1f)] public float airDirectionInfluence = 0.2f;
    public float airControlLerpSpeed = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Animation")]
    public Animator animator;
    public AnimationController animController;

    [Header("Push Settings")]
    public float pushForce = 3f;

    [Header("Shooting Settings")]
    public ShootingController shootingController;
    public ItemSelector items;
    public ItemSelector ghostItems;

    [Header("Camera Reference")]
    [Tooltip("Assign the camera whose rotation defines forward movement.")]
    public Camera mainCamera;

    [Header("Footstep Settings")]
    [Tooltip("List of footstep sounds per ground tag.")]
    public List<TagAudio> tagFootstepSounds = new List<TagAudio>();
    [Tooltip("Fallback clips if no tag matches.")]
    public AudioClip[] defaultFootstepSounds;
    [Tooltip("Distance (in world units) traveled between steps.")]
    public float stepDistance = 2f;
    [Tooltip("Which layers count as ground for the footstep raycast.")]
    public LayerMask groundLayers = ~0;

    // --- private state ---
    private CharacterController controller;
    private AudioSource audioSource;

    private Vector3 inputDirection;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded;
    private Vector3 jumpDirection;
    private Vector3 currentHorizontalMove;

    private bool isShootingMode;
    private bool wasShootingMode;

    // Footstep state
    private float stepCycle;
    private bool wasWalkingLastFrame;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        animController = FindObjectOfType<AnimationController>();

        wasGrounded = controller.isGrounded;
        wasShootingMode = false;

        stepCycle = 0f;
        wasWalkingLastFrame = false;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // Toggle shooting mode
        if (shootingController != null && shootingController.canShoot)
        {
            isShootingMode = Input.GetMouseButton(0);
            if (isShootingMode && !wasShootingMode)
                shootingController.StartShooting();
            if (!isShootingMode && wasShootingMode)
                shootingController.StopShooting();
            wasShootingMode = isShootingMode;
        }

        HandleInput();
        HandleMovement();
        RotateModel();
        UpdateAnimations();
        HandleFootsteps();
    }

    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 rawInput = new Vector3(h, 0f, v).normalized;

        if (mainCamera != null)
        {
            Vector3 camForward = mainCamera.transform.forward;
            camForward.y = 0f; camForward.Normalize();
            Vector3 camRight = mainCamera.transform.right;
            camRight.y = 0f; camRight.Normalize();
            inputDirection = (camRight * h + camForward * v).normalized;
        }
        else
        {
            inputDirection = rawInput;
        }
    }

    void HandleMovement()
    {
        bool prevGrounded = wasGrounded;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
            velocity.y = -1f;

        // Block movement while shooting
        if (isShootingMode)
        {
            animController.PlayAnimation("isShooting");
            velocity.y += gravity * Time.deltaTime;
            controller.Move(Vector3.up * velocity.y * Time.deltaTime);
            return;
        }
        else
        {
            animController.StopAnimation("isShooting");
        }

        if (isGrounded)
        {
            currentHorizontalMove = inputDirection;

            if (Input.GetKeyDown(KeyCode.Space) && prevGrounded)
            {
                jumpDirection = inputDirection.sqrMagnitude > 0.01f
                    ? inputDirection
                    : Vector3.up;
                currentHorizontalMove = new Vector3(jumpDirection.x, 0f, jumpDirection.z).normalized;
                velocity.y = jumpForce;
            }
        }
        else
        {
            Vector3 targetAirDir = Vector3.Lerp(jumpDirection, inputDirection, airDirectionInfluence).normalized;
            currentHorizontalMove = Vector3.Lerp(currentHorizontalMove, targetAirDir, Time.deltaTime * airControlLerpSpeed);
            velocity.y += gravity * Time.deltaTime;
        }

        Vector3 move = currentHorizontalMove * moveSpeed;
        move.y = velocity.y;
        controller.Move(move * Time.deltaTime);
    }

    void RotateModel()
    {
        if (isShootingMode && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 dir = hitPoint - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                    playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }
        }
        else if (inputDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDirection, Vector3.up);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateAnimations()
    {
        HandleWalkingAnimation();
        HandleJumpingAnimation();
    }

    void HandleJumpingAnimation()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && wasGrounded)
            animController.TriggerAnimation("JumpStart");
        if (!wasGrounded && isGrounded)
            animController.TriggerAnimation("JumpLand");
        animator.SetBool("IsInAir", !isGrounded);
        wasGrounded = isGrounded;
    }

    void HandleWalkingAnimation()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.2f)
            animController.PlayAnimation("IsWalking");
        else
            animController.StopAnimation("IsWalking");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            body.AddForce(pushDir * pushForce, ForceMode.Impulse);
        }

        var button = hit.collider.GetComponent<PhysicsButton3D>();
        if (button != null)
            button.ForcePress();
    }

    // --- Footstep System ---

    private void HandleFootsteps()
    {
        bool isWalkingNow = controller.isGrounded && inputDirection.sqrMagnitude > 0.01f;

        if (isWalkingNow)
        {
            // First step immediately
            if (!wasWalkingLastFrame)
            {
                PlayFootstep();
                stepCycle = 0f;
            }

            // Subsequent steps after distance
            stepCycle += moveSpeed * Time.deltaTime;
            if (stepCycle >= stepDistance)
            {
                PlayFootstep();
                stepCycle -= stepDistance;
            }
        }
        else
        {
            // Reset cycle on stop, but do NOT stop the last clip
            stepCycle = 0f;
        }

        wasWalkingLastFrame = isWalkingNow;
    }

    private void PlayFootstep()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (!Physics.Raycast(origin, Vector3.down, out hit, 1.5f, groundLayers))
            return;

        string groundTag = hit.collider.tag;
        AudioClip[] clips = defaultFootstepSounds;

        foreach (var entry in tagFootstepSounds)
        {
            if (entry.tag == groundTag && entry.clips.Length > 0)
            {
                clips = entry.clips;
                break;
            }
        }

        if (clips.Length == 0)
            return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
