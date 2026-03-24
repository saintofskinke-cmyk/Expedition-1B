using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public static event Action Generator;
    public static void StartGenerator() { Generator?.Invoke(); }
}
