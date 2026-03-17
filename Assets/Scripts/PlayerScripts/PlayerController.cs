using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    private CharacterController controller;

    [SerializeField] private Transform eyes;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private GameObject GM;

    private Inventory inventory;

    private float eyesPosY;
    private float eyesPosCrouch;
    private float eyesPosStand;

    [Header("Movement Parameters")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;

    private bool isPlayerGrounded;
    private bool isPlayerCrouching;
    private Vector3 velocity;

    [Header("Other Parameters")]
    private bool isFlareThrown;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference throwFlareAction;

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (sprintAction != null) sprintAction.action.Enable();
        if (crouchAction != null) crouchAction.action.Enable();
        if (jumpAction != null) jumpAction.action.Enable();
        if (throwFlareAction != null) throwFlareAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (sprintAction != null) sprintAction.action.Disable();
        if (crouchAction != null) crouchAction.action.Disable();
        if (jumpAction != null) jumpAction.action.Disable();
        if (throwFlareAction != null) throwFlareAction.action.Disable();
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inventory = GetComponent<Inventory>();

        if (controller == null)
        {
            Debug.LogError("PlayerController: CharacterController mangler.");
            enabled = false;
            return;
        }

        if (eyes == null)
        {
            Debug.LogError("PlayerController: 'eyes' reference mangler i Inspector.");
            enabled = false;
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("PlayerController: Inventory component mangler på samme objekt.");
            enabled = false;
            return;
        }

        eyesPosY = eyes.localPosition.y;
        eyesPosCrouch = eyesPosY - 0.3f;
        eyesPosStand = eyesPosY;
    }

    private void Update()
    {
        if (!enabled) return;

        velocity.y += gravity * Time.deltaTime;

        MoveUpdate();
        ActionUpdate();
    }

    private void MoveUpdate()
    {
        isPlayerGrounded = controller.isGrounded;

        if (isPlayerGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (jumpAction != null && jumpAction.action.triggered && isPlayerGrounded && !isPlayerCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        Vector3 eyesForward = eyes.forward;
        Vector3 eyesRight = eyes.right;

        eyesForward.y = 0f;
        eyesRight.y = 0f;

        eyesForward.Normalize();
        eyesRight.Normalize();

        Vector2 input = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        Vector3 move = (eyesRight * input.x + eyesForward * input.y).normalized;

        if (crouchAction != null && crouchAction.action.IsPressed() && isPlayerGrounded)
        {
            isPlayerCrouching = true;
            controller.height = crouchingHeight;

            Vector3 localPos = eyes.localPosition;
            localPos.y = eyesPosCrouch;
            eyes.localPosition = localPos;
        }
        else if (isPlayerCrouching)
        {
            bool blockedAbove = Physics.Raycast(eyes.position, Vector3.up, 0.5f);

            if (!blockedAbove)
            {
                isPlayerCrouching = false;
                controller.height = standingHeight;

                Vector3 localPos = eyes.localPosition;
                localPos.y = eyesPosStand;
                eyes.localPosition = localPos;
            }
        }

        if (isPlayerGrounded)
        {
            if (isPlayerCrouching)
            {
                moveSpeed = crouchSpeed;
            }
            else if (sprintAction != null && sprintAction.action.IsPressed())
            {
                moveSpeed = sprintSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
        }

        Vector3 finalMove = move * moveSpeed + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void ActionUpdate()
    {
        if (throwFlareAction == null || inventory == null)
            return;

        if (throwFlareAction.action.WasPerformedThisFrame() && inventory.flareCount > 0 && !isFlareThrown)
        {
            GameManager gameManager = null;

            if (GM != null)
            {
                gameManager = GM.GetComponent<GameManager>();
            }

            if (gameManager == null)
            {
                Debug.LogError("PlayerController: Kunne ikke finde GameManager på GM reference.");
                return;
            }

            if (gameManager.flarePrefab == null)
            {
                Debug.LogError("PlayerController: flarePrefab mangler på GameManager.");
                return;
            }

            StartCoroutine(FlareCountdown());

            int rndX = Random.Range(-180, 181);
            int rndY = Random.Range(-180, 181);
            int rndZ = Random.Range(-180, 181);

            GameObject flare = Instantiate(
                gameManager.flarePrefab,
                eyes.position + eyes.forward,
                Quaternion.Euler(rndX, rndY, rndZ)
            );

            Rigidbody flareRb = flare.GetComponent<Rigidbody>();
            if (flareRb != null)
            {
                flareRb.AddForce(eyes.forward * 10f, ForceMode.VelocityChange);
            }
            else
            {
                Debug.LogWarning("PlayerController: Det instantiatede flare prefab har ingen Rigidbody.");
            }

            inventory.RemoveItem("Flare");
        }
    }

    private IEnumerator FlareCountdown()
    {
        isFlareThrown = true;

        float seconds = 2f;
        float barSize = 0.4f;

        if (inventory != null && inventory.IsUiReady && inventory.flareCooldown != null)
        {
            inventory.flareCooldown.style.scale = new Scale(new Vector3(0.5f, barSize, 1f));
        }

        int iterations = Mathf.RoundToInt(seconds / 0.5f);

        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForSeconds(0.5f);
            barSize -= 0.1f;

            if (inventory != null && inventory.IsUiReady && inventory.flareCooldown != null)
            {
                inventory.flareCooldown.style.scale = new Scale(new Vector3(0.5f, Mathf.Max(0f, barSize), 1f));
            }
        }

        isFlareThrown = false;
    }
}
