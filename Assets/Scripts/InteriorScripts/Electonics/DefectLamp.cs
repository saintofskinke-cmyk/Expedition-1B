using System.Collections;
using UnityEngine;

public class DefectLamp : MonoBehaviour
{
    [SerializeField] private GameObject bulb;

    [Header("Bulb Settings")]
    [SerializeField] private float minOffTime = 0.5f;
    [SerializeField] private float maxOffTime = 1f;
    [SerializeField] private float minOnTime = 0.2f;
    [SerializeField] private float maxOnTime = 0.3f;
    [SerializeField] private Material matUnlit, matLit; // needs to detach bulbs from lamp...

    private void Start() { EventManager.Generator += StartLights; bulb.SetActive(false); }
   // private void OnDisable() { EventManager.Generator -= StartLights; }
    private void StartLights() { StartCoroutine(BlinkingLights()); }

    IEnumerator BlinkingLights()
    {
        while (true)
        {
            bulb.SetActive(false);
            float rndOffTime = Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(rndOffTime);

            bulb.SetActive(true);
            float rndOnTime = Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(rndOnTime);
        }
    }
}
