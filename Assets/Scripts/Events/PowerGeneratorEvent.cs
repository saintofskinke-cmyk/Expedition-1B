using System.Collections.Generic;
using UnityEngine;

public class PowerGeneratorEvent : MonoBehaviour
{
    public static PowerGeneratorEvent Instance;

    [Header("Generator Room")]
    [SerializeField] private GameObject generatorAlarmLamp;
    [SerializeField] private GameObject generatorLamps;
    private List<GameObject> generatorLights = new List<GameObject>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < generatorLamps.transform.childCount; i++) {
            generatorLights.Add(generatorLamps.transform.GetChild(i).gameObject);
        }
    }

    public void ActivateEvent()
    {
        generatorAlarmLamp.GetComponentInChildren<Light>().enabled = false;
        foreach (GameObject go in generatorLights)
        {
            go.GetComponentInChildren<Light>().enabled = true;
        }
    }
}
