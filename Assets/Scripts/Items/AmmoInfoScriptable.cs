using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "AmmoInfo", menuName = "ScriptableObjects/AmmoInfo")]
public class AmmoInfoScriptable : ScriptableObject {

    public Constants.AMMO_TYPE ammoType;
    public string ammoName;
    public GameObject ammoIcon; //little icon for all ammos info, is a prefab
    public GameObject ammoUIPrefab;// the actual Ammo Equiped Prefab  - botom right
}
