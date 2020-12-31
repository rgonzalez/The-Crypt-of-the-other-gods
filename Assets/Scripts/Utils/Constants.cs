using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants 
{
    // tags
    public static string TAG_ENEMY = "Enemy";
    public static string TAG_PLAYER = "Player";
    public static string TAG_WEAPON = "Weapon";
    public static string TAG_LEVELGENERATOR = "LevelGenerator";
    public static string TAG_CANVAS = "MainCanvas";
    public enum DIFICULT { EASY, NORMAL, HARD};

    public enum AMMO_TYPE { BULLET, PLASMA, ROCKET, LASER };
    public enum PICKABLE_TYPE { AMMO, WEAPON, HEALTH};
    // TODO: change to a combination of weapon/ammo types

        // THE INT VALUE IS USED IN THE BUTTONS SHOP
    public enum WEAPON_TYPE {
        BULLET_RIFLE, //0
        BULLET_SHOTGUN, // 1
        BULLET_MCHGUN, // 2
        PLASMA_RIFLE, // 3
        PLASMA_SHOTGUN, // 4
        PLASMA_MCHGUN, // 5
        PLASMA_GRENADE, // 6
        ROCKET_LAUNCHER, // 7
       // ROCKET_SHOTGUN,
       // ROCKET_MCHGUN,
        ROCKET_GRENADE, // 8
        LASER_RIFLE, // 9
        LASER_SHOTGUN, // 10
        //  LASER_MCHGUN,
        //  LASER_GRENADE
    };
}



