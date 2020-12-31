using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelInterface : MonoBehaviour
{

    public GameObject panel;
    public bool startClosed = true;
    public bool disableMainCanvas = false; // check to disable the main canvas: only in tutorial
    public GameObject otherCanvas = null; // if must disable other canvas

    private void Start()
    {
        if (startClosed && panel)
        {
            panel.SetActive(false);
        }
        if (disableMainCanvas)
        {
            
        }
    }
    public void OpenPanel()
    {
        StartCoroutine(OpenPanelRoutine());
    }
    
    private IEnumerator OpenPanelRoutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (UIManager.instance)
        {
            UIManager.instance.SetMenuCursor();
        }
        if (disableMainCanvas)
        {
            Debug.Log("disable canvas1");
            otherCanvas = GameObject.FindGameObjectWithTag(Constants.TAG_CANVAS);
            if (otherCanvas)
            {
                Debug.Log("disable canvas2");
                otherCanvas.SetActive(false);
            }
        }
        panel.SetActive(true);
    }
    public void ClosePanel()
    {
        Debug.Log("closing panel");
        if (UIManager.instance)
        {
            UIManager.instance.RestoreWeaponCursor();
        }

        if (otherCanvas)
        {
            otherCanvas.SetActive(true);
        }
        Debug.Log("time scale");
        Time.timeScale = 1;

        Debug.Log("panel false");
        panel.SetActive(false);
        Debug.Log("closed panel");
    }

    public void AlternateState()
    {
        panel.SetActive(!panel.active);
    }
}
