using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PaperInput : MonoBehaviour
{
    public PlayerController controller;
    public PlayerLook playerLook;
    public InputAction exitUI;
    
    private UIDocument paperUIDocument;
    private VisualElement paperUI;
    private bool showingPaper;

    private void UpdateUI()
    {
        paperUIDocument = gameObject.GetComponent<UIDocument>();
        paperUI = paperUIDocument.rootVisualElement.Q<VisualElement>("Paper");
        paperUI.style.display = DisplayStyle.None;
    }

    public void ShowPaperText()
    {
        UpdateUI();
        showingPaper = true;
        paperUI.style.display = DisplayStyle.Flex;
        HidePaper();
    }

    private void HidePaper()
    {
        while (showingPaper)
        {
            controller.enabled = false;
            playerLook.enabled = false;
            
            if(GameManager.Instance.exitUIAction.action.WasPressedThisFrame())
            {
                paperUI.style.display = DisplayStyle.None;
                showingPaper = false;
                controller.enabled = true;
                playerLook.enabled = true;
            }
        }
    }
}
