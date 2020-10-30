using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants 
{
    // tags
    public static string TAG_ENEMY = "Enemy";
    public static string TAG_PLAYER = "Player";
    public static string TAG_WEAPON = "Weapon";
    public enum DIFICULT { EASY, NORMAL, HARD};

    public enum AMMO_TYPE { BULLET, PLASMA, ROCKET, LASER };
    public enum PICKABLE_TYPE { AMMO, WEAPON, HEALTH};
    // TODO: change to a combination of weapon/ammo types
    public enum WEAPON_TYPE { BULLET_RIFLE,
        BULLET_SHOTGUN,
        BULLET_MCHGUN,
        PLASMA_RIFLE,
        PLASMA_SHOTGUN,
        PLASMA_MCHGUN,
        PLASMA_GRENADE,
        ROCKET_LAUNCHER,
        ROCKET_SHOTGUN,
        ROCKET_MCHGUN,
        ROCKET_GRENADE,
        LASER_RIFLE,
        LASER_SHOTGUN,
        LASER_MCHGUN,
        LASER_GRENADE};
}



