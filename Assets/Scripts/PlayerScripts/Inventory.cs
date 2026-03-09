using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    [Header("UI Elements")]
    private VisualElement root;
    public GameObject playerHudDocument;
    public VisualElement flareCooldown;
    private Label flareCountLabel;

    [Header("Inventory Data")]
    public int flareCount = 10;



    private void Awake()
    {
        UpdatePlayerHud();
    }

    public void UpdatePlayerHud()
    {
        root = playerHudDocument.GetComponent<UIDocument>().rootVisualElement;
        flareCooldown = root.Q("flareCooldown");
        flareCountLabel = root.Q<Label>("txtFlareCount");
        flareCountLabel.text = flareCount.ToString();
        
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
