using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private UIDocument uiDocument;

    private VisualElement root;
    public VisualElement flareCooldown;
    private Label flareCountLabel;

    [Header("Inventory Data")]
    public int flareCount = 10;

    public bool IsUiReady { get; private set; }

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        SetupUI();
        UpdateUI();
    }

    private void SetupUI()
    {
        if (uiDocument == null)
        {
            Debug.LogError("Inventory: UIDocument mangler på objektet.");
            IsUiReady = false;
            return;
        }

        root = uiDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("Inventory: rootVisualElement er null. Tjek at UIDocument har en Visual Tree Asset assigned.");
            IsUiReady = false;
            return;
        }

        flareCooldown = root.Q<VisualElement>("flareCooldown");
        flareCountLabel = root.Q<Label>("txtFlareCount");

        if (flareCooldown == null)
        {
            Debug.LogWarning("Inventory: Kunne ikke finde 'flareCooldown' i UXML.");
        }

        if (flareCountLabel == null)
        {
            Debug.LogWarning("Inventory: Kunne ikke finde 'txtFlareCount' i UXML.");
        }

        IsUiReady = true;
    }

    private void UpdateUI()
    {
        if (!IsUiReady)
            return;

        if (flareCountLabel != null)
            flareCountLabel.text = flareCount.ToString();
    }

    public void AddItem(string item)
    {
        switch (item)
        {
            case "Flare":
                flareCount++;
                UpdateUI();
                break;

            default:
                Debug.LogWarning("Inventory: Unknown item: " + item);
                break;
        }
    }

    public void RemoveItem(string item)
    {
        switch (item)
        {
            case "Flare":
                if (flareCount > 0)
                {
                    flareCount--;
                    UpdateUI();
                }
                break;

            default:
                Debug.LogWarning("Inventory: Unknown item: " + item);
                break;
        }
    }
}
