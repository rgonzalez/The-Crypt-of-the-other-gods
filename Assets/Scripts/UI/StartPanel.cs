using UnityEngine;
using System.Collections;

public class StartPanel : MonoBehaviour
{
    public GameObject panel;
    // Use this for initialization

    public void OpenPanel()
    {
        panel.SetActive(true);
        UIManager.instance.SetMenuCursor();
        ShopMenuScript.instance.shopOpen = true;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        UIManager.instance.RestoreWeaponCursor();
        ShopMenuScript.instance.shopOpen = false;
    }
}
