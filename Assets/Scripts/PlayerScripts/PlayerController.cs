using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using DevData;

public class PlayerController : MonoBehaviour

    

{
    #region Parameters
    [Header("Player Components")]
    private CharacterController controller;
    [SerializeField] private Transform eyes;
    private float eyesPosY;
    private float eyesPosCrouch, eyesPosStand;
    [SerializeField] private PlayerLook PlayerLook;
    private GameObject GM;
    private GameManager gameManager;
    private AudioSource audSovs;
    [SerializeField] private Inventory inventory;
    private PauseMenu PauseMenu;
    private QuestManager questManager;

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
    private float stamina = 25f;
    private int staminaMultiplier = 3;
    private bool isStaminaRecovering;
    private bool isPlayerMoving;

    [Header("Other Parameters")]
    private bool isFlareThrown;
    private Color staminaBarColor;
    VisualElement flareIcon;

    [Header("PhotoCamera Parameters")]
    public GameObject mainCamera;
    public GameObject photoCamera;
    public bool inPhotoMode;
    public Animator cameraAnim;
    public bool isCameraInHand = false;
    [SerializeField] private CameraFlash cameraFlash;
    [SerializeField] private AudioSource cameraAudioSource;
    [SerializeField] private AudioClip cameraFlashPopSound;
    [SerializeField] private GameObject flashObject;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] LayerMask photoLayers;
    #endregion

    #region Input Actions
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference jumpAction;
    public InputActionReference throwFlareAction;
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
        GM = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = GM.GetComponent<GameManager>();
        PauseMenu = GM.GetComponentInChildren<PauseMenu>();

        controller = GetComponent<CharacterController>();
        eyesPosY = eyes.position.y;
        eyesPosCrouch = eyesPosY - 0.3f;
        eyesPosStand = eyesPosCrouch + 0.3f;
        inventory = gameObject.GetComponent<Inventory>();
        StartCoroutine(FlareCountdown());
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();

        audSovs = GetComponent<AudioSource>();
    }

    private void Start()
    {
        mainCamera.SetActive(true);
        photoCamera.SetActive(false);
        flashObject.SetActive(false);
        cameraObject.SetActive(false);
        StartCoroutine(Footsteps()); // Start checking if player is moving. If player is moving Play footstep sounds

    }

    private void Update()
    {
        if (!PauseMenu.isGamePaused)
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;

            MoveUpdate();
            ActionUpdate();

            CameraActionUpdate();
        }
    }

  
    private void MoveUpdate()
    {
        
        {
            
        }
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
        if (move != Vector3.zero) { 
            transform.position = move;
            isPlayerMoving = true;
        }
        else { isPlayerMoving = false; }

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
            if (sprintAction.action.IsPressed() && move != Vector3.zero && !isStaminaRecovering)
            {
                stamina -= staminaMultiplier * Time.deltaTime; // Make stamina decrease when running
                if (stamina <= 0)
                {
                    stamina = 0;
                    moveSpeed = walkSpeed;
                    staminaBarColor = Colors.staminaRed; // Stamina bar is red
                    isStaminaRecovering = true;
                } // If stamina is EMPTY stop removing stamina and stop running
                else
                {
                    moveSpeed = sprintSpeed; // Only sprint if player has stamina
                }
            }
            else
            {
                moveSpeed = walkSpeed;
                stamina += staminaMultiplier * 0.7f * Time.deltaTime; // Make stamina increase when not running
                if (stamina >= 25f) { 
                    stamina = 25f;
                    staminaBarColor = Colors.staminaWhite; // Stamina bar is white
                    isStaminaRecovering = false; } // If stamina is FULL stop adding more stamina
            }
        
            // Stamina UI
            if (stamina < 25f)
            {
                inventory.staminaBar.style.display = DisplayStyle.Flex; // Make stamina bar visible
                inventory.staminaBar.style.scale = new Vector3(stamina, 0.01f, 0); // Update the stamina bar
                inventory.staminaBar.style.backgroundColor = staminaBarColor;
            }
            else { inventory.staminaBar.style.display = DisplayStyle.None; } // Make stamina bar invisible if stamina is FULL
        }
        



        // Make MoveUpdate() work
        finalMove = move * moveSpeed + velocity.y * Vector3.up;
        controller.Move(finalMove * Time.deltaTime);
    }

    IEnumerator Footsteps()
    {
        while (true)
        {
            yield return new WaitUntil(() => isPlayerMoving); // Only play footstep sounds when moving
            yield return new WaitForSeconds(0.1f);

            int i = UnityEngine.Random.Range(0, AudioManager.Instance.footSteps.Length);
            audSovs.PlayOneShot(AudioManager.Instance.footSteps[i]);

            yield return new WaitForSeconds(0.25f);
            if (moveSpeed <= walkSpeed)
            { yield return new WaitForSeconds(0.25f); }
        }
    }


    // Add force to rigidbodies when colliding with them
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        { hit.rigidbody.AddRelativeForce(finalMove, ForceMode.Force); }
    }

    #region ActionUpdate + Flare
    private void ActionUpdate()
    {
        if(throwFlareAction.action.WasPressedThisFrame() && inventory.flareCount != 0 && !isFlareThrown && !isCameraInHand && !PlayerLook.hasItemInHand)
        {
            StartCoroutine(FlareCountdown());

            // Setting random rotation for the flare
            int rndX = UnityEngine.Random.Range(-180, 180);
            int rndY = UnityEngine.Random.Range(-180, 180);
            int rndZ = UnityEngine.Random.Range(-180, 180);

            GameObject flare;
            if (PlayerLook.isHoldingFlare)
            {
                PlayerLook.TryFlareDrop();
            }
            else
            {
                // Instantiate flare and add forward force
                flare = Instantiate(GM.GetComponent<GameManager>().flarePrefab, eyes.position + eyes.forward, Quaternion.Euler(rndX, rndY, rndZ));
                inventory.RemoveItem("Flare"); // Remove flare from inventory
                flare.GetComponent<Rigidbody>().AddForce(eyes.forward * 10f, ForceMode.VelocityChange);
            }
            

        }

        if(getCameraAction.action.WasPressedThisFrame() && !PlayerLook.hasItemInHand && !inPhotoMode && !PlayerLook.isHoldingFlare)
        {
            flareIcon = PlayerLook.playerHudDocument.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Inventory");
            if (!isCameraInHand)
            {
                isCameraInHand = true;
                cameraObject.SetActive(true);
                flashObject.SetActive(true);
                flareIcon.style.display = DisplayStyle.None;
            }
            else
            {
                isCameraInHand = false;
                cameraObject.SetActive(false);
                flashObject.SetActive(false);
                flareIcon.style.display = DisplayStyle.Flex;
            }
        }
    }

    public IEnumerator FlareCountdown()
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
    #endregion

    #region PhotoCamera
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
        questManager.UpdateUI();
        PlayerLook.UpdateTextUI();

        flareIcon = PlayerLook.playerHudDocument.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Inventory");
        flareIcon.style.display = DisplayStyle.None;

        inPhotoMode = false;
        yield return new WaitForSeconds(0.1f); //wait
        cameraAnim.SetBool("isAiming", false);
    }

    void OnPhotoTaken(InputAction.CallbackContext context)
    {
        if (inPhotoMode && !cameraFlash.takingPicture)
        {
            cameraFlash.Flash();
            cameraAudioSource.PlayOneShot(cameraFlashPopSound);

            CheckObjectInPicture();
        }
    }
    #endregion

    void CheckObjectInPicture()
    {
        //checks what the player is taking a picture of, using a raycast, by drawing a vector from the center of the viewport (0.5, 0.5)
        Ray ray = photoCamera.GetComponentInChildren<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        //draws the raycast from start to finish - for debugging
        Debug.DrawRay(ray.origin, ray.direction * 25f, Color.red, 5f);

        //using previous ray and hit (output) with a distance of 10f, layermask ~0 (everything), ignore trigger colliders
        if (Physics.SphereCast(ray, 0.15f, out hit, 25f, photoLayers, QueryTriggerInteraction.Ignore))
        {
            //name of the hit object
            Debug.Log("Hit: " + hit.collider.name);
            //actual ray displayed as a green line
            Debug.DrawLine(ray.origin, hit.point, Color.green, 5f);

            PhotoTargetOfInterest target = hit.collider.GetComponentInParent<PhotoTargetOfInterest>();
            if (target != null)
            {
                target.Captured();
            }
            else
            {
                Debug.Log("component not found");
            }
        }

    }

}
