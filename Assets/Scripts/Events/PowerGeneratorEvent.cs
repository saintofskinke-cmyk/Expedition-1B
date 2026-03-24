using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGeneratorEvent : MonoBehaviour
{
    [Header("Generator Room")]
    [SerializeField] private GameObject generatorAlarmLamp;
    [SerializeField] private AudioSource generatorAudio;



    private void Start() { EventManager.Generator += PowerGenerator; }
    private void OnDisable() { EventManager.Generator -= PowerGenerator; }

    private void PowerGenerator()
    {
        generatorAlarmLamp.GetComponentInChildren<Light>().enabled = false;

        // Audio
        generatorAudio.PlayOneShot(AudioManager.Instance.generatorRoom_StartUp);
        StartCoroutine(GeneratorSound());
    }

    IEnumerator GeneratorSound()
    {
        while (generatorAudio.isPlaying)
        {
            yield return null;
        }
        generatorAudio.Play();
    }
}
