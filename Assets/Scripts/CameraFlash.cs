using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFlash : MonoBehaviour
{
    UIDocument cameraUI;

    VisualElement flash;

    public void ReadyFlash()
    {
        cameraUI = GetComponent<UIDocument>();
        flash = cameraUI.rootVisualElement.Q<VisualElement>("PhotoFlash");
        flash.style.opacity = 0f;
    }

    public void Flash()
    {
        StartCoroutine(StartFlash());
    }
    
    IEnumerator StartFlash()
    {
        flash.style.opacity = 0f;
        yield return null;

        flash.style.opacity = 100f;

        yield return new WaitForSeconds(0.05f);

        flash.style.opacity = 0f;
    }

}
