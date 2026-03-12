using UnityEngine;

public class ObjectiveClass : MonoBehaviour
{
    public string description;
    public int requiredProgress = 1;
    public int currentProgress;
    public bool isComplete;

    //henter data om hvorvidt spilleren har gennemført det givne objective
    public bool IsObjectiveComplete()
    {
        if(currentProgress >= requiredProgress)
        {
            isComplete = true;
        }
        else
        {
            isComplete = false;
        }

       return isComplete;
    }

    //henter info om objective beskrivelse
    public string GetDescription()
    {
        if(requiredProgress > 1)
        {
            return $"{description} ({currentProgress}/{requiredProgress})";
        }
        return description;
    }

}
