﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSpawnManager : MonoBehaviour
{

    public static WeaponSpawnManager instance;
    // a list of weapon info scriptable objects
    public List<WeaponInfoScriptable> weaponInfoList = new List<WeaponInfoScriptable>();


    // Use this for initialization
    void Start()
    {
        if (WeaponSpawnManager.instance != null)
        {
            Destroy(this);
        } else
        {
            WeaponSpawnManager.instance = this;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    ///  Instantiate a new Pickable item weapon in the World, in the transform position/rotation
    ///  CAN instantiate a totally NEW weapon (specifing the type) or a pickable with a weapon inside
    ///  for example a droped weapon with half ammo, cant edit the prefab, so we attach the real weapon
    /// </summary>
    /// <param name="pointToSpawn"> The position in the world where drop the weapon, is the transform of the spawnPoint</param>
    /// <param name="instantiateNew"> if FALSE: the new pickableWeapon contains an GameObject of weapon, if TRUE contains a PREFAB</param>
    /// <param name="weaponType"> The Kind of weapon, to search in the list of weaponlist</param>
    /// <param name="weapon">The GameObject of an instantiated weapon, in case of not creating a prefab</param>
    public void InstantiateWeapon( Transform pointToSpawn, bool instantiateNew, Constants.WEAPON_TYPE weaponType, GameObject weapon )
    {
        WeaponInfoScriptable weaponInstantiate = null;
        bool found = false;
        foreach (WeaponInfoScriptable wInfo in weaponInfoList)
        {
            if (wInfo.weaponType == weaponType)
            {
                weaponInstantiate = wInfo;
                found = true;
            }
        }
        if (found)
        {
            GameObject newPickable;
            newPickable = Instantiate(weaponInstantiate.pickableWeaponPrefab);
            newPickable.transform.position = pointToSpawn.position;
            newPickable.transform.rotation = pointToSpawn.rotation;
         
            if (instantiateNew)
            {
                // the new drop is a pickable from PREFAB, so the prefab already contains the prefabweapon, do nothing
                //just secure that the weapon has not an objectattached
                newPickable.GetComponent<Pickable>().weaponAttached = null;
              
            } else
            { // IS AN ACTUAL WEAPON  ON THE MAP!!! NOT A PREFAB
                // is not a instance of prefab? no new item? then is a already instantiated weapon, attach to the
                // pickable
                weapon.GetComponent<AbstractWeapon>().active = false; // just in case disable the weapon
                newPickable.GetComponent<Pickable>().weaponAttached = weapon;
                //we have move the weapon from the player, to the gameObject
                weapon.transform.SetParent(newPickable.transform);
            }
            newPickable.GetComponent<Rigidbody>().AddForce(transform.forward * 10);
            // now with the pickable push it into the real world
        }
    }
}
