using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : MonoBehaviour
{
    public List<ObjectiveClass> objectiveOrder = new List<ObjectiveClass>();
    public int currentObjectiveIndex = 0;

    private Label objectiveDisplay;
    [SerializeField] private UIDocument playerUIDocument;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectiveDisplay = playerUIDocument.rootVisualElement.Q<Label>("ObjectiveDisplayText");
        UpdateUI();
    }

    public void UpdateUI()
    {
        objectiveDisplay = playerUIDocument.rootVisualElement.Q<Label>("ObjectiveDisplayText");
        objectiveDisplay.text = objectiveOrder[currentObjectiveIndex].GetDescription();
    }

    public void Progress(int progressAmount)
    {
        if(currentObjectiveIndex > objectiveOrder.Count)
        {
            return;
        }
        
        //laver en instans af objective, med det givne objective spilleren er nået til
        ObjectiveClass objective = objectiveOrder[currentObjectiveIndex];

        //tilfører en exstra værdi til currentProgress-variabel
        objective.currentProgress += progressAmount;

        //kalder IsObjectiveComplete(), som forholder sig til foreskellen mellem current og required progress (og altså derved om spilleren har færdiggjort den)
        if (objective.IsObjectiveComplete())
        {
            NextObjective();
        }

        UpdateUI();
    }

    private void NextObjective()
    {
        currentObjectiveIndex++;

        if(currentObjectiveIndex >= objectiveOrder.Count)
        {
            //You completed the game
            objectiveDisplay.text = "You Win";
            return;
        }

        UpdateUI();
    }
}
