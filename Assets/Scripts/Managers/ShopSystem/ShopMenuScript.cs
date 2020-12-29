using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class WeaponToShow
{
    public GameObject weapon;
    public Constants.WEAPON_TYPE wType;
}

// info for buttons
[Serializable]
public class ButtonInfo
{
    public Button shopButton;
    public Button updateButton;
    public Constants.WEAPON_TYPE wType;
    public Text priceText;
    public float percentDamage;
    public float percentFireRate;
    public float percentClipAmmo;
    public Image damageBar;
    public Image fireRateBar;
    public Image clipBar;
    //unlock
    public Image damageUnlockBar;
    public Image fireRateUnlockBar;
    public Image clipUnlockBar;
    public bool toShop = false; // is this weapon button available to shop (is unlocked?) if false, is a update button    
    public Text descriptionText;
    //audio description
    public AudioClip audioDescription;
}

[Serializable]
public class TabInfo
{
    public Button tabButton;
    public Constants.AMMO_TYPE ammoType;
    public bool selected = false;
    public GameObject panel;
}

public class ShopMenuScript : MonoBehaviour
{
    /// <summary>
    /// Manager that instances the menu Shop system
    /// </summary>

    private AudioSource audioSource;
    public static ShopMenuScript instance;
    public Button shopButton;
    public Button updateButton;
    public Camera camera;
    public GameObject spawnPointInTable; // the point to spawn on the table
    public List<WeaponToShow> weaponsToShow; // the spawned pickables in the table, created from the scriptables
    public bool shopOpen = false;
    public GameObject shopPanel;
    //the list of buttons to update, shop, and info, MUST BE filled in INSPECTOR
    [SerializeField]
    public List<ButtonInfo> buttonInfos = new List<ButtonInfo>();
    [SerializeField]
    public List<TabInfo> tabInfos = new List<TabInfo>();

    //vars set by the TableShop
    public GameObject spawnPoint;
    public int charges = 0; //ammount of weapons that can Spawn this shop, is set by the activated shop
    public TextMeshProUGUI chargesText; // the text of actualCharges
    public TextMeshProUGUI expText; // the text of actual experience
    public Text actualWeaponPrice;
    public PanelInterface infoPanel;

    private ShopTable activeShop;
    private WeaponToShow selectedWeapon;
    private WeaponInfoScriptable selectedWeaponInfo;


    // 
    public Sprite unselectedTabSprite;
    public Sprite selectedTabSprite;
    public Sprite selectedButtonSprite;
    public Sprite unlockButtonSprite;
    public Sprite shopButtonSprite;

    // Use this for initialization

    private Camera playerCamera;
    void Start()
    {
        if (ShopMenuScript.instance == null)
        {
            ShopMenuScript.instance = this;
            GeneratePickables();
            audioSource = GetComponent<AudioSource>();
        } else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Set the status of the menushop in the canvas
    /// </summary>
    /// <param name="state"></param>
    public void SetShopMenu(bool state)
    {
        if (shopPanel)
            shopPanel.SetActive(state);
    }


    /// <summary>
    /// Close the menu shop
    /// </summary>
    public void Close()
    {
        if (playerCamera == null)
        {
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        //playerCamera.enabled = true;
        SetShopMenu(false);
        shopOpen = false;

        UIManager.instance.RestoreWeaponCursor();
    }

    /// <summary>
    /// Open the shop menu from a table/computer
    /// </summary>
    /// <param name="spawnPoint">where spawn the items in the map</param>
    /// <param name="charges">nº of weapons can spawn</param>
    public void OpenShop(GameObject spawnPoint, int charges, ShopTable shopTable)
    {
        if (playerCamera == null)
        {
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
      //  playerCamera.enabled = false;
        selectedWeapon = null;
        selectedWeaponInfo = null;
        foreach (WeaponToShow w in weaponsToShow)
        {
            // hide all
            w.weapon.SetActive(false);
        }
        foreach (ButtonInfo button in buttonInfos)
        {
            if (button.descriptionText) // disable all texts
            {
                button.descriptionText.gameObject.SetActive(false);
            }
        }
        if (infoPanel)
        {
            infoPanel.ClosePanel();
        }
        this.activeShop = shopTable;
        expText.text = ExperienceManager.instance.actualExp.ToString();
        this.charges = charges;
        this.spawnPoint = spawnPoint;
        UpdateButtonStatus();
        UpdateButtonsUI();
        UpdateExpText();
        UpdateChargeText();
        DisableButtons();
        SelectBulletPanel();
        UpdateWeaponsStats();
        SetShopMenu(true);
        shopOpen = true;
        UIManager.instance.SetMenuCursor();
    }


    private void UpdateWeaponsStats()
    {
        foreach (ButtonInfo info in buttonInfos)
        {
            if (info.damageBar) info.damageBar.fillAmount = info.percentDamage / 100f;
            if (info.damageUnlockBar) info.damageUnlockBar.fillAmount = info.percentDamage / 100f;

            if (info.clipBar) info.clipBar.fillAmount = info.percentClipAmmo / 100f;
            if (info.clipUnlockBar) info.clipUnlockBar.fillAmount = info.percentClipAmmo / 100f;


            if (info.fireRateBar) info.fireRateBar.fillAmount = info.percentFireRate / 100f;
            if (info.fireRateUnlockBar) info.fireRateUnlockBar.fillAmount = info.percentFireRate / 100f;

            if (info.priceText)
            {
                foreach (WeaponInfoScriptable wType in WeaponSpawnManager.instance.weaponInfoList)
                {
                    if (info.wType == wType.weaponType)
                    {
                        info.priceText.text = wType.price.ToString() + " XP";
                    }
                }
            }
        }
    }

    private void UpdateChargeText()
    {
        chargesText.text = this.charges.ToString();
    }

    private void UpdateExpText()
    {
        expText.text = ExperienceManager.instance.actualExp.ToString();
    }

    /// <summary>
    /// Buy a weapon, so extract 1 CHARGE from the tableShop, and the new pickable weapon is created in the 
    /// real world, then closes the menu
    /// </summary>
    public void CreateWeapon()
    {
        if (selectedWeapon != null)
        {
            if (charges > 0)
            {
                if (activeShop != null && spawnPoint != null)
                {
                    WeaponSpawnManager.instance.InstantiateWeapon(spawnPoint.transform, true, selectedWeapon.wType, null);
                    charges--;
                    this.activeShop.charges--;
                    UpdateChargeText();
                    if (this.charges <= 0)
                    {
                        shopButton.gameObject.SetActive(false);
                    }
                   // this.Close();
                }
            }
        }
    }


    /// <summary>
    /// Unlock the selected Weapon for the price, if possible
    /// </summary>
    public void UnlockWeapon()
    {
        if (selectedWeapon != null && selectedWeaponInfo != null)
        {
            if (selectedWeaponInfo.price <= ExperienceManager.instance.actualExp)
            {
                // can unlock the weapon
                ExperienceManager.instance.EnableWeapon(selectedWeaponInfo.weaponType);
                ExperienceManager.instance.actualExp -= selectedWeaponInfo.price;
                ExperienceManager.instance.SaveExp();
                ExperienceManager.instance.SaveAvailability(selectedWeaponInfo.weaponType);
                expText.text = ExperienceManager.instance.actualExp.ToString();
                //pass to purchase the actual item:
                if (this.charges > 0)
                {
                    SetToShopButton();
                } else
                {
                    DisableButtons();
                }
                UpdateButtonStatus();
                UpdateButtonsUI();
            }
        }
    }

    ///
    /// Show weapon in menu Logic

    /// <summary>
    ///  Instantiate all the pickables on the table to show, this is just for the menu, so the user can see the weapons
    /// </summary>
    public void GeneratePickables()
    {
        if (weaponsToShow.Count > 0)
        {
            for (int i = 0; i < weaponsToShow.Count; i++)
            {
                Destroy(weaponsToShow[i].weapon);
            }
        }
        weaponsToShow = new List<WeaponToShow>();
        foreach(WeaponInfoScriptable info in WeaponSpawnManager.instance.weaponInfoList)
        {

            WeaponToShow wToShow = new WeaponToShow();
            wToShow.weapon = Instantiate(info.pickableWeaponPrefab, spawnPoint.transform);
            wToShow.weapon.SetActive(false);
            wToShow.wType = info.weaponType;
            weaponsToShow.Add(wToShow);
        }
    }


    /// <summary>
    /// Show the weapon in the table to create it;
    /// </summary>
    /// <param name="wType"></param>
    public void ShowWeapon(int weaponValue)
    {
        Constants.WEAPON_TYPE wType = (Constants.WEAPON_TYPE)weaponValue;
        SelectWeapon(wType);
    }

    private void SelectWeapon(Constants.WEAPON_TYPE wType)
    {
        //clean al descriptions
        foreach (ButtonInfo info in buttonInfos)
        {
            if (info.descriptionText)
            {
                if (info.wType == wType)
                {
                    info.descriptionText.gameObject.SetActive(true);
                } else
                {
                    info.descriptionText.gameObject.SetActive(false);
                }

            }
        }
        foreach (WeaponToShow weapon in weaponsToShow)
        {
            if (weapon.wType == wType)
            {
                weapon.weapon.SetActive(true);
                selectedWeapon = weapon;
            }
            else
            {
                weapon.weapon.SetActive(false);
            }
        }
        foreach(WeaponInfoScriptable info in WeaponSpawnManager.instance.weaponInfoList)
        {
            if (info.weaponType == wType)
            {
                selectedWeaponInfo = info;
            }
        }
    } 

    private void ResetImageButtons()
    {
        foreach(ButtonInfo buttonInfo in buttonInfos)
        {
            buttonInfo.shopButton.GetComponent<Image>().sprite = shopButtonSprite;
            buttonInfo.updateButton.GetComponent<Image>().sprite = unlockButtonSprite;
        }
    }

    /// <summary>
    /// Update the status of all buttons depending if can buy or unlock the weapon
    /// </summary>
    public void UpdateButtonStatus()
    {
        foreach (ButtonInfo buttonInfo in buttonInfos)
        {
            buttonInfo.toShop = ExperienceManager.instance.LoadAvailability(buttonInfo.wType);
            buttonInfo.shopButton.onClick.RemoveAllListeners();
            buttonInfo.updateButton.onClick.RemoveAllListeners();
            buttonInfo.shopButton.onClick.AddListener(() =>
            { //SELECT A WEAPON TO BUY FUNCTION BUTTON
                ResetImageButtons();
                buttonInfo.shopButton.GetComponent<Image>().sprite = selectedButtonSprite;
         
                if (this.charges > 0)
                {
                    SetToShopButton();
                } else
                {
                    DisableButtons();
                }
                SelectWeapon(buttonInfo.wType);
            });

            buttonInfo.updateButton.onClick.AddListener(() =>
            { //SELECT A WEAPON TO UNLOCK FUNCTION BUTTON
                ResetImageButtons();

                buttonInfo.updateButton.GetComponent<Image>().sprite = selectedButtonSprite;
                SelectWeapon(buttonInfo.wType);
                SetToUpdateButton();
            });
        }
    }


    public void SetToShopButton()
    {
        this.shopButton.gameObject.SetActive(true);
        this.updateButton.gameObject.SetActive(false);

    }

    public void SetToUpdateButton()
    {
        if (selectedWeaponInfo != null)
        {
            this.actualWeaponPrice.text = selectedWeaponInfo.price.ToString();
        }
        this.shopButton.gameObject.SetActive(false);
        this.updateButton.gameObject.SetActive(true);

    }
    // disable the shop/unlock buttons
    public void DisableButtons()
    {
        this.shopButton.gameObject.SetActive(false);
        this.updateButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update the buttons depending on the status list
    /// this function Shows or Hide the buttons if they are enabled to shop, to update...
    /// </summary>
    public void UpdateButtonsUI()
    {
        foreach( ButtonInfo buttonInfo in buttonInfos)
        {
            buttonInfo.shopButton.gameObject.SetActive(buttonInfo.toShop);
            buttonInfo.updateButton.gameObject.SetActive(!buttonInfo.toShop);
        }
    }

    //interface for buttons 
    public void SelectTab(Constants.AMMO_TYPE ammoType)
    {
        foreach(TabInfo tab in tabInfos)
        {
            if (ammoType == tab.ammoType)
            {
                tab.panel.SetActive(true);
                tab.selected = true;
                tab.tabButton.GetComponent<Image>().sprite = selectedTabSprite;
                //autoselect the first button of the tab
            } else
            {
                tab.panel.SetActive(false);
                tab.selected = false;
                tab.tabButton.GetComponent<Image>().sprite = unselectedTabSprite;
            }
        }
    }

    public void SelectBulletPanel()
    {
        SelectTab(Constants.AMMO_TYPE.BULLET);
    }
    public void SelectPlasmaPanel()
    {
        SelectTab(Constants.AMMO_TYPE.PLASMA);
    }
    public void SelectLaserPanel()
    {
        SelectTab(Constants.AMMO_TYPE.LASER);
    }
    public void SelectRocketPanel()
    {
        SelectTab(Constants.AMMO_TYPE.ROCKET);
    }

}
