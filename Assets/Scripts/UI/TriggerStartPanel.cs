using UnityEngine;
using System.Collections;

public class TriggerStartPanel : MonoBehaviour
{

    public bool active = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER) && active)
        {
            if (UIManager.instance && UIManager.instance.panelStart)
            {
                UIManager.instance.panelStart.OpenPanel();
                active = false;
            }
        }
    }
}
