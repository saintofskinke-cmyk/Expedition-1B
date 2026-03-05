using UnityEngine;
using UnityEngine.UIElements;

public class SignText : MonoBehaviour
{
    private VisualElement root;
    private Label label;

    [SerializeField] private string text;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        label = root.Q<Label>("txtSign");

        label.text = text;
    }
}
