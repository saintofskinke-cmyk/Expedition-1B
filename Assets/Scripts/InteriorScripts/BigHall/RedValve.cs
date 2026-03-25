using UnityEngine;

public class RedValve : MonoBehaviour
{
    public int correctValveTurn;
    [SerializeField] private int valveNumber;
    [SerializeField] private string valveSide;
    [SerializeField] private GameObject radio;

    private void Awake()
    {
        correctValveTurn = Random.Range(0, 8);
        radio.GetComponent<RadioCode>().SetCode(correctValveTurn, valveNumber, valveSide);
    }
}
