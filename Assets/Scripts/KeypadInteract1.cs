using UnityEngine;
using UnityEngine.UIElements;

public class KeypadInteract1 : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label interactText;

    private bool playerNearby = false;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        interactText = root.Q<Label>("InteractText");

        interactText.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player interacted!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            interactText.style.display = DisplayStyle.Flex;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            interactText.style.display = DisplayStyle.None;
        }
    }
}