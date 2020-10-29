using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(fileName = "WeaponInfo", menuName = "ScriptableObjects/WeaponInfo")]
public class WeaponInfoScriptable : ScriptableObject
{

    public string prefabName;
    public Constants.WEAPON_TYPE weaponType;
    public Constants.AMMO_TYPE ammoType;
    public GameObject weaponPrefab;
    public GameObject pickableWeaponPrefab;

}
