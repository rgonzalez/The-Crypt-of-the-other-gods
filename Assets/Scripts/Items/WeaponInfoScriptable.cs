using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "ScriptableObjects/WeaponInfo", order = 1)]
public class WeaponInfoScriptable : ScriptableObject
{

    public string prefabName;
    public Constants.WEAPON_TYPE weaponType;
    public GameObject weaponPrefab;
    public GameObject pickableWeaponPrefab;

}
