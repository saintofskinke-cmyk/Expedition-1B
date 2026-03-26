using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.Generator += TagInteractable;
    }

    private void TagInteractable()
    {
        gameObject.tag = "Interactable";
    }

    private void OnDisable()
    {
        EventManager.Generator -= TagInteractable;
    }
}
