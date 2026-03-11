using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField]
    private Animator interactionAnimator;
    private bool isLeverActivated = false;


    public void StartInteractionLogic()
    {
        

        switch (gameObject.name)
        {
            case "LeverHandle":
                isLeverActivated = !isLeverActivated; // Toggle the lever state
                interactionAnimator.SetBool("ActivateLever", isLeverActivated); 
                break;

            case "Door_Wooden":
                isLeverActivated = !isLeverActivated;
                interactionAnimator.SetBool("OpenDoor", isLeverActivated);
                break;

            default:
                Debug.LogWarning("No animation found for this object.");
                break;
        }
    }

}
