using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    private CharacterController controller;
    [SerializeField] private Transform eyes;
    private float eyesPosY;
    private float eyesPosCrouch, eyesPosStand;
    [SerializeField] private PlayerLook PlayerLook;
    [SerializeField] private GameObject GM;
    [SerializeField] private Inventory inventory;

    [Header("UI Parameters")]
    private VisualElement root;
    private VisualElement staminaBar;

    [Header("Movement Parameters")]
    private float moveSpeed;
    private float walkSpeed = 3f;
    private float sprintSpeed = 7f;
    private float crouchSpeed = 1.5f;
    private float jumpForce = 1.5f;
    private float gravity = -9.81f;
    private int standingHeight = 2;
    private int crouchingHeight = 1;
    private bool isPlayerGrounded;
    private bool isPlayerCrouching;
    private Vector3 velocity;
    private Vector3 finalMove;
    private float staminaTimer = 25f;
    private int staminaMultiplier = 5;

    [Header("Other Parameters")]
    private bool isFlareThrown;

    [Header("PhotoCamera Parameters")]
    public GameObject mainCamera;
    public GameObject photoCamera;
    public bool inPhotoMode;
    public Animator cameraAnim;
    private bool isCameraInHand = false;
    [SerializeField] private CameraFlash cameraFlash;
    [SerializeField] private AudioSource cameraShutterSound;
    [SerializeField] private GameObject flashObject;
    [SerializeField] private GameObject cameraObject;

    #region Input Actions
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference throwFlareAction;
    [SerializeField] private InputActionReference cameraAction;
    [SerializeField] private InputActionReference photoAction;
    [SerializeField] private InputActionReference getCameraAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();
        jumpAction.action.Enable();
        throwFlareAction.action.Enable();
        getCameraAction.action.Enable();

        photoAction.action.performed += OnPhotoTaken;
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();
        jumpAction.action.Disable();
        throwFlareAction.action.Disable();
        getCameraAction.action.Disable();

        photoAction.action.performed -= OnPhotoTaken;
    }
    #endregion

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        eyesPosY = eyes.position.y;
        eyesPosCrouch = eyesPosY - 0.3f;
        eyesPosStand = eyesPosCrouch + 0.3f;
        inventory = gameObject.GetComponent<Inventory>();
        StartCoroutine(FlareCountdown());
        
        root = mainCamera.GetComponent<UIDocument>().rootVisualElement;
        staminaBar = root.Q("StaminaBar");
    }

    private void Start()
    {
        mainCamera.SetActive(true);
        photoCamera.SetActive(false);
        flashObject.SetActive(false);
        cameraObject.SetActive(false);
    }

    private void Update()
    {
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        MoveUpdate();
        ActionUpdate();

        CameraActionUpdate();
    }

    private void MoveUpdate()
    {
        // Jump
        isPlayerGrounded = controller.isGrounded;
        if (isPlayerGrounded && velocity.y < 0f)
            { velocity.y = -2f; }

        if (jumpAction.action.WasPressedThisFrame() && isPlayerGrounded) 
        { velocity.y = Mathf.Sqrt(jumpForce * -gravity); }

        // Move direction
        Vector3 eyesForward = eyes.forward;
        Vector3 eyesRight = eyes.right;
        eyesForward.y = 0f;
        
        // Movement
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = eyesRight * input.x + eyesForward * input.y;
        move.Normalize(); // S�rger for at spilleren bev�ger sig med samme hastighed uanset hvor spilleren kigger hen
        if (move != Vector3.zero) { transform.position = move; }
        
        
        // Crouch
        if (crouchAction.action.IsPressed() && isPlayerGrounded)
        {
            isPlayerCrouching = true;
            controller.height = crouchingHeight;
            moveSpeed = crouchSpeed;
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
        if (isPlayerGrounded && !isPlayerCrouching)
        {
            // Sprint
            if (sprintAction.action.IsPressed() && move != Vector3.zero)
            {
                staminaTimer -= staminaMultiplier * Time.deltaTime; // Make stamina decrease when running
                if (staminaTimer <= 0) { staminaTimer = 0; moveSpeed = walkSpeed; } // If stamina is EMPTY stop removing stamina and stop running
                else moveSpeed = sprintSpeed; // Only sprint if player has stamina
            }
            else
            {
                moveSpeed = walkSpeed;
                staminaTimer += staminaMultiplier * 0.7f * Time.deltaTime; // Make stamina increase when not running
                if (staminaTimer >= 25f) { staminaTimer = 25f; } // If stamina is FULL stop adding more stamina
            }
        
            // Stamina UI
            if (staminaTimer < 25f)
            {
                staminaBar.style.display = DisplayStyle.Flex; // Make stamina bar visible
                staminaBar.style.scale = new Vector3(staminaTimer, 0.01f, 0); // Update the stamina bar
            }
            else { staminaBar.style.display = DisplayStyle.None; } // Make stamina bar invisible if stamina is FULL
        }
        else {  }



        // Make MoveUpdate() work
        finalMove = move * moveSpeed + velocity.y * Vector3.up;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void ActionUpdate()
    {
        if(throwFlareAction.action.WasPerformedThisFrame() && inventory.flareCount != 0 && !isFlareThrown && !isCameraInHand)
        {
            StartCoroutine(FlareCountdown());

            // Setting random rotation for the flare
            int rndX = UnityEngine.Random.Range(-180, 180);
            int rndY = UnityEngine.Random.Range(-180, 180);
            int rndZ = UnityEngine.Random.Range(-180, 180);

            // Instantiate flare and add forward force
            GameObject flare = Instantiate(GM.GetComponent<GameManager>().flarePrefab, eyes.position + eyes.forward, Quaternion.Euler(rndX, rndY, rndZ));
            flare.GetComponent<Rigidbody>().AddForce(eyes.forward * 10f, ForceMode.VelocityChange);

            inventory.RemoveItem("Flare"); // Remove flare from inventory
        }

        if(getCameraAction.action.WasPressedThisFrame())
        {
            if(!PlayerLook.hasItemInHand)
            {
                isCameraInHand = true;
                cameraObject.SetActive(true);
                flashObject.SetActive(true);
            }
            else
            {
                isCameraInHand = false;
                cameraObject.SetActive(false);
                flashObject.SetActive(false);
            }
            PlayerLook.hasItemInHand = !PlayerLook.hasItemInHand; // Toggle camera in hand state
        }
    }

    IEnumerator FlareCountdown()
    {
        isFlareThrown = true;
        float seconds = 2f;
        float barSize = 0.4f;
        inventory.flareCooldown.style.scale = new Vector3(0.5f, barSize, 1f); // flare cooldown UI bar

        for (int i = 0; i < seconds * 2; i++)
        {
            yield return new WaitForSeconds(0.5f);
            barSize -= 0.1f;
            inventory.flareCooldown.style.scale = new Vector3(0.5f, barSize, 1f); // flare cooldown UI bar is shrinking
        }

        isFlareThrown = false;
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        { hit.rigidbody.AddRelativeForce(finalMove, ForceMode.Force); }
    }

    void CameraActionUpdate()
    {
        if (isCameraInHand)
        { 
            if (cameraAction.action.IsPressed() && !inPhotoMode)
            {
                StartCoroutine(EnterPhotoMode());
            }

            if(!cameraAction.action.IsPressed() && inPhotoMode && !cameraFlash.takingPicture)
            {
                StartCoroutine(ExitPhotoMode());
            }
        }
       
    }

    IEnumerator EnterPhotoMode()
    {
        cameraAnim.SetBool("isAiming", true);

        yield return new WaitForSeconds(0.3f); //wait for animation to finish

        cameraObject.GetComponent<MeshRenderer>().enabled = false;
        

        //hold camera up to eyes - switch to "camera"-mode
        mainCamera.SetActive(false);
        photoCamera.SetActive(true);

        cameraFlash.ReadyFlash();

        inPhotoMode = true;
    }

    IEnumerator ExitPhotoMode()
    {
        //hold camera up to eyes - switch to "camera"-mode
        mainCamera.SetActive(true);
        photoCamera.SetActive(false);
       
        cameraObject.GetComponent<MeshRenderer>().enabled = true;
        inventory.UpdatePlayerHud();

        

        inPhotoMode = false;
        yield return new WaitForSeconds(0.1f); //wait
        cameraAnim.SetBool("isAiming", false);
    }

    void OnPhotoTaken(InputAction.CallbackContext context)
    {
        if (inPhotoMode && !cameraFlash.takingPicture)
        {
            cameraFlash.Flash();
            cameraShutterSound.Play();

        }
    }

}
