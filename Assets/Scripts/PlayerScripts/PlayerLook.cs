using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("Actions")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference handInterAction;

    [Header("Looking Parameters")]
    [SerializeField] private Transform orientation;
    private float mouseSens = 20f;
    private float xRotation;
    private float yRotation;

    [Header("Pick Up Parameters")]
    private int pickUpRange = 4;
    public bool hasItemInHand;
    [SerializeField] private Transform handAnchor;
    private Transform originalHandItemAnchor;
    public Transform itemInHand;
    private VisualElement root;
    private Label txtPickUp;

    private void Awake()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        root = GetComponent<UIDocument>().rootVisualElement;
        txtPickUp = root.Q<Label>("txtPickUp");
    }

    private void OnEnable()
    {
        lookAction.action.Enable();
        handInterAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
        handInterAction.action.Disable();
    }

    private void Update()
    {
        LookUpdate();
        PickUpdate();
    }

    private void LookUpdate()
    {
        Vector2 mouse = mouseSens * Time.deltaTime * lookAction.action.ReadValue<Vector2>();

        yRotation += mouse.x;
        xRotation -= mouse.y;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        // Camera rotation & orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void PickUpdate()
    {
        if (!playerController.inPhotoMode)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, pickUpRange) 
                && (hit.collider.gameObject.CompareTag("Item") || hit.collider.gameObject.CompareTag("Camera")))
            {
                txtPickUp.style.display = DisplayStyle.Flex;
                if (handInterAction.action.WasPressedThisFrame() && !hasItemInHand)
                {
                    originalHandItemAnchor = hit.transform.parent;
                    OnItemPickedUp(hit, handAnchor);
                    itemInHand = hit.transform;
                    hasItemInHand = true;
                }
                else TryItemDrop();
            }
            else
            {
                TryItemDrop();
                txtPickUp.style.display = DisplayStyle.None;
            }
        }
    }
    
    private void TryItemDrop()
    {
        if (handInterAction.action.WasPressedThisFrame() && hasItemInHand)
        {
            OnItemDropped(hasItemInHand, itemInHand, originalHandItemAnchor);
            hasItemInHand = false;
        }
    }
    private void OnItemPickedUp(RaycastHit hit, Transform handAnchor)
    {
        hit.transform.SetParent(handAnchor);
        hit.rigidbody.isKinematic = true;
        hit.collider.enabled = false;
        hit.transform.position = handAnchor.position;
        hit.transform.rotation = handAnchor.rotation;
    }

    private void OnItemDropped(bool hasItemInHand, Transform itemInHand, Transform originalAnchor)
    {
        itemInHand.GetComponent<Rigidbody>().isKinematic = false;
        itemInHand.GetComponent<Collider>().enabled = true;
        itemInHand.SetParent(originalAnchor);
    }
}
