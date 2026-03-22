using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Items")]
    public GameObject flarePrefab;

    public static GameManager Instance;
    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }
}
