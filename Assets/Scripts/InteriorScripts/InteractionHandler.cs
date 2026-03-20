using System.Collections.Generic;
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

    [Header("Giant Metal Door Parameters")]
    private int valveTurn;
    private int maxValveTurns = 8;
    private int correctValveTurn;
    [SerializeField] private string valveSide;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        correctValveTurn = Random.Range(0, maxValveTurns);
    }

    public void StartInteractionLogic()
    {
        // Functionallity for different interactable objects can be added here by using the name of the gameobject as a switch case.
        // This way we can have one script that handles all interactions in the game and we can easily add new interactable objects by adding a new case to the switch statement.
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
                PowerGeneratorEvent.Instance.ActivateEvent();

                break;

            case "BigHallLeverHandle":
                PlayAnimation();
                gameObject.GetComponentInParent<BigHallLever>().ActivateLever();
                break;

            case "GiantDoorLeverHandle":
                PlayAnimation();
                StartCoroutine(GetComponentInParent<GiantMetalDoor>().OpenGiantMetalDoor(valveSide));
                gameObject.GetComponent<Collider>().enabled = false;
                break;

            case "RedValve":
                // Change the turn value
                valveTurn++;
                if(valveTurn >= maxValveTurns) { valveTurn = 0; }

                // Play a different sound at each valve turn
                AudioSource.PlayClipAtPoint(GameManager.Instance.valveSounds[valveTurn], gameObject.transform.position);

                // Check if Valve is turned correctly
                if (valveTurn == correctValveTurn) {
                    GetComponentInParent<GiantMetalDoor>().UnlockGiantMetalDoor(gameObject, true, valveSide);
                } else {
                    GetComponentInParent<GiantMetalDoor>().UnlockGiantMetalDoor(gameObject, false, valveSide);
                }
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
