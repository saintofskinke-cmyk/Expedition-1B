using System.Collections;
using UnityEngine;

public class DefectLamp : MonoBehaviour
{
    [Header("Bulb Settings")]
    [SerializeField] private float minOffTime = 0.5f;
    [SerializeField] private float maxOffTime = 1f;
    [SerializeField] private float minOnTime = 0.2f;
    [SerializeField] private float maxOnTime = 0.3f;

    private void Start() { 
        EventManager.Generator += StartLights;
        GetComponentInChildren<Light>().enabled = false;
    }
   // private void OnDisable() { EventManager.Generator -= StartLights; }
    private void StartLights() { StartCoroutine(BlinkingLights()); }

    IEnumerator BlinkingLights()
    {
        while (true)
        {
            GetComponentInChildren<Light>().enabled = false;
            GetComponent<MeshRenderer>().material = GameManager.Instance.standardBulb_Unlit;
            float rndOffTime = Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(rndOffTime);

            GetComponentInChildren<Light>().enabled = true;
            GetComponent<MeshRenderer>().material = GameManager.Instance.standardBulb_Lit;
            float rndOnTime = Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(rndOnTime);
        }
    }
}
