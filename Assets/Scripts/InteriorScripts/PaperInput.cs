using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PaperInput : MonoBehaviour
{
    public PlayerController controller;
    public PlayerLook playerLook;
    public QuestManager questManager;
    public Inventory inventory;
    public InputAction exitUI;
    
    private UIDocument paperUIDocument;
    public UIDocument playerUIDocument;
    private VisualElement paperUI;
    private bool showingPaper = false;

    private void Update()
    {
        PaperUpdate();
    }

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
    }

    private void PaperUpdate()
    {
        if (showingPaper)
        {
            controller.enabled = false;
            playerLook.enabled = false;
            playerUIDocument.enabled = false;
            
            if(GameManager.Instance.interAction.action.WasPressedThisFrame())
            {
                paperUI.style.display = DisplayStyle.None;
                showingPaper = false;
                controller.enabled = true;
                playerLook.enabled = true;

                playerUIDocument.enabled = true;

                inventory.UpdatePlayerHud();
                questManager.UpdateUI();
                playerLook.UpdateTextUI();
            }
        }
    }
}
