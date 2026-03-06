using UnityEngine;

public class KeypadInteract : MonoBehaviour
{
    public DoorKeypad keypadUI;
    public float interactDistance = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(
                Camera.main.transform.position,
                transform.position
            );

            if (distance <= interactDistance)
            {
                keypadUI.OpenUI();
            }
        }
    }
}