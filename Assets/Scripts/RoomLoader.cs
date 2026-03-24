using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] private GameObject roomToLoad;
    [SerializeField] private GameObject roomToUnload;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (roomToLoad != null) { roomToLoad.SetActive(true); }

            if (roomToUnload != null) { roomToUnload.SetActive(false); }
        }
    }

    
}
 