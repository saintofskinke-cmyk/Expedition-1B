using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    private CharacterController controller;
    [SerializeField] private Transform eyes;
    private float eyesPosY;
    private float eyesPosCrouch, eyesPosStand;
    [SerializeField] private PlayerLook PlayerLook;

    [Header("Movement Parameters")]
    private float moveSpeed;
    private float walkSpeed = 7f;
    private float sprintSpeed = 12f;
    private float crouchSpeed = 3.5f;
    private float jumpForce = 1.5f;
    private float gravity = -9.81f;
    private int standingHeight = 2;
    private int crouchingHeight = 1;
    private bool isPlayerGrounded;
    private bool isPlayerCrouching;
    private Vector3 velocity;

    [Header("PhotoCamera Parameters")]
    public GameObject mainCamera;
    public GameObject photoCamera;
    public bool inPhotoMode;
    public Animator cameraAnim;
    bool isTransitioning = false;
    [SerializeField] private CameraFlash cameraFlash;
    [SerializeField] private AudioSource cameraShutterSound;

    #region Input Actions
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference cameraAction;
    [SerializeField] private InputActionReference photoAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        eyesPosY = eyes.position.y;
        eyesPosCrouch = eyesPosY - 0.3f;
        eyesPosStand = eyesPosCrouch + 0.3f;

    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();
        jumpAction.action.Enable();

        photoAction.action.performed += OnPhotoTaken;
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();
        jumpAction.action.Disable();

        photoAction.action.performed -= OnPhotoTaken;
    }
    #endregion

    private void Start()
    {
        mainCamera.SetActive(true);
        photoCamera.SetActive(false);
    }

    private void Update()
    {
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        MoveUpdate();

        CameraActionUpdate();
    }

    private void MoveUpdate()
    {
        // Jump
        isPlayerGrounded = controller.isGrounded;
        if (isPlayerGrounded && velocity.y < 0f)
            { velocity.y = -2f; }

        if (jumpAction.action.triggered && isPlayerGrounded)
            { velocity.y = Mathf.Sqrt(jumpForce * -gravity); }


        // Move direction
        Vector3 eyesForward = eyes.forward;
        Vector3 eyesRight = eyes.right;
        eyesForward.y = 0f;

        // Movement
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = eyesRight * input.x + eyesForward * input.y;
        move.Normalize();
        if (move != Vector3.zero) { transform.position = move; }


        // Crouch
        if (crouchAction.action.IsPressed() && isPlayerGrounded)
        {
            isPlayerCrouching = true;
            controller.height = crouchingHeight;
        }
        else if (isPlayerCrouching && !crouchAction.action.IsPressed())
        {
            if (Physics.Raycast(eyes.position, Vector3.up, 0.5f))
            {
                isPlayerCrouching = true;
                controller.height = crouchingHeight;
            }
            else
            {
                isPlayerCrouching = false;
                controller.height = standingHeight;
            }
            Debug.Log(Physics.Raycast(eyes.position, Vector3.up, 0.5f));
        }

        // Apply moveSpeed
        if (isPlayerGrounded)
        {
            if (sprintAction.action.IsPressed()) { moveSpeed = sprintSpeed; }
            else if (isPlayerCrouching) { moveSpeed = crouchSpeed; }
            else { moveSpeed = walkSpeed; }
        }

        // Make MoveUpdate() work
        Vector3 finalMove = move * moveSpeed + velocity.y * Vector3.up;
        controller.Move(finalMove * Time.deltaTime);
    }


    void CameraActionUpdate()
    {
        if (PlayerLook.hasItemInHand && PlayerLook.itemInHand.CompareTag("Camera"))
        {
            if(cameraAction.action.IsPressed() && !inPhotoMode && !isTransitioning)
            {
                StartCoroutine(EnterPhotoMode());
            }

            if(!cameraAction.action.IsPressed() && inPhotoMode && !isTransitioning)
            {
                StartCoroutine(ExitPhotoMode());
            }
        }
    }

    IEnumerator EnterPhotoMode()
    {
        isTransitioning = true;
        cameraAnim.SetBool("isAiming", true);

        yield return new WaitForSeconds(0.25f); //wait for animation to finish


        if (PlayerLook.cameraInHand != null)
        {
            PlayerLook.cameraInHand.GetComponent<MeshRenderer>().enabled = false;
        }

        //hold camera up to eyes - switch to "camera"-mode
        mainCamera.SetActive(false);
        photoCamera.SetActive(true);

        cameraFlash.ReadyFlash();

        inPhotoMode = true;
        isTransitioning = false;
    }

    IEnumerator ExitPhotoMode()
    {
        isTransitioning = true;

        yield return new WaitForSeconds(0.25f); //wait

        //hold camera up to eyes - switch to "camera"-mode
        mainCamera.SetActive(true);
        photoCamera.SetActive(false);

        if (PlayerLook.cameraInHand != null)
        {
            PlayerLook.cameraInHand.GetComponent<MeshRenderer>().enabled = true;
        }

        cameraAnim.SetBool("isAiming", false);

        inPhotoMode = false;
        isTransitioning = false;
    }

    void OnPhotoTaken(InputAction.CallbackContext context)
    {
        if (inPhotoMode && !isTransitioning)
        {
            cameraFlash.Flash();
            cameraShutterSound.Play();

        }
    }

}
