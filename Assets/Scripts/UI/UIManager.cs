﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammo
{
    public int ammo = 0;
    public AmmoInfoScriptable info;
    public Text text; //the textcomponent with the ammo info (general TOP LEFT)
    public Text equipedAmmoText; // The equiped ammo (BOTTOM RIGHT)
    public GameObject ammoUIEquiped; // the actual instance of the equiped ammo
    public AbstractUIAmmo ammoUI; // the ammoUI script that keeps the clip icon/animations
    public GameObject ammoIcon; // the actual instance of the ammo little icon
}

/// <summary>
/// Manager to update the interface system
/// </summary>
public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    /// <summary>
    /// HEALTH CONFIGURATION
    /// </summary>
    public int maxHealth = 100; // set by Health player script
    public int actualHealth = 100;

    public bool heartMode = false; //set the hearts or bar
    public Image healthBar; // the red health bar
    public GameObject healthBarPanel; // the panel with the health red bar
    [SerializeField]
    private int heartHealth = 20; // how mucho health is a UI Heart
    [SerializeField]
    private GameObject healthPanel; //the GameObjectPanelHealth that is filled with Hearts
    [SerializeField]
    private GameObject heartPrefab; // the Heart, is a sprite with 2 layers (red and gray)
    [SerializeField]
    private Image equipedWeaponIcon; // the bottom right icon of weapon equiped
    [SerializeField]
    private GameObject deadPanel;

    /// <summary>
    /// AMMO CONFIGURATION
    /// </summary>
    /// 
    private List<Ammo> ammoList = new List<Ammo>();
    [SerializeField]
    private GameObject allAmmoPanel;
    [SerializeField]
    private GameObject equipedAmmoPanel;
    private Ammo actualAmmo; // the actual Ammo equiped


    /// <summary>
    /// Centered info in screen
    /// </summary>
    /// 
    [SerializeField]
    private Text centeredText;
    [SerializeField]
    private Image centeredImage;
    public GameObject centralPanel;
    public Text keyEquiped; // the text with nº of keys in the inventory


    //cursor
    public Texture2D cursorMenu;
    private Texture2D actualCursor; // the last weapon used
    

    // Start is called before the first frame update
    void Start()
    {
        if (UIManager.instance == null)
        {
            UIManager.instance = this;
            if (heartMode)
            {
                healthPanel.SetActive(true);
                healthBarPanel.SetActive(false);
            } else
            {
                healthPanel.SetActive(false);
                healthBarPanel.SetActive(true);
            }
            // Start Health Config
            ConfigureMaxHealth();
            ConfigureHealth();
            PrepareAmmoUI();
            if (centralPanel)
            {
                centralPanel.SetActive(false);
            }

            //Ammo Config
        } else
        {
            Destroy(this);
        }
    }


    public void SetMenuCursor()
    {
        Cursor.SetCursor(cursorMenu, Vector2.zero, CursorMode.Auto);
    }

    public void RestoreWeaponCursor()
    {
        if (actualCursor)
        {
            Cursor.SetCursor(actualCursor, new Vector2(actualCursor.width / 2, actualCursor.height / 2), CursorMode.Auto);
        } else
        {
            //set default
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    #region HEALTH
    /// <summary>
    /// Set the max Health image, NOTE: this doesnt adjust the real actual life aspect
    /// </summary>
    public void ConfigureMaxHealth()
    {
        if (heartMode)
        {
            // clean the panel of hearts
            for (int i = 0; i < healthPanel.transform.childCount; i++)
            {
                Destroy(healthPanel.transform.GetChild(i));
            }
            //now Clear, set the max Health 
            int numHearts = maxHealth / heartHealth + (maxHealth % heartHealth); // just in case the division is not mod 0
            for (int i = 0; i < numHearts; i++)
            {
                GameObject heart = Instantiate(heartPrefab, healthPanel.transform);
                //the heart contains an image that can be filled depending the actual Health          
            }
        } else
        {
            //health bar
            healthBar.fillAmount = 1;
        }
    }

    public void ConfigureHealth()
    {
        if (heartMode)
        {
            bool beatActive = false;
            for (int i = 0; i < healthPanel.transform.childCount; i++)
            {
                GameObject heart = healthPanel.transform.GetChild(i).GetChild(0).gameObject;
                if (actualHealth >= ((i + 1) * heartHealth))
                {
                    //there is more than this heart of life
                    heart.GetComponent<Image>().fillAmount = 1;

                    heart.transform.parent.gameObject.GetComponent<HeartBeat>().isActive = false;
                }
                else
                {
                    int leftHeart = (actualHealth - (i * heartHealth));
                    heart.GetComponent<Image>().fillAmount = (float)leftHeart / (float)heartHealth; // set the % of the heart image
                    if (leftHeart > 0)
                    {
                        heart.transform.parent.gameObject.GetComponent<HeartBeat>().isActive = true;
                        beatActive = true;
                    }
                    else
                    {
                        heart.transform.parent.gameObject.GetComponent<HeartBeat>().isActive = false;
                    }
                }
            }
            if (!beatActive)
            {
                healthPanel.transform.GetChild(healthPanel.transform.childCount - 1).GetComponent<HeartBeat>().isActive = true;
            }
        } else
        {
            healthBar.fillAmount = (float)actualHealth / (float)maxHealth;
        }
    }

    public void ShowDeadPanel()
    {
        if (deadPanel)
        {
            SetMenuCursor();
            deadPanel.SetActive(true);
        }
    }
    #endregion HEALTH

    #region AMMO
    /// <summary>
    /// Prepare all the UI for the posible ammos
    /// </summary>
    public void PrepareAmmoUI()
    {
        foreach (Constants.AMMO_TYPE ammoType in (Constants.AMMO_TYPE[])Enum.GetValues(typeof(Constants.AMMO_TYPE)))
        {
            Ammo ammoInfo = new Ammo();
            if (WeaponSpawnManager.instance)
            {
                AmmoInfoScriptable ammoScriptableInfo = WeaponSpawnManager.instance.GetAmmoInfo(ammoType);
                if (ammoScriptableInfo)
                {
                    ammoInfo.info = ammoScriptableInfo;
                    // create the instances of the icon and equiped ammo in the canvas
                    ammoInfo.ammoIcon = Instantiate(ammoInfo.info.ammoIcon, allAmmoPanel.transform);
                    ammoInfo.text = ammoInfo.ammoIcon.transform.GetChild(0).GetComponent<Text>();
                    ammoInfo.text.text = InventoryManager.instance.GetAvailableAmmo(ammoInfo.info.ammoType).ToString();
                    ammoInfo.ammoUIEquiped = Instantiate(ammoInfo.info.ammoUIPrefab, equipedAmmoPanel.transform);
                    ammoInfo.equipedAmmoText = ammoInfo.ammoUIEquiped.GetComponent<Text>();
                    ammoInfo.ammoUI = ammoInfo.ammoUIEquiped.GetComponent<AbstractUIAmmo>();
                    ammoInfo.ammoUIEquiped.SetActive(false); //disable the equiped ammo, later active the actual Weapon
                    ammoList.Add(ammoInfo);
                }
            }
        }
    }
    /// <summary>
    /// Active the UI of the ammo equiped
    /// </summary>
    /// <param name="ammoType">The ammo equiped in this moment</param>
    public void EquipAmmo(Constants.AMMO_TYPE ammoType)
    {
        foreach(Ammo ammo in ammoList)
        {
            if (ammo.info.ammoType == ammoType)
            {
                ammo.ammoUIEquiped.SetActive(true);
                actualAmmo = ammo;
            } else
            {
                ammo.ammoUIEquiped.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Update the top/left ammo info panel (the inventory)
    /// </summary>
    public void UpdateAmmoBagInfo(Constants.AMMO_TYPE ammoType, int ammount )
    {
        foreach(Ammo ammo in ammoList)
        {
            if (ammo.info.ammoType == ammoType)
            {
                ammo.text.text = ammount.ToString();
            }
        }
    }

    /// <summary>
    /// Set to the equiped ammo display, start shoot animation
    /// </summary>
    /// <param name="ammoType">the kind of ammo used</param>
    /// <param name="ammountWasted">the ammount to extract</param>
    /// <param name="ammount">How much left in the clip</param>

    public void Shoot(Constants.AMMO_TYPE ammoType, int ammountWasted, int ammountLeft)
    {
        foreach(Ammo ammo in ammoList)
        {
            if (ammo.info.ammoType == ammoType)
            {
                ammo.equipedAmmoText.text = ammountLeft.ToString(); // update the ammount in the clip info
                if (ammountWasted > 0) 
                    ammo.ammoUI.Shoot(ammountWasted);
            }
        }
    }

    public void ReloadAmmo(Constants.AMMO_TYPE ammoType,  int maxClip, int actualAmmo, int perfectAmmo)
    {
        foreach (Ammo ammo in ammoList)
        {
            if (ammo.info.ammoType == ammoType)
            {
                ammo.equipedAmmoText.text = actualAmmo.ToString(); // update the ammount in the clip info
                ammo.ammoUI.Reload(perfectAmmo, actualAmmo, maxClip);
            }
        }
    }

    #endregion AMMO

    #region WEAPON
    /// <summary>
    /// Show in the actual equip weapon the icon
    /// </summary>
    /// <param name="weaponType"></param>
    public void SetEquipedWeapon(Constants.WEAPON_TYPE weaponType)
    {
        WeaponInfoScriptable info =  WeaponSpawnManager.instance.GetWeaponInfo(weaponType);
        if (info && equipedWeaponIcon)
        {
            equipedWeaponIcon.sprite = info.equipedWeaponIcon;
        }
        if (info && info.cursor)
        {
            Cursor.SetCursor(info.cursor, new Vector2(info.cursor.width / 2, info.cursor.height / 2), CursorMode.Auto);
            actualCursor = info.cursor;
        }
    }

    /// <summary>
    /// Show a pickable weapon info in the center of screen
    /// </summary>
    /// <param name="weaponType"></param>
    public void ShowWeaponInfo(Constants.WEAPON_TYPE weaponType)
    {
        if (WeaponSpawnManager.instance)
        {
            if (centralPanel)
            {
                centralPanel.SetActive(true);
            }
            WeaponInfoScriptable info = WeaponSpawnManager.instance.GetWeaponInfo(weaponType);
            if (info)
            {
                if (info.equipedWeaponIcon)
                {
                    centeredImage.sprite = info.equipedWeaponIcon;
                    centeredImage.preserveAspect = true;
                    centeredImage.gameObject.SetActive(true);
                }
                if (info.prefabName != null)
                {
                    centeredText.text = info.prefabName;
                }
            }
        }
    }

 
    public void ClearWeaponInfo()
    {
        this.CleanPanel();
    }
    #endregion WEAPON

    #region KEYS

    /// <summary>
    /// Show a panel with the info of the keys for the user
    /// </summary>
    /// <param name="status"></param>
    public void ShowKeysInfo(bool status)
    {
        if (status)
        {
            // the door is going to be open
            centeredText.text = "Abriendo puerta..";
        }
        else
        {

            centeredText.text = "No se tienen todas las llaves para poder acceder";
        }
    }

    public void UpdateKeyNumber(int number)
    {
        keyEquiped.text = number.ToString();
    }

    public void CleanPanel()
    {
        if (centralPanel)
        {
            centralPanel.SetActive(false);
        }
        centeredImage.sprite = null;
        centeredImage.gameObject.SetActive(false);
        centeredText.text = "";
    }
    #endregion KEYS
}
