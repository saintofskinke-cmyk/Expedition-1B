using UnityEngine;

public class PhotoTargetOfInterest : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    private bool hasBeenCaptured = false;
    private int progressAmount = 1;
    [SerializeField] private int objectiveEventID = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    public void Captured()
    {
        if (hasBeenCaptured) { return; }

        if(objectiveEventID == questManager.currentObjectiveIndex)
        {
            hasBeenCaptured = true;
            questManager.Progress(progressAmount);
        }
    }
}
