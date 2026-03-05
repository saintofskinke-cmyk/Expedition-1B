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
    [SerializeField] private GameObject GM;
    private Inventory inventory;


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

    [Header("Other Parameters")]
    private bool isFlareThrown;

    #region Input Actions
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference throwFlareAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();
        jumpAction.action.Enable();
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();
        jumpAction.action.Disable();
    }
    #endregion

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        eyesPosY = eyes.position.y;
        eyesPosCrouch = eyesPosY - 0.3f;
        eyesPosStand = eyesPosCrouch + 0.3f;
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        MoveUpdate();
        ActionUpdate();
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

    private void ActionUpdate()
    {
        if(throwFlareAction.action.WasPerformedThisFrame() && inventory.flareCount != 0 && !isFlareThrown)
        {
            FlareCountdown();
            // Setting random rotation for the flare
            int rndX = Random.Range(-180, 180);
            int rndY = Random.Range(-180, 180);
            int rndZ = Random.Range(-180, 180);

            // Instantiate flare and add forward force
            GameObject flare = Instantiate(GM.GetComponent<GameManager>().flarePrefab, eyes.position + eyes.forward, Quaternion.Euler(rndX, rndY, rndZ));
            flare.GetComponent<Rigidbody>().AddForce(eyes.forward * 10f, ForceMode.VelocityChange);

            // Remove flare from inventory
            inventory.RemoveItem("Flare");

        }
    }

    IEnumerator FlareCountdown()
    {
        isFlareThrown = true;
        yield return new WaitForSeconds(2f);
        isFlareThrown = false;
    }
}
