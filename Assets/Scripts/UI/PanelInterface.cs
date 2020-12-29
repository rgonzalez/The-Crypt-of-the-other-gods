using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelInterface : MonoBehaviour
{

    public GameObject panel;
    public bool startClosed = true;


    private void Start()
    {
        if (startClosed && panel)
        {
            panel.SetActive(false);
        }
    }
    public void OpenPanel()
    {
        panel.SetActive(true);
    }
    
    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void AlternateState()
    {
        panel.SetActive(!panel.active);
    }
}
