using System.Collections;
using UnityEngine;

public class DefectLamp : MonoBehaviour
{
    GameObject bolbs;

    [Header("Bulb Settings")]
    [SerializeField] private float minOffTime = 0.5f;
    [SerializeField] private float maxOffTime = 1f;
    [SerializeField] private float minOnTime = 0.2f;
    [SerializeField] private float maxOnTime = 0.3f;

    private void Start() {
        bolbs = transform.GetChild(0).gameObject;
        EventManager.Generator += StartLights;
        bolbs.SetActive(false);
    }
   // private void OnDisable() { EventManager.Generator -= StartLights; }
    private void StartLights() { StartCoroutine(BlinkingLights()); }

    IEnumerator BlinkingLights()
    {
        while (true)
        {
            bolbs.SetActive(false);
            GetComponent<MeshRenderer>().material = GameManager.Instance.standardBulb_Unlit;
            float rndOffTime = Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(rndOffTime);

            bolbs.SetActive(true);
            GetComponent<MeshRenderer>().material = GameManager.Instance.standardBulb_Lit;
            float rndOnTime = Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(rndOnTime);
        }
    }
}
