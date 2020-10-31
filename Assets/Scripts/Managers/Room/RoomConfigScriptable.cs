using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class EnemyConfig
{
    public int ammount;
    public GameObject enemy;

    public EnemyConfig Clone()
    {
        EnemyConfig conf = new EnemyConfig();
        conf.ammount = ammount;
        conf.enemy = enemy;
        return conf;
    }
}

[Serializable]
public class WaveConfiguration { 
    [SerializeField]
    public List<EnemyConfig> enemies;

    public WaveConfiguration Clone()
    {

        WaveConfiguration wave = new WaveConfiguration();
        wave.enemies = new List<EnemyConfig>();
        foreach(EnemyConfig enemy in enemies)
        {
            wave.enemies.Add(enemy.Clone());
        }
        return wave;
    }
}

[CreateAssetMenu(fileName = "AmmoInfo", menuName = "ScriptableObjects/RoomConfig")]
public class RoomConfigScriptable : ScriptableObject
{
    public Constants.DIFICULT difficult;
    [SerializeField]
    public List<WaveConfiguration> waves;

 
}
