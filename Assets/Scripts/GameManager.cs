using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Items")]
    public GameObject flarePrefab;

    [Header("Sounds")]
    public AudioClip[] footSteps;
    public AudioClip[] valveSounds;


    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }
}
