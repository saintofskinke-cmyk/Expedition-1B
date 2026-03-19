using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private Animator interactionAnimator;
    private bool boolValue = false;

    private int progressAmount = 1;
    [SerializeField] private int objectiveEventID = 0;
    private bool alreadyCompleted = false;
    [SerializeField] QuestManager questManager;
    [SerializeField] private GenStart genStart1;
    [SerializeField] private GenStart genStart2;

    [Header("Valve Parameters")]
    private int valveTurn;
    private int maxValveTurns = 5;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    public void StartInteractionLogic()
    {
        // Functionallity for different interactable objects can be added here by using the name of the gameobject as a switch case. This way we can have one script that handles all interactions in the game and we can easily add new interactable objects by adding a new case to the switch statement.
        switch (gameObject.name)
        {
            case "LeverHandle":
                PlayAnimation();
                if (boolValue && objectiveEventID == questManager.currentObjectiveIndex && !alreadyCompleted)
                {
                    questManager.Progress(progressAmount);
                    alreadyCompleted = true;
                    genStart1.PlayGenSound();
                    genStart2.PlayGenSound();
                }
                break;

            case "BigHallLeverHandle":
                PlayAnimation();
                gameObject.GetComponentInParent<BigHallLever>().ActivateLever();
                break;

            case "RedValve":
                valveTurn++;
                if(valveTurn >= maxValveTurns) { valveTurn = 0; }

                switch (valveTurn)
                {
                    case 0:
                        gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        break;
                }
                Debug.Log(valveTurn);
                break;

            default:
                PlayAnimation();
                break;
        }
    }

    private void PlayAnimation()
    {
        interactionAnimator = GetComponent<Animator>();
        boolValue = !boolValue; // Toggle animation state
        interactionAnimator.SetBool("Activate", boolValue);
    }

}
