using UnityEngine;

public class FluorescentLamp : MonoBehaviour
{
    [SerializeField] private GameObject[] bulbs;
    [SerializeField] private GameObject leftLights, rightLights;

    private void Start() { 
        EventManager.Generator += PowerLamp;
        leftLights.SetActive(false);
        rightLights.SetActive(false);
    }

    private void PowerLamp()
    {
        leftLights.SetActive(true);
        rightLights.SetActive(true);

        foreach (GameObject bulb in bulbs)
        { bulb.GetComponent<MeshRenderer>().material = GameManager.Instance.standardBulb_Lit; }
    }
}
