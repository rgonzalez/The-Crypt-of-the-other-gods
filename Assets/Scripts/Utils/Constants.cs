using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants 
{
    // tags
    public static string TAG_ENEMY = "Enemy";
    public static string TAG_PLAYER = "Player";
    public static string TAG_WEAPON = "Weapon";

    public enum AMMO_TYPE { BULLET, PLASMA, ROCKET, LASER };
    public enum PICKABLE_TYPE { AMMO, WEAPON, HEALTH};
}


