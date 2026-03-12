using UnityEngine;
using UnityEngine.UIElements;

public class DoorKeypad : MonoBehaviour
{
    public string correctCode = "1234";
    public GameObject player; // assign your player GameObject

    private string input = "";
    private UIDocument ui;
    private Button _Keypad;
    private List<Button> _keypadButtons = new List<Button>();
    private Label display;
    private MonoBehaviour[] playerScripts;

    private void Awake()
    {
        _Keypad = _document.rootVisualElement.Q("Keypad") as _Keypad:
        _Keypad.RegisterCallback<ClickEvent>(OnPlayGameClick);

        _keypadButtons = _document.rootVisualElement.Query<_Keypad>(). ToList();
        for (int i = 0; i < _keypadButtons.Count; i++)
        {
            _keypadButtons[i].RegisterCallback<ClickEvent>()
        }

    }
    private void OndDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnPlayGameClick;)
        for (int i = 0; i < _keypadButtons.Count; i++)
        {
            _keypadButtons[i].RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    void Start()
    {
        ui = GetComponent<UIDocument>();
        ui.rootVisualElement.style.display = DisplayStyle.None;

        display = ui.rootVisualElement.Q<Label>("Display");

        // Find alle scripts på player
        if (player != null)
            playerScripts = player.GetComponents<MonoBehaviour>();
    }

    public void OpenUI()
    {
        ui.rootVisualElement.style.display = DisplayStyle.Flex;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        // Deaktiver player scripts
        if (playerScripts != null)
        {
            foreach (var script in playerScripts)
            {
                if (script != this)
                    script.enabled = false;
            }
        }

        Debug.Log("Keypad åbnet. Indtast koden med tastaturet.");
    }

    public void CloseUI()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        // Aktivér player scripts igen
        if (playerScripts != null)
        {
            foreach (var script in playerScripts)
            {
                if (script != this)
                    script.enabled = true;
            }
        }

        ResetInput();

        Debug.Log("Keypad lukket.");
    }

    void Update()
    {
        if (ui.rootVisualElement.style.display == DisplayStyle.Flex)
        {
            // Keyboard input 0-9
            for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
            {
                if (Input.GetKeyDown(key))
                {
                    string number = (key == KeyCode.Alpha0) ? "0" : ((int)key - (int)KeyCode.Alpha0).ToString();
                    OnNumberPressed(number);
                }
            }

            // Escape lukker UI
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseUI();
        }
    }

    void OnNumberPressed(string number)
    {
        if (input.Length >= 4) return;

        input += number;
        display.text = input;

        Debug.Log("Tastetryk registreret: " + number);

        if (input.Length == 4)
            CheckCode();
    }

    void CheckCode()
    {
        if (input == correctCode)
        {
            display.text = "Correct";
            Debug.Log("Døren er åben! Koden er korrekt.");
            Invoke(nameof(CloseUI), 1f);
        }
        else
        {
            display.text = "Wrong";
            Debug.Log("Forkert kode! Prøv igen.");
            Invoke(nameof(ResetInput), 1f);
        }
    }

    void ResetInput()
    {
        input = "";
        if (display != null)
            display.text = "";

        Debug.Log("Input nulstillet.");
    }
}