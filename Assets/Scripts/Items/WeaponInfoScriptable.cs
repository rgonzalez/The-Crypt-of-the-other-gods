﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "ScriptableObjects/WeaponInfo")]
public class WeaponInfoScriptable : ScriptableObject
{

    public string prefabName;
    public Constants.WEAPON_TYPE weaponType;
    public Constants.AMMO_TYPE ammoType;
    public GameObject weaponPrefab;
    public GameObject pickableWeaponPrefab;
    public Sprite equipedWeaponIcon;
    public int price; // the price of the weapon to unlock it
    public Texture2D cursor;
}
