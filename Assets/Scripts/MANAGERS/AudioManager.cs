using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Player")]
    public AudioClip[] footSteps;

    [Header("Electricity")]
    public AudioClip lever;
    public AudioClip generatorRoom_StartUp;
    public AudioClip generatorRoom_Loop;

    [Header("Valves")]
    public AudioClip[] redValveSounds;
    public AudioClip[] radioValveSounds;
    public AudioClip giantValveTurn, giantValveTurn_2;

    [Header("SFX")]
    public AudioClip endScreenSound;
    public AudioClip metalDoorUnlock;


    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }
}
