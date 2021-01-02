using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{

    public PanelInterface optionPanel;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionPanel.panel.active)
            {
                //is active
                Time.timeScale = 1;
                optionPanel.ClosePanel();
            }
            else
            {
                Time.timeScale = 0;
                optionPanel.OpenPanel();
            }
        }
    }
}
