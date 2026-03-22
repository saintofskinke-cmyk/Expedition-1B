using System.Collections.Generic;
using System.Collections;
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
    private Quaternion rotAngle;
    [SerializeField] private string valveSide;
    [SerializeField] private int valveNumber;
    private bool isRadioPlaying = false;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    private void Update()
    {
        if (gameObject.name == "RedValve")
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotAngle, 0.1f);
        }
    }

    public void StartInteractionLogic()
    {
        // Functionallity for different interactable objects can be added here by using the name of the gameobject as a switch case.
        // This way we can have one script that handles all interactions in the game and we can easily add new interactable objects by adding a new case to the switch statement.
        switch (gameObject.name)
        {
            case "LeverHandle":
                PlayAnimation();
                AudioSource.PlayClipAtPoint(AudioManager.Instance.lever, gameObject.transform.position);
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
                AudioSource.PlayClipAtPoint(AudioManager.Instance.lever, gameObject.transform.position);
                gameObject.GetComponentInParent<BigHallLever>().ActivateLever();
                break;

            case "GiantDoorLeverHandle":
                PlayAnimation();
                AudioSource.PlayClipAtPoint(AudioManager.Instance.lever, gameObject.transform.position);
                StartCoroutine(GetComponentInParent<GiantMetalDoor>().OpenGiantMetalDoor(valveSide));
                gameObject.GetComponent<Collider>().enabled = false;
                break;

            case "Radio_Code":
                StartCoroutine(gameObject.GetComponent<RadioCode>().PlayCode(isRadioPlaying));
                break;

            case "RedValve":
                // Change the turn value
                valveTurn++;
                if(valveTurn >= maxValveTurns) { valveTurn = 0; }
                rotAngle = Quaternion.Euler(transform.rotation.x - 45f * valveTurn, transform.rotation.y + 180, transform.rotation.z);

                // Play a different sound at each valve turn
                AudioSource.PlayClipAtPoint(AudioManager.Instance.redValveSounds[valveTurn], gameObject.transform.position);

                // Check if Valve is turned correctly
                if (valveTurn == gameObject.GetComponent<RedValve>().correctValveTurn) {
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
