using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class WeaponToShow
{
    public GameObject weapon;
    public Constants.WEAPON_TYPE wType;
}

public class ShopMenuScript : MonoBehaviour
{
    /// <summary>
    /// Manager that instances the menu Shop system
    /// </summary>

    public static ShopMenuScript instance;
    public Camera camera;
    public GameObject spawnPointInTable; // the point to spawn on the table
    public List<WeaponToShow> weaponsToShow; // the spawned pickables in the table, created from the scriptables
    public bool shopOpen = false;
    public GameObject shopPanel;

    //vars set by the TableShop
    public GameObject spawnPoint;
    public int charges = 0; //ammount of weapons that can Spawn this shop, is set by the activated shop

    private ShopTable activeShop;
    private WeaponToShow selectedWeapon;

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

    public void SetShopMenu(bool state)
    {
        if (shopPanel)
            shopPanel.SetActive(state);
    }

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
        foreach (WeaponToShow w in weaponsToShow)
        {
            // hide all
            w.weapon.SetActive(false);
        }
        this.activeShop = shopTable;
        this.charges = charges;
        this.spawnPoint = spawnPoint;
        SetShopMenu(true);
    }



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
                    this.Close();
                }
            }
        }
    }

    /// <summary>
    ///  Instantiate all the pickables on the table to show
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
    }
}
