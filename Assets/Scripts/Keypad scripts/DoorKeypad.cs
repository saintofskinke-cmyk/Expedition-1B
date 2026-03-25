using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorKeypad : MonoBehaviour
{
    public string correctCode = "1957";

    public UIDocument ui;

    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;   // ← NY

    public Animator doorAnimator;
    public string openTriggerName = "Activate"; // ------- Unused?

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
        EventManager.Generator += TurnOnKeypad; // This will add the method to the Generator event. That means the method will be activated when this event is happening.
    }

    private void TurnOnKeypad()
    {
        tag = "Interactable";
    }

    private void OnDisable()
    {
        EventManager.Generator -= TurnOnKeypad; // This will remove the object from the event, so it doesn't accidentually cause an error if the event is activated while the object is not active.
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

            if (doorAnimator != null)
            {
                doorAnimator.SetBool("Activate", true);
                AudioSource.PlayClipAtPoint(AudioManager.Instance.metalDoorUnlock, doorAnimator.gameObject.transform.position);
            }
            else
            {
                Debug.LogError("Door Animator mangler på DoorKeypad!");
            }

            Invoke(nameof(CloseUI), 1f);
        }
        else
        {
            display.text = "Wrong";
            Invoke(nameof(ResetInput), 1f);
        }
    }
    private void Update()
    {
        if (ui.rootVisualElement.style.display == DisplayStyle.Flex)
        {
            if (GameManager.Instance.pauseAction.action.WasPressedThisFrame())
            {
                CloseUI();
            }
        }
    }
    private void ResetInput()
    {
        input = "";
        display.text = "";
    }
}   