using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("UI Elements")]
    public GameObject playerHudDocument;
    private VisualElement root;
    public VisualElement flareCooldown;
    public VisualElement staminaBar;
    private Label flareCountLabel;

    [Header("Inventory Data")]
    public int flareCount = 10;



    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else {  Instance = this; }

        UpdatePlayerHud();
    }

    public void UpdatePlayerHud()
    {
        root = playerHudDocument.GetComponent<UIDocument>().rootVisualElement;
        flareCooldown = root.Q("flareCooldown");
        flareCountLabel = root.Q<Label>("txtFlareCount");
        flareCountLabel.text = flareCount.ToString();
        staminaBar = root.Q("StaminaBar");
    }

    public void AddItem(string item)
    {

        switch (item)
        {
            case "Flare":
                flareCount++;
                flareCountLabel.text = flareCount.ToString();
                break;
            default:
                Debug.LogWarning("Unknown item: " + item);
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
                    flareCountLabel.text = flareCount.ToString();
                }
                break;
            default:
                Debug.LogWarning("Unknown item: " + item);
                break;
        }
    }
}
