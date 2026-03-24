using UnityEngine;

public class LoadManager : MonoBehaviour
{
    [SerializeField] private GameObject[] roomsToUnload;

    private void Start()
    {
        foreach (GameObject room in roomsToUnload)
        {
            room.SetActive(false);
        }
    }
}
