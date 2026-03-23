using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TypewriterUI : MonoBehaviour
{
    public UIDocument uiDocument;

    private bool showCursor = true;
    private bool finishedTyping = false;

    public string fullText = "Welcome to the world of Unity! This is a typewriter effect demonstration.";
    public float typingSpeed = 0.05f;

    public AudioSource audioSource;
    public AudioClip clickSound;

    private Label textLabel;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        textLabel = root.Q<Label>("IntroText");

        textLabel.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char letter in fullText)
        {
            textLabel.text += letter;

            if (letter != ' ')
            {
                audioSource.PlayOneShot(clickSound);
            }

            if (letter == '.')
                yield return new WaitForSeconds(0.5f);
            else
                yield return new WaitForSeconds(typingSpeed);

            finishedTyping = true;
            StartCoroutine(BlinkCursor());


        }
        IEnumerator BlinkCursor()
        {
            while (true)
            {
                showCursor = !showCursor;

                if (showCursor)
                    textLabel.text = fullText + "|";
                else
                    textLabel.text = fullText;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
