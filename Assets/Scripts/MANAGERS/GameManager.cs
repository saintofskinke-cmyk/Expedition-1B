using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    

    [Header("Items")]
    public GameObject flarePrefab;

    [Header("Materials")]
    public Material standardBulb_Unlit;
    public Material standardBulb_Lit;



    [Header("Player Input")]
    public InputActionReference pauseAction;
    public InputActionReference interAction;

    private void OnEnable()
    {
        pauseAction.action.Enable();
        interAction.action.Enable();
    }
    private void OnDisable()
    {
        pauseAction.action.Disable();
        interAction.action.Disable();
    }
    
    public static GameManager Instance;
    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }
}
