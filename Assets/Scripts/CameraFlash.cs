using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFlash : MonoBehaviour
{
    UIDocument cameraUI;

    VisualElement flash;
    VisualElement freezeFrame;
    Image cameraImageUI;
    public Background pictureCapturedTexture;

    [SerializeField] private PlayerLook PlayerLook;
    public bool takingPicture = false;

    [SerializeField] private GameObject flashObject;

    public void ReadyFlash()
    {
        cameraUI = GetComponent<UIDocument>();
        flash = cameraUI.rootVisualElement.Q<VisualElement>("PhotoFlash");
        flash.style.opacity = 0f;

        freezeFrame = cameraUI.rootVisualElement.Q<VisualElement>("Picture");
        freezeFrame.style.opacity = 0f;

        cameraImageUI = cameraUI.rootVisualElement.Q<Image>("cameraUI");
    }

    public void Flash()
    {
        StartCoroutine(StartFlash());
    }
    
    IEnumerator StartFlash()
    {
        takingPicture = true;
        flash.style.opacity = 0f;
        yield return null;

        flash.style.opacity = 100f;

        flashObject.GetComponent<Light>().intensity = 2f;

        yield return new WaitForSeconds(0.05f);

        flash.style.opacity = 0f;

        cameraImageUI.style.opacity = 0f;

        ShowPicture();
    }


    public void ShowPicture()
    {
        freezeFrame.style.backgroundImage = new StyleBackground(pictureCapturedTexture);
        StartCoroutine(FreezeFrameRoutine());
    }

    IEnumerator FreezeFrameRoutine()
    {
        freezeFrame.style.opacity = 100f;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);

        flashObject.GetComponent<Light>().intensity = 0f;

        Time.timeScale = 1f;

        freezeFrame.style.opacity = 0f;

        cameraImageUI.style.opacity = 100f;

        takingPicture = false;
    }

}
