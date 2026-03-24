using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TypewriterUI : MonoBehaviour
{
    public System.Action OnFinished;
    public UIDocument uiDocument;


    private bool showCursor = true;
    private bool finishedTyping = false;

    private string fullText = "WHEN\r\n" +
        "The 7th of January in the geophysical year 1957.\r\n\r\n" +
        "WHERE\r\n" +
        "About 50 miles east of the US McMURDO resarch\r\nstation on antarctica\r\n" +
        "-78,29053262971581,  170,4175091612813.\r\n\r\n" +
        "HOW\r\n" +
        "We don't know...\r\n\r\n" +
        "WHY\r\n" +
        "We don't know that either...\r\n\r\n" +
        "WHO\r\n" +
        "We found an underground cave system under\r\n" +
        "the ice and i found a door that led to an \r\n" +
        "underground USSR research facility.\r\n\r\n\r\n" +
        "OBJECTIVE\r\n" +
        "Discover and document...\r\n" +
        "What were they doing down here?";
    public float typingSpeed = 0.05f;

    public AudioSource audioSource;
    public AudioClip clickSound;

    // Delay (seconds) before hiding the UI after typing completes
    public float hideDelay = 4f;

    // How many characters to reveal per key press (set to 3)
    public int charsPerKey = 3;

    private Label textLabel;
    private Coroutine blinkCoroutine;

    // UIElements root reference and flag set by KeyDown callback
    private VisualElement rootElement;
    private bool nextKeyPressed;

    void Awake()
    {

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument reference is not assigned.");
            enabled = false;
            return;
        }

        rootElement = uiDocument.rootVisualElement;
        textLabel = rootElement.Q<Label>("IntroText");

        // Make root focusable and register keyboard callback so UIElements receives keys.
        rootElement.focusable = true;
        rootElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
        rootElement.Focus();

        textLabel.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        // Clear at start
        textLabel.text = "";

        // Toggle used to play sound every other non-space character
        bool playThisCharSound = false;

        int i = 0;
        int length = fullText.Length;

        while (i < length)
        {
            // Wait until the user presses any key on the UIElements root.
            nextKeyPressed = false;
            yield return new WaitUntil(() => nextKeyPressed);

            // Reveal up to charsPerKey characters for this key press
            for (int c = 0; c < charsPerKey && i < length; c++, i++)
            {
                char letter = fullText[i];
                textLabel.text += letter;

                if (letter != ' ')
                {
                    // Flip the toggle for each non-space char, play only on true
                    playThisCharSound = !playThisCharSound;

                    if (playThisCharSound && audioSource != null && clickSound != null)
                    {
                        audioSource.PlayOneShot(clickSound);
                    }
                }

                // If you still want a tiny delay between characters inside the batch, uncomment:
                // yield return new WaitForSeconds(typingSpeed);
            }
        }

        // Typing finished; set flag and start cursor blink once
        finishedTyping = true;
        blinkCoroutine = StartCoroutine(BlinkCursor());

        // Unregister keyboard callback since we no longer need input
        if (rootElement != null)
            rootElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);

        // Hide UI after configured delay
        StartCoroutine(HideUIAfterDelay());
    }

    void OnKeyDown(KeyDownEvent evt)
    {
        // Accept any key press to advance the text.
        nextKeyPressed = true;

        // Prevent other handlers from also consuming if needed:
        evt.StopImmediatePropagation();
    }

    IEnumerator BlinkCursor()
    {
        // Ensure the label shows the full text first
        textLabel.text = fullText;

        while (true)
        {
            showCursor = !showCursor;

            textLabel.text = fullText + (showCursor ? "|" : "");

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        // Stop blinking coroutine if running
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        // Hide the UI document's root visual element
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            EventManager.StartPlayerEvent();
        }
        else
        {
            // Fallback: disable this GameObject if UIDocument is not assigned
            gameObject.SetActive(false);
        }
        OnFinished?.Invoke();
    }
}
