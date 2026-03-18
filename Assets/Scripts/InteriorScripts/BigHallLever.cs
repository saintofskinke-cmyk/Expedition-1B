using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigHallLever : MonoBehaviour
{
    [SerializeField] private List<GameObject> industrialLamps;
    [SerializeField] private Material bolbMaterial_Lit;

    public void ActivateLever()
    {
        StartCoroutine(PowerHallLights());
        gameObject.transform.GetChild(1).tag = "Untagged"; // Remove the tag from the lever to prevent it from being activated again
    }

    IEnumerator PowerHallLights()
    {
        foreach (GameObject _gameObject in industrialLamps)
        {
            yield return new WaitForSeconds(0.5f);
            _gameObject.GetComponentsInChildren<Light>()[0].enabled = true;
            _gameObject.GetComponentsInChildren<Light>()[1].enabled = true; 
            _gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = bolbMaterial_Lit; // Change the material of the lightbulb to a lit version
            yield return new WaitForSeconds(1f);
        }
    }
}
