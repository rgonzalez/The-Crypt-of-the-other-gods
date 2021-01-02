using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class TriggerPanel : MonoBehaviour
{
    public PanelInterface panel;
    public bool panelByUIManager = false; //only for maincanvas
    public bool active = true; //is active by default?
    public bool reusable = false; // must be disabled or can enter multiple times
    // Start is called before the first frame update
 

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            Debug.Log("touch player " + panel);
            if (panel)
            {

                Debug.Log("panel" + this.name);
                if (active)
                {
                    Debug.Log("active" + this.name);
                    Time.timeScale = 0;
                    panel.OpenPanel();
                    Debug.Log("opened" + this.name);
                    if (!reusable)
                    {
                        Debug.Log("disable " + this.name);
                        //if not reusable, just disable
                        active = false;
                    }
                }

            }
        }
    }

}
