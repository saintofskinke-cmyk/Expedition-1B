using UnityEngine;

public class GeneratorLamp : MonoBehaviour
{

    private void Start()
    {
        EventManager.Generator += TurnOnLight;
    }

    private void TurnOnLight()
    {
        GetComponentInChildren<Light>().enabled = true;
    }

    private void OnDisable()
    {
        EventManager.Generator -= TurnOnLight;
    }
}
