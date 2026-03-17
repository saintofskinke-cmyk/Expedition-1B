using UnityEngine;

public class GenStart : MonoBehaviour
{
    private AudioSource audioSourceGen;
    public AudioClip genSound;
    [SerializeField] private bool isLooping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioSourceGen = GetComponent<AudioSource>();
    }

    public void PlayGenSound()
    {
        if (isLooping)
        {
            audioSourceGen.loop = true;
            audioSourceGen.PlayOneShot(genSound);
        }
        else
        {
            audioSourceGen.loop = false;
            audioSourceGen.PlayOneShot(genSound);
        }
    }
}
