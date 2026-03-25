using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    [SerializeField] public bool requiresKey;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.Generator += TagInteractable;
    }

    private void TagInteractable()
    {
        if (!requiresKey)
        {
            gameObject.tag = "Interactable";
        }
    }

    private void OnDisable()
    {
        EventManager.Generator -= TagInteractable;
    }

    public void UnlockWithKey()
    {
        
        gameObject.tag = "Interactable";
    }

    public void LockWithoutKey()
    {

        gameObject.tag = "Untagged";
    }
}
