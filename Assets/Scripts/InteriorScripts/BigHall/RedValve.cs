using UnityEngine;

public class RedValve : MonoBehaviour
{
    public int correctValveTurn;
    [SerializeField] private int valveNumber;
    [SerializeField] private string valveSide;
    [SerializeField] private GameObject radio;

    private void Start()
    {
        correctValveTurn = Random.Range(0, 8);
        if (valveSide == "Right") { radio.GetComponent<RadioCode>().SetCode(correctValveTurn, valveNumber, valveSide); }
    }
}
