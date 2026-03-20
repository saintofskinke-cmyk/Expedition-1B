using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorKeypad : MonoBehaviour
{
    public string correctCode = "1234";

    public UIDocument ui;

    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;   // ← NY

    private string input = "";
    private Label display;
    private List<Button> keypadButtons = new List<Button>();

    private void Awake()
    {
        ui = GetComponent<UIDocument>();

        keypadButtons = ui.rootVisualElement.Query<Button>().ToList();

        foreach (Button button in keypadButtons)
        {
            button.RegisterCallback<ClickEvent>(OnButtonClicked);
        }

        display = ui.rootVisualElement.Q<Label>("Display");
    }

    private void Start()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void OpenUI()
    {
        ui.rootVisualElement.style.display = DisplayStyle.Flex;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;
    }

    public void CloseUI()
    {
        ui.rootVisualElement.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        ResetInput();
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

        if (value == "X")
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
            display.text = input;

        if (input.Length == 4)
            CheckCode();
    }

    private void CheckCode()
    {
        if (input == correctCode)
        {
            display.text = "Correct";
            Invoke(nameof(CloseUI), 1f);
        }
        else
        {
            display.text = "Wrong";
            Invoke(nameof(ResetInput), 1f);
        }
    }

    private void ResetInput()
    {
        input = "";
        display.text = "";
    }
}