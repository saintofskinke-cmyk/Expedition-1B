using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    PlayerController playerController;
    private VisualElement root;
    private VisualElement playerHudRoot;
    [SerializeField] private GameObject mainCameraParent;
    private Button continueButton, settingsButton, menuButton;
    [SerializeField] private InputActionReference pauseAction;
    [HideInInspector] public bool isGamePaused;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        root = GetComponent<UIDocument>().rootVisualElement;
        playerHudRoot = mainCameraParent.GetComponent<UIDocument>().rootVisualElement;
        continueButton = root.Q<Button>("btnContinue");
        settingsButton = root.Q<Button>("btnSettings");
        menuButton = root.Q<Button>("btnMainMenu");
        continueButton.RegisterCallback<ClickEvent>(OnContinueClicked);
        settingsButton.RegisterCallback<ClickEvent>(OnSettingsClicked);
        menuButton.RegisterCallback<ClickEvent>(OnMenuClicked);
        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        // Open & Close the Pause Menu when ESC is pressed
        if (pauseAction.action.WasPressedThisFrame() && !playerController.inPhotoMode)
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
        }
    }

    private void OnContinueClicked(ClickEvent evt)
    {
        Time.timeScale = 1;
        isGamePaused = false;
        root.style.display = DisplayStyle.None;
        playerHudRoot.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void OnSettingsClicked(ClickEvent evt)
    {
        Debug.Log("Settings button clicked");
    }

    private void OnMenuClicked(ClickEvent evt)
    {
        Debug.Log("Go back to Main Menu");
    }
}
