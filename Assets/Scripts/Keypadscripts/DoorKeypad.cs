using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorKeypad : MonoBehaviour
{
    public string correctCode = "1234";
    public GameObject player;

    private string input = "";
    private UIDocument ui;
    private Label display;
    private List<Button> keypadButtons = new List<Button>();
    private MonoBehaviour[] playerScripts;

    private void Awake()
    {
        ui = GetComponent<UIDocument>();

        if (ui == null)
        {
            return;
        }

        keypadButtons = ui.rootVisualElement.Query<Button>().ToList();

        foreach (Button button in keypadButtons)
        {
            button.RegisterCallback<ClickEvent>(OnButtonClicked);
        }

        display = ui.rootVisualElement.Q<Label>("Display");
    }

    private void OnDisable()
    {
        if (keypadButtons == null) return;

        foreach (Button button in keypadButtons)
        {
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }

    private void Start()
    {
        if (ui != null)
        {
            ui.rootVisualElement.style.display = DisplayStyle.None;
        }

        if (player != null)
        {
            playerScripts = player.GetComponents<MonoBehaviour>();
        }
    }

    public void OpenUI()
    {
        if (ui == null) return;

        ui.rootVisualElement.style.display = DisplayStyle.Flex;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (playerScripts != null)
        {
            foreach (var script in playerScripts)
            {
                if (script != this)
                {
                    script.enabled = false;
                }
            }
        }
    }

    public void CloseUI()
    {
        if (ui == null) return;

        ui.rootVisualElement.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        if (playerScripts != null)
        {
            foreach (var script in playerScripts)
            {
                if (script != this)
                {
                    script.enabled = true;
                }
            }
        }

        ResetInput();
    }

    private void Update()
    {
        if (ui == null) return;

        if (ui.rootVisualElement.style.display.value == DisplayStyle.Flex)
        {
            for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
            {
                if (Input.GetKeyDown(key))
                {
                    string number = ((int)key - (int)KeyCode.Alpha0).ToString();
                    OnNumberPressed(number);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUI();
            }
        }
    }

    private void OnButtonClicked(ClickEvent evt)
    {
        Button button = evt.currentTarget as Button;
        if (button == null) return;

        string value = button.text.Trim();

        if (value == "C")
        {
            ResetInput();
            return;
        }

        if (value == "X" || value.ToLower() == "close")
        {
            CloseUI();
            return;
        }

        if (value == "OK")
        {
            CheckCode();
            return;
        }

        if (value.Length == 1 && char.IsDigit(value[0]))
        {
            OnNumberPressed(value);
        }
    }

    private void OnNumberPressed(string number)
    {
        if (input.Length >= 4) return;

        input += number;

        if (display != null)
        {
            display.text = input;
        }

        if (input.Length == 4)
        {
            CheckCode();
        }
    }

    private void CheckCode()
    {
        if (input == correctCode)
        {
            if (display != null)
            {
                display.text = "Correct";
            }

            Invoke(nameof(CloseUI), 1f);
        }
        else
        {
            if (display != null)
            {
                display.text = "Wrong";
            }

            Invoke(nameof(ResetInput), 1f);
        }
    }

    private void ResetInput()
    {
        input = "";

        if (display != null)
        {
            display.text = "";
        }
    }
}