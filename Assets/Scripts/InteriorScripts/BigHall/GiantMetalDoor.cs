using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GiantMetalDoor : MonoBehaviour
{
    public static bool isDoorUnlocked = false;

    [SerializeField] private List<GameObject> correctValves_Left, correctValves_Right; // Listen holder styr på hvilke valves der er sat correct
    [SerializeField] private GameObject leverLeft, leverRight, alarmLampLeft, alarmLampRight;
    [SerializeField] private GameObject[] alarmLamp_green;
    private Animator anim;
    [SerializeField] private QuestManager questManager;
    private int progressAmount = 1;
    [SerializeField] private int objectiveEventID = 0;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void UnlockGiantMetalDoor(GameObject valve, bool correctValveTurn, string doorSide)
    {
        switch (correctValveTurn, doorSide)
        {
            case (false, "Left"): correctValves_Left.Remove(valve); break;
            case (false, "Right"): correctValves_Right.Remove(valve); break;

            case (true, "Left"): 
                correctValves_Left.Add(valve); 
                if (correctValves_Left.Count == 3)
                {
                    foreach (GameObject redValve in correctValves_Left)
                    { redValve.tag = "Untagged"; } // Make sure the player can't turn the valves after completion

                    AudioSource.PlayClipAtPoint(AudioManager.Instance.giantValveTurn, gameObject.transform.GetChild(1).position);
                    anim.SetBool(doorSide, true); // Play GiantValve animation

                    leverLeft.transform.GetChild(1).tag = "Interactable"; // Make the lever interactable
                    alarmLampLeft.SetActive(true); // Turn on the red light
                }
                break;

            case (true, "Right"):
                correctValves_Right.Add(valve);
                if (correctValves_Right.Count == 3)
                {
                    foreach (GameObject redValve in correctValves_Right)
                    { redValve.tag = "Untagged"; } // Make sure the player can't turn the valves after completion

                    AudioSource.PlayClipAtPoint(AudioManager.Instance.giantValveTurn_2, gameObject.transform.GetChild(2).position);
                    anim.SetBool(doorSide, true); // Play GiantValve animation

                    leverRight.transform.GetChild(1).tag = "Interactable"; // Make the lever interactable
                    alarmLampRight.SetActive(true); // Turn on the red light
                }
                break;
        }
    }

    public IEnumerator OpenGiantMetalDoor(string leverSide)
    {
        yield return new WaitForSeconds(1f);

        switch (leverSide) {
            case "Left":
                alarmLampLeft.SetActive(false);
                anim.SetBool("LeverLeft", true);
                break;

            case "Right": 
                alarmLampRight.SetActive(false);
                anim.SetBool("LeverRight", true);
                break;
        }

        if (anim.GetBool("LeverLeft") == true && anim.GetBool("LeverRight") == true)
        {
            isDoorUnlocked = true;
            
            if(questManager.currentObjectiveIndex == objectiveEventID)
            {
                questManager.Progress(progressAmount);
            }

            yield return new WaitForSeconds(1f);

            foreach (GameObject greenAlarmLamp in alarmLamp_green) {
                greenAlarmLamp.SetActive(true);
            }

            GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.giantMetalDoor_Open);

            StartCoroutine(StopVeryEarlyAccessGameplay());
        }
        
    }

    IEnumerator StopVeryEarlyAccessGameplay()
    {
        yield return new WaitForSeconds(19.57f);
        EventManager.StartWorm();

        yield return new WaitForSeconds(0.1957f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.wormScream);
        
        yield return new WaitForSeconds(1.957f);
        GetComponent<UIDocument>().enabled = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.endScreenSound);
        Time.timeScale = 0f;
    }
}
