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
    public bool toShop = false; // is this weapon button available to shop (is unlocked?) if false, is a update button    
}

public class ShopMenuScript : MonoBehaviour
{
    /// <summary>
    /// Manager that instances the menu Shop system
    /// </summary>

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

    //vars set by the TableShop
    public GameObject spawnPoint;
    public int charges = 0; //ammount of weapons that can Spawn this shop, is set by the activated shop
    public TextMeshProUGUI chargesText; // the text of actualCharges
    public TextMeshProUGUI expText; // the text of actual experience
    public Text actualWeaponPrice; 


    private ShopTable activeShop;
    private WeaponToShow selectedWeapon;
    private WeaponInfoScriptable selectedWeaponInfo;



    // Use this for initialization
    void Start()
    {
        if (ShopMenuScript.instance == null)
        {
            ShopMenuScript.instance = this;
            GeneratePickables();
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
        SetShopMenu(false);
    }

    /// <summary>
    /// Open the shop menu from a table/computer
    /// </summary>
    /// <param name="spawnPoint">where spawn the items in the map</param>
    /// <param name="charges">nº of weapons can spawn</param>
    public void OpenShop(GameObject spawnPoint, int charges, ShopTable shopTable)
    {
        selectedWeapon = null;
        selectedWeaponInfo = null;
        foreach (WeaponToShow w in weaponsToShow)
        {
            // hide all
            w.weapon.SetActive(false);
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
        SetShopMenu(true);
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
            if (selectedWeaponInfo.price < ExperienceManager.instance.actualExp)
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
                SetToUpdateButton();
                SelectWeapon(buttonInfo.wType);
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
}
