using UnityEngine;

public class LeaveZoneEvent : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    private int progressAmount = 1;
    private bool alreadyCompleted = false;
    [SerializeField] private int objectiveEventID = 0;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (objectiveEventID == questManager.currentObjectiveIndex && !alreadyCompleted)
            {
                questManager.Progress(progressAmount);
                alreadyCompleted = true;
            }
        }
    }
}
