using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioCode : MonoBehaviour
{
    [SerializeField] private List<int> rightCode = new List<int>();

    private void Awake()
    {
        while (rightCode.Count < 3) { rightCode.Add(0); } // Sørger for at listen kun har 3 tal
    }

    public void SetCode(int correctTurn, int valveNumber, string valveSide)
    {
        if (valveSide == "Right")
        {
            rightCode[valveNumber - 1] = correctTurn;
        }
    }

    public IEnumerator PlayCode(bool isPlaying)
    {
        if (!isPlaying)
        {
            int i = 0;
            foreach (var valveTurn in rightCode)
            {
                AudioSource.PlayClipAtPoint(AudioManager.Instance.radioValveSounds[rightCode[i]], gameObject.transform.position, 1f);
                i++;
                yield return new WaitForSeconds(1f);
            }
            Debug.Log("Done");
        }
    }
}
