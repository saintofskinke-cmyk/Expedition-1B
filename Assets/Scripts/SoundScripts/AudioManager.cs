using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Player")]
    public AudioClip[] footSteps;

    [Header("Valves")]
    public AudioClip[] redValveSounds;
    public AudioClip[] radioValveSounds;
    public AudioClip giantValveTurn, giantValveTurn_2;


    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }
}
