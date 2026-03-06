using UnityEngine;
using UnityEngine.Animations;

public class GeneratorLever : MonoBehaviour
{
    private Animator leverAnimator;
    
    private void Awake()
    {
        leverAnimator = GetComponent<Animator>();
    }

    
}
