using UnityEngine;
using UnityEngine.UIElements;

public class PaperInput : MonoBehaviour
{
    private UIDocument uIDocument;
    private VisualElement paperUI;

    private void UpdateUI()
    {
        uIDocument = gameObject.GetComponent<UIDocument>();
        paperUI = uIDocument.rootVisualElement.Q<VisualElement>("Paper");
        paperUI.style.display = DisplayStyle.None;
    }

    public void ShowPaperText()
    {
        UpdateUI();
        paperUI.style.display = DisplayStyle.Flex;
        StartCoroutine(HidePaper());
    }

    System.Collections.IEnumerator HidePaper()
    {
        yield return new WaitForSeconds(4f);
        paperUI.style.display = DisplayStyle.None;
    }
}
