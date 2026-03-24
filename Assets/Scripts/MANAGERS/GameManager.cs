using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }


    [Header("Items")]
    public GameObject flarePrefab;

    [Header("Materials")]
    public Material standardBulb_Unlit;
    public Material standardBulb_Lit;
}
