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
        transform.GetChild(1).GetComponent<MeshRenderer>().material = GameManager.Instance.standardBulb_Lit;
    }

    private void OnDisable()
    {
        EventManager.Generator -= TurnOnLight;
    }
}
