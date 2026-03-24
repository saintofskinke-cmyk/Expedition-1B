using UnityEngine;

public class StartPlayerEvent : MonoBehaviour
{
    private void Start()
    {
        EventManager.StartPlayer += StartPlayerController;
    }

    private void StartPlayerController()
    { GetComponent<PlayerController>().enabled = true; }

    private void OnDisable()
    {
        EventManager.StartPlayer -= StartPlayerController;
    }
}
