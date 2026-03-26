using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private PlayerController playerController;

    [Header("Actions")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference handInterAction;
    [SerializeField] private InputActionReference handPickUpAction;

    [Header("Looking Parameters")]
    [SerializeField] private Transform orientation;
    public float mouseSens = 7f;
    private float xRotation;
    private float yRotation;

    [Header("Pick Up Parameters")]
    [SerializeField] private Camera raycastCamera;
    private int pickUpRange = 4;
    public bool hasItemInHand, isHoldingFlare;
    [SerializeField] private Transform handAnchor, secondHandAnchor;
    private Transform originalHandItemAnchor, originalHandFlareAnchor;
    public Transform itemInHand;
    public GameObject playerHudDocument, flareInHand;
    private VisualElement root;
    private Label txtPickUp;

    private void Awake()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        root = playerHudDocument.GetComponent<UIDocument>().rootVisualElement;
        txtPickUp = root.Q<Label>("txtPickUp");
        mouseSens = 0f;
    }

    private void Start()
    {
        root.style.display = DisplayStyle.None;
    }

    public void Enable() { 
        pauseMenu.GetComponent<PauseMenu>().UpdateSettings();
    }

    private void OnEnable()
    {
        lookAction.action.Enable();
        handInterAction.action.Enable();
        handPickUpAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
        handInterAction.action.Disable();
        handPickUpAction.action.Disable();
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
        transform.rotation = Quaternion.Euler(xRotation, yRotation - 90f, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void PickUpdate()
    {
        if (!playerController.isCameraInHand)
        {
            if (Physics.Raycast(raycastCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, pickUpRange))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.CompareTag("Interactable") || hitObj.CompareTag("Camera"))
                {
                    if (hitObj.layer == 7) { txtPickUp.text = "[E] or [R]"; } // if an interactable object can be picked up
                    else { txtPickUp.text = "[E]"; }
                    txtPickUp.style.display = DisplayStyle.Flex;
                    
                    if (handInterAction.action.WasPressedThisFrame())
                    {
                        hitObj.GetComponent<InteractionHandler>().StartInteractionLogic();
                        return;
                    }
                    else if (handPickUpAction.action.WasPressedThisFrame() && !hasItemInHand && hitObj.layer == 7)
                    {
                        SetItemInHand(hit);
                        return;
                    }
                    else { TryItemDrop(); }
                    return;
                }
                else if (hitObj.CompareTag("Item") || hitObj.CompareTag("ImportantItem") || hitObj.CompareTag("Flare"))
                {
                    txtPickUp.text = "[R]";
                    txtPickUp.style.display = DisplayStyle.Flex;

                    if (handPickUpAction.action.WasPressedThisFrame() && !hasItemInHand)
                    {
                        switch (hit.collider.gameObject.tag)
                        {
                            case "Item":
                                SetItemInHand(hit);
                                break;

                            case "Flare":
                                if (isHoldingFlare) { return; }
                                originalHandFlareAnchor = hit.transform.parent;
                                OnItemPickedUp(hit, secondHandAnchor);
                                isHoldingFlare = true;
                                flareInHand = hit.transform.gameObject;
                                flareInHand.GetComponentInChildren<Light>().transform.localPosition = new Vector3(-0.02f, 0.7f, -0.11f); // Change the position of the light to make it look better in the player's hand
                                break;

                            case "ImportantItem":
                                if (hit.collider.gameObject.GetComponent<ItemPickupEvent>() != null)
                                {
                                    hit.collider.gameObject.GetComponent<ItemPickupEvent>().OnPickup();
                                }
                                SetItemInHand(hit);
                                break;
                        }
                    }
                    else { TryItemDrop(); }
                    return;   
                }
            }
            TryItemDrop();
        }
        txtPickUp.style.display = DisplayStyle.None;
    }
    
    private void SetItemInHand(RaycastHit hit)
    {
        originalHandItemAnchor = hit.transform.parent;
        OnItemPickedUp(hit, handAnchor);
        itemInHand = hit.transform;
        hasItemInHand = true;
    }

    private void TryItemDrop()
    {
        if (handPickUpAction.action.WasPressedThisFrame() && hasItemInHand)
        {
            if (itemInHand.CompareTag("Flare"))
            {
                itemInHand.GetComponentInChildren<Light>().transform.localPosition = new Vector3(0f, 0.92f, 0f); // Change the position of the light to make it look better when dropped
            }
            OnItemDropped(itemInHand, originalHandItemAnchor, 10f);
            hasItemInHand = false;
        }
    }

    public void TryFlareDrop()
    {
        if (playerController.throwFlareAction.action.WasPressedThisFrame() && isHoldingFlare)
        {
            flareInHand.GetComponentInChildren<Light>().transform.localPosition = new Vector3(0f, 0.92f, 0f); // Change the position of the light to make it look better when dropped
            OnItemDropped(flareInHand.transform, originalHandFlareAnchor, 10f);
            isHoldingFlare = false;
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

    private void OnItemDropped(Transform itemInHand, Transform originalAnchor, float force)
    {
        itemInHand.GetComponent<Rigidbody>().isKinematic = false;
        itemInHand.GetComponent<Collider>().enabled = true;
        itemInHand.SetParent(originalAnchor);

        // Random rotation
        int rndX = Random.Range(-90, 90);
        int rndY = Random.Range(-90, 90);
        int rndZ = Random.Range(-90, 90);
        itemInHand.Rotate(rndX, rndY, rndZ);
        
        // Throw the item
        itemInHand.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.VelocityChange);
        StartCoroutine(playerController.FlareCountdown());
    }

    public void UpdateTextUI()
    {
        root = playerHudDocument.GetComponent<UIDocument>().rootVisualElement;
        txtPickUp = root.Q<Label>("txtPickUp");
    }
}
