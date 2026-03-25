using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RadioCode : MonoBehaviour
{
    [SerializeField] private List<int> leftCode = new List<int>();
    [SerializeField] private List<int> rightCode = new List<int>();
    bool isRadioPlaying = false;

    [Header("UI")]
    private VisualElement root;
    private Button knobLeft, knobRight, btnExit;

    private void Awake()
    {
        while (leftCode.Count < 3) { leftCode.Add(0); }
        while (rightCode.Count < 3) { rightCode.Add(0); } // Sørger for at listen kun har 3 tal

        root = GetComponent<UIDocument>().rootVisualElement;
        knobLeft = root.Q<Button>("Knob_Left");
        knobRight = root.Q<Button>("Knob_Right");
        btnExit = root.Q<Button>("Exit");
        knobLeft.RegisterCallback<ClickEvent>(OnKnobLeftClicked);
        knobRight.RegisterCallback<ClickEvent>(OnKnobRightClicked);
        btnExit.RegisterCallback<ClickEvent>(OnExitClicked);

        root.Q("RadioUI").style.display = DisplayStyle.None;
    }

    private void OnKnobLeftClicked(ClickEvent evt) { StartCoroutine(PlayCode(isRadioPlaying, leftCode)); }
    private void OnKnobRightClicked(ClickEvent evt) { StartCoroutine(PlayCode(isRadioPlaying, rightCode)); }
    private void OnExitClicked(ClickEvent evt) { 
        root.Q("RadioUI").style.display = DisplayStyle.None;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        GameManager.Instance.pauseAction.action.Enable();
    }


    public void SetCode(int correctTurn, int valveNumber, string valveSide)
    {
        if (valveSide == "Left")
        {
            leftCode[valveNumber - 1] = correctTurn;
        }
        else if (valveSide == "Right")
        {
            rightCode[valveNumber - 1] = correctTurn;
        }
    }

    public IEnumerator PlayCode(bool isPlaying, List<int> valveCode)
    {
        if (!isPlaying)
        {
            isRadioPlaying = true;
            int i = 0;
            foreach (var valveTurn in valveCode)
            {
                AudioSource.PlayClipAtPoint(AudioManager.Instance.radioValveSounds[valveCode[i]], gameObject.transform.position, 1f);
                i++;
                yield return new WaitForSeconds(1f);
            }
            isRadioPlaying = false;
            Debug.Log(isRadioPlaying);
        }
    }
}
