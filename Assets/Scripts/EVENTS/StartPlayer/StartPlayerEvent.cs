using UnityEngine;
using UnityEngine.UIElements;

public class StartPlayerEvent : MonoBehaviour
{
    private void Start()
    {
        EventManager.StartPlayer += StartPlayerController;
    }

    private void StartPlayerController()
    { 
        GetComponent<PlayerController>().enabled = true; 
        GetComponentInChildren<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void OnDisable()
    {
        EventManager.StartPlayer -= StartPlayerController;
    }
}
