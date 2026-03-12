using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField]
    private Animator interactionAnimator;
    private bool isLeverActivated = false;
    private int progressAmount = 1;
    [SerializeField] private int objectiveEventID = 0;
    private bool alreadyCompleted = false;
    [SerializeField] QuestManager questManager;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    public void StartInteractionLogic()
    {
        

        switch (gameObject.name)
        {
            case "LeverHandle":
                isLeverActivated = !isLeverActivated; // Toggle the lever state
                interactionAnimator.SetBool("ActivateLever", isLeverActivated);

                if (isLeverActivated && objectiveEventID == questManager.currentObjectiveIndex && !alreadyCompleted)
                {
                    questManager.Progress(progressAmount);
                    alreadyCompleted = true;
                }

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
