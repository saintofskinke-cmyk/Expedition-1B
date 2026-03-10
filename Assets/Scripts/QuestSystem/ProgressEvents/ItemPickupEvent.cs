using Unity.VisualScripting;
using UnityEngine;

public class ItemPickupEvent : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private int progressAmount = 1;
    [SerializeField] private int objectiveEventID = 0;
    private bool alreadyCollected = false;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    public void OnPickup()
    {
        if (alreadyCollected) {  return; }

        if(objectiveEventID == questManager.currentObjectiveIndex)
        {
            alreadyCollected = true;
            questManager.Progress(progressAmount);
        }
    }
}
