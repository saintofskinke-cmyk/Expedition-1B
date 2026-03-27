using UnityEngine;

public class AreaSounds : MonoBehaviour
{
    [SerializeField] private bool ActivateOnEvent;
    private void Start()
    {
        if (ActivateOnEvent) { 
            EventManager.Generator += ActivateCollider;
            GetComponent<Collider>().enabled = false;
        }
        
    }
    private void ActivateCollider() { GetComponent<Collider>().enabled = true; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { GetComponent<AudioSource>().Play(); }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") { GetComponent<AudioSource>().Stop(); }
    }
}
