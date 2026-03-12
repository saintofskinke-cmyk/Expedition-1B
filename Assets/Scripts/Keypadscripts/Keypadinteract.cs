using UnityEngine;

public class KeypadTrigger : MonoBehaviour
{
    public DoorKeypad keypad;

    private bool playerInside = false;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            keypad.OpenUI();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
