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
            Time.timeScale = 0;
            optionPanel.OpenPanel();
        }
    }
}
