using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : MonoBehaviour
{
    public List<ObjectiveClass> objectiveOrder = new List<ObjectiveClass>();
    public int currentObjectiveIndex = 0;

    private bool allObjectiveComplete;

    private Label objectiveDisplay;
    [SerializeField] private UIDocument playerUIDocument;
    [SerializeField] private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager.StartPlayer += StartQuestSystem;
    }

    private void StartQuestSystem()
    {
        objectiveDisplay = playerUIDocument.rootVisualElement.Q<Label>("ObjectiveDisplayText");
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (!allObjectiveComplete) {
            objectiveDisplay = playerUIDocument.rootVisualElement.Q<Label>("ObjectiveDisplayText");
            objectiveDisplay.text = objectiveOrder[currentObjectiveIndex].GetDescription();
        }
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

        if (!playerController.inPhotoMode)
        {
            UpdateUI();
        }
    }

    private void NextObjective()
    {
        currentObjectiveIndex++;

        if(currentObjectiveIndex >= objectiveOrder.Count)
        {
            //You completed the game
            objectiveDisplay.text = "Go Trough The Door";
            allObjectiveComplete = true;
            return;
        }

        if (!playerController.inPhotoMode)
        {
            UpdateUI();
        }
    }
}
