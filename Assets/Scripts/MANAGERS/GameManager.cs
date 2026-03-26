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
    public InputActionReference exitUIAction;

    private void OnEnable()
    {
        pauseAction.action.Enable();
        exitUIAction.action.Enable();
    }
    private void OnDisable()
    {
        pauseAction.action.Disable();
        exitUIAction.action.Disable();
    }
    
    public static GameManager Instance;
    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); }
        else { Instance = this; }
    }
}
