using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    PlayerController playerController;
    PlayerLook playerLook;

    private VisualElement root;
    private VisualElement playerHudRoot;
    private Button continueButton, menuButton;
    private Slider mouseSensSlider;
    [SerializeField] private GameObject mainCameraParent;
    [HideInInspector] public bool isGamePaused;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerLook = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerLook>();

        root = GetComponent<UIDocument>().rootVisualElement;
        playerHudRoot = mainCameraParent.GetComponent<UIDocument>().rootVisualElement;

        continueButton = root.Q<Button>("btnContinue");
        menuButton = root.Q<Button>("btnMainMenu");
        mouseSensSlider = root.Q<Slider>("MouseSensitivity");

        continueButton.RegisterCallback<ClickEvent>(OnContinueClicked);
        menuButton.RegisterCallback<ClickEvent>(OnMenuClicked);

        root.style.display = DisplayStyle.None;
        UpdateSettings();
    }

    private void Update()
    {
        // Open & Close the Pause Menu when ESC is pressed
        if (GameManager.Instance.pauseAction.action.WasPressedThisFrame() && !playerController.inPhotoMode)
        {
            if (!isGamePaused)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                playerHudRoot = mainCameraParent.GetComponent<UIDocument>().rootVisualElement;
                playerHudRoot.style.display = DisplayStyle.None;

                root.style.display = DisplayStyle.Flex;
                Time.timeScale = 0;

            }
            else
            {
                Time.timeScale = 1;
                root.style.display = DisplayStyle.None;

                playerHudRoot = mainCameraParent.GetComponent<UIDocument>().rootVisualElement;
                playerHudRoot.style.display = DisplayStyle.Flex;

                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
            isGamePaused = !isGamePaused;
            UpdateSettings();
        }
    }

    private void OnContinueClicked(ClickEvent evt)
    {
        Time.timeScale = 1;
        isGamePaused = false;
        UpdateSettings();

        root.style.display = DisplayStyle.None;
        playerHudRoot.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OnMenuClicked(ClickEvent evt)
    {
        Debug.Log("Go back to Main Menu");
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        playerLook.mouseSens = mouseSensSlider.value * 0.25f + 1; // Sætter mouse sensitivity til sliderens værdi
    }
}
