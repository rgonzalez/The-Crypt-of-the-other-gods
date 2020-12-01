using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponAvailability
{
    public WeaponInfoScriptable info;
    public bool available;
}
public class ExperienceManager : MonoBehaviour
{
    /// <summary>
    /// THe Experience Manager gets the actual experience points of the User, so he can spend to shop new weapons
    /// The experience is saved between games, so the system should be saved
    /// 
    /// Also this Manager controls the locked/unlocked weapon
    /// </summary>

    public int actualExp = 0;
    public int defaultExp = 0; //start experience 

    public static ExperienceManager instance;

    private List<WeaponAvailability> weaponsAvailables = new List<WeaponAvailability>();

    // Use this for initialization
    void Start()
    {
        if (ExperienceManager.instance != null)
        {
            Destroy(this);
        }
        else
        {
            ExperienceManager.instance = this;
            //load the experience from previous games
            LoadExp();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddExperience(int exp)
    {
        actualExp += exp;
    }

    public void SaveExp()
    {
        PlayerPrefs.SetInt("EXP", actualExp);
        PlayerPrefs.Save();
    }

    public void LoadExp()
    {
        actualExp = PlayerPrefs.GetInt("EXP", defaultExp);

    }

    /// <summary>
    /// Check if a weapon is available for the user from previous games
    /// </summary>
    /// <param name="wType">The weapon type</param>
    /// <returns>TRUE if the weapon is available, FALSE if not</returns>
    public bool LoadAvailability(Constants.WEAPON_TYPE wType)
    {
        int status = PlayerPrefs.GetInt(wType.ToString("G"), 0);
        return status != 0;
    }

    /// <summary>
    /// Save a change of Availability in disk
    /// </summary>
    /// <param name="wType"></param>
    public void SaveAvailability(Constants.WEAPON_TYPE wType)
    {
        PlayerPrefs.SetInt(wType.ToString("G"), 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load what weapons are available from Disk
    /// </summary>
    public void LoadAvailableWeapons()
    {
       List<WeaponInfoScriptable> infos = WeaponSpawnManager.instance.weaponInfoList;
        //generate the list
        weaponsAvailables = new List<WeaponAvailability>();
       foreach(WeaponInfoScriptable info in infos)
        {
            WeaponAvailability wAvailable = new WeaponAvailability();
            wAvailable.info = info;
            // check if the weapon is available from Disk (this funcion is called first time)
            // we get the value of the weapon avalability from Disk
            wAvailable.available = this.LoadAvailability(info.weaponType);
            weaponsAvailables.Add(wAvailable);
        }
    }


    public void EnableWeapon(Constants.WEAPON_TYPE wType)
    {
        foreach(WeaponAvailability availableWeapon in weaponsAvailables)
        {
            if (availableWeapon.info.weaponType == wType)
            {
                availableWeapon.available = true;
                this.SaveAvailability(wType);
            }
        }
    }
}
