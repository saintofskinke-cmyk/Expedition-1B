using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantMetalDoor : MonoBehaviour
{
    [SerializeField] private List<GameObject> correctValves_Left, correctValves_Right; // Listen holder styr på hvilke valves der er sat correct
    [SerializeField] private GameObject leverLeft, leverRight, alarmLampLeft, alarmLampRight;
    [SerializeField] private GameObject[] alarmLamp_green;
    private Animator anim;

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

                    anim.SetBool(doorSide, true); // Play GiantValve animation

                    leverLeft.tag = "Interactable"; // Make the lever interactable
                    alarmLampLeft.SetActive(true); // Turn on the red light
                }
                break;

            case (true, "Right"):
                correctValves_Right.Add(valve);
                if (correctValves_Right.Count == 3)
                {
                    foreach (GameObject redValve in correctValves_Right)
                    { redValve.tag = "Untagged"; } // Make sure the player can't turn the valves after completion

                    anim.SetBool(doorSide, true); // Play GiantValve animation

                    leverRight.tag = "Interactable"; // Make the lever interactable
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
            yield return new WaitForSeconds(1f);

            foreach (GameObject greenAlarmLamp in alarmLamp_green) {
                greenAlarmLamp.SetActive(true);
            }
        }
        
    }
}
