using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammo
{
    public int ammo = 0;
    public AmmoInfoScriptable info;
    public Text text; //the textcomponent with the ammo info
    public GameObject ammoUIEquiped; // the actual instance of the equiped ammo
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
    [SerializeField]
    private int heartHealth = 20; // how mucho health is a UI Heart
    [SerializeField]
    private GameObject healthPanel; //the GameObjectPanelHealth that is filled with Hearts
    [SerializeField]
    private GameObject heartPrefab; // the Heart, is a sprite with 2 layers (red and gray)

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


    // Start is called before the first frame update
    void Start()
    {
        if (UIManager.instance == null)
        {
            UIManager.instance = this;
            // Start Health Config
            ConfigureMaxHealth();
            ConfigureHealth();

            //Ammo Config
        } else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region HEALTH
    /// <summary>
    /// Set the max Health image, NOTE: this doesnt adjust the real actual life aspect
    /// </summary>
    public void ConfigureMaxHealth()
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
    }

    public void ConfigureHealth()
    {
        for (int i = 0; i < healthPanel.transform.childCount; i++)
        {
            GameObject heart = healthPanel.transform.GetChild(i).GetChild(0).gameObject;
            if (actualHealth >= ((i + 1) * heartHealth))
            {
                //there is more than this heart of life
                heart.GetComponent<Image>().fillAmount = 1;
            }
            else
            {
                int leftHeart = (actualHealth - (i * heartHealth));
                heart.GetComponent<Image>().fillAmount = (float)leftHeart  / (float)heartHealth; // set the % of the heart image
            }
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
                    ammoInfo.text = ammoInfo.ammoIcon.GetComponent<Text>();
                    ammoInfo.text.text = "0";
                    ammoInfo.ammoUIEquiped = Instantiate(ammoInfo.info.ammoUIPrefab, equipedAmmoPanel.transform);
                    ammoInfo.ammoUIEquiped.SetActive(false); //disable the equiped ammo, later active the actual Weapon
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
                ammo.ammoUIEquiped.SetActive(false);
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
                ammo.text = ammount;
            }
        }
    }

    //ADD THE SHOOT AND RELOAD

    #endregion AMMO
}
