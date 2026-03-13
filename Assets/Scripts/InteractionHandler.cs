using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private Animator interactionAnimator;
    private bool boolValue = false;

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
        // Animation
        interactionAnimator = GetComponent<Animator>();
        boolValue = !boolValue; // Toggle animation state
        interactionAnimator.SetBool("Activate", boolValue);

        // Functionallity
        switch (gameObject.name)
        {
            case "LeverHandle":
                if (boolValue && objectiveEventID == questManager.currentObjectiveIndex && !alreadyCompleted)
                {
                    questManager.Progress(progressAmount);
                    alreadyCompleted = true;
                }
                break;
        }
    }

}
