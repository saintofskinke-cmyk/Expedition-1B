using UnityEngine;

public class GeneratorLamp : MonoBehaviour
{
    [SerializeField] private Material materialLit;

    private void Start() { EventManager.Generator += TurnOnLight; }

    private void TurnOnLight()
    {
        foreach (Light light in GetComponentsInChildren<Light>()) { light.enabled = true; }
        GetComponent<MeshRenderer>().material = materialLit;
        GetComponent<AudioSource>().Play();
    }

    private void OnDisable()
    {
        EventManager.Generator -= TurnOnLight;
    }
}
