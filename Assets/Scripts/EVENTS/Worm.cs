using System.Collections;
using UnityEngine;

public class Worm : MonoBehaviour
{
    Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
        EventManager.Worm += MoveWorm;
    }

    public void MoveWorm() => anim.SetTrigger("SWIM");
    public void BONK() => anim.SetTrigger("BONK");
}
