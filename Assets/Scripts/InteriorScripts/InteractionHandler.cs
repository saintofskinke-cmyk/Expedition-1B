using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionHandler : MonoBehaviour
{
    AudioManager AUDIO;
    Vector3 objPos;
    private Animator interactionAnimator;
    private bool boolValue = false;
    private PaperInput paperInput;

    private int progressAmount = 1;
    [SerializeField] private int objectiveEventID = 0;
    private bool alreadyCompleted = false;
    [SerializeField] private QuestManager questManager;

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
        AUDIO = AudioManager.Instance;
        objPos = transform.position;
    }

    private void Update()
    {
        if (name == "RedValve" && !GiantMetalDoor.isDoorUnlocked)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotAngle, 0.1f);
        }
    }

    public void StartInteractionLogic()
    {
        // Functionallity for different interactable objects can be added here by using the name of the gameobject as a switch case.
        // This way we can have one script that handles all interactions in the game and we can easily add new interactable objects by adding a new case to the switch statement.
        switch (name)
        {
            case "Door_Metal": PlayAnimation(); AudioSource.PlayClipAtPoint(AUDIO.metalDoorOpen, objPos); break;

            case "GeneratorLeverHandle":
                PlayAnimation();
                AudioSource.PlayClipAtPoint(AUDIO.lever, objPos);
                if (boolValue && objectiveEventID == questManager.currentObjectiveIndex && !alreadyCompleted)
                {
                    questManager.Progress(progressAmount);
                    alreadyCompleted = true;
                }
                tag = "Untagged";
                EventManager.StartGenerator();
                break;

            case "BigHallLeverHandle":
                PlayAnimation();
                AudioSource.PlayClipAtPoint(AUDIO.lever, objPos);
                GetComponentInParent<BigHallLever>().ActivateLever();
                break;

            case "GiantDoorLeverHandle":
                PlayAnimation();
                AudioSource.PlayClipAtPoint(AUDIO.lever, objPos);
                StartCoroutine(GetComponentInParent<GiantMetalDoor>().OpenGiantMetalDoor(valveSide));
                GetComponent<Collider>().enabled = false;
                break;

            case "Keypad": GetComponent<DoorKeypad>().OpenUI(); break;

            case "Radio_Code":
                GetComponent<UIDocument>().rootVisualElement.Q("RadioUI").style.display = DisplayStyle.Flex;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                GameManager.Instance.pauseAction.action.Disable();

                if (questManager.currentObjectiveIndex == objectiveEventID)
                {
                    questManager.Progress(progressAmount);
                }
                    break;

            case "RedValve":
                // Change the turn value
                valveTurn++;
                if(valveTurn >= maxValveTurns) { valveTurn = 0; }
                rotAngle = Quaternion.Euler(transform.rotation.x - 45f * valveTurn, transform.rotation.y + 180, transform.rotation.z);

                // Play a different sound at each valve turn
                AudioSource.PlayClipAtPoint(AUDIO.redValveSounds[valveTurn], objPos);

                // Check if Valve is turned correctly
                if (valveTurn == GetComponent<RedValve>().correctValveTurn) {
                    GetComponentInParent<GiantMetalDoor>().UnlockGiantMetalDoor(gameObject, true, valveSide);
                } else {
                    GetComponentInParent<GiantMetalDoor>().UnlockGiantMetalDoor(gameObject, false, valveSide);
                }
                break;

            case "Paper":
                paperInput = gameObject.GetComponent<PaperInput>();
                paperInput.ShowPaperText();

                break;

            default: PlayAnimation(); break;
        }
    }

    private void PlayAnimation()
    {
        interactionAnimator = GetComponent<Animator>();
        boolValue = !boolValue; // Toggle animation state
        interactionAnimator.SetBool("Activate", boolValue);
    }
}
