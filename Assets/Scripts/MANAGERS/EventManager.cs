using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public static event Action Generator;
    public static void StartGenerator() { Generator?.Invoke(); }


    public static event Action StartPlayer;
    public static void StartPlayerEvent() { StartPlayer?.Invoke(); }
}
