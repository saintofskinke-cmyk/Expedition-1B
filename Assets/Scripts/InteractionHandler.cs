using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField]
    private Animator interactionAnimator;
    private bool isLeverActivated = false;


    public void StartInteractionLogic()
    {
        Debug.Log($"Interacted with {gameObject.name}");
        switch (gameObject.name)
            {
            case "GeneratorLever":
                interactionAnimator = GetComponentInChildren<Animator>();
                isLeverActivated = !isLeverActivated; // Toggle the lever state
                interactionAnimator.SetBool("ActivateLever", isLeverActivated); 
                Debug.Log("Lever Pulled: " + isLeverActivated);
                break;
            default:
                Debug.LogWarning("No animation found for this object.");
                break;
        }
    }

}
