using UnityEngine;
using UnityEngine.UIElements;

public class DoorKeypad : MonoBehaviour
{
    [SerializeField] private string code = "1234";
    private string input = "";

    private UIDocument ui;
    private MonoBehaviour playerController; // Referencer til FPS-controller script

    void Start()
    {
        ui = GetComponent<UIDocument>();
        if (ui == null)
        {
            Debug.LogError("UIDocument mangler på GameObjectet!");
            return;
        }

        // Find FPS-controller script i scenen (tilpas navnet)
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
            Debug.LogWarning("FPS-controller script ikke fundet. UI input kan blive fanget.");

        var root = ui.rootVisualElement;
        var display = root.Q<Label>("Display");

        // Skjul UI til start
        ui.rootVisualElement.style.display = DisplayStyle.None;

        // Tilføj klik-events til alle knapper
        root.Query<Button>().ForEach(btn =>
        {
            btn.clicked += () =>
            {
                input += btn.text;
                display.text = input;

                if (input.Length == 4)
                {
                    if (input == code)
                    {
                        Debug.Log("Døren er åben!");
                        CloseUI();
                    }
                    else
                    {
                        Debug.Log("Forkert kode");
                        display.text = "Forkert kode";
                        Invoke(nameof(ResetInput), 1f);
                    }
                }
            };
        });
    }

    private void ResetInput()
    {
        input = "";
        var display = ui.rootVisualElement.Q<Label>("Display");
        if (display != null)
            display.text = "";
    }

    public void OpenUI()
    {
        if (ui == null) return;

        ui.rootVisualElement.style.display = DisplayStyle.Flex;

        // Unlock mus og gør den synlig
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        // Deaktiver FPS-controller midlertidigt
        if (playerController != null)
            playerController.enabled = false;
    }

    public void CloseUI()
    {
        if (ui == null) return;

        ui.rootVisualElement.style.display = DisplayStyle.None;

        // Lock mus igen
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        // Re-aktiver FPS-controller
        if (playerController != null)
            playerController.enabled = true;

        ResetInput();
    }
}