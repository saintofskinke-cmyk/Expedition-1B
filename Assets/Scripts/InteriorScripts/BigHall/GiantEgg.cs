using UnityEngine;

public class GiantEgg : MonoBehaviour
{
    private AudioSource audSource;
    private Animator animator;
    [SerializeField] private int eggID;

    private void Start()
    {
        audSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        switch (eggID)
        {
            case 0: StartAnimation(); break;
            case 1: Invoke("StartAnimation", 1.9f); break;
            case 2: Invoke("StartAnimation", 0.57f); break;
        }
    }

    private void StartAnimation()
    { animator.enabled = true; }

    public void GiantEggHeartBeat()
    { audSource.Play(); }
}
