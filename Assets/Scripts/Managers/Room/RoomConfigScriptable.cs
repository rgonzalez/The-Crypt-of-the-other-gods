using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class EnemyConfig
{
    public int ammount;
    public GameObject enemy;
}

[Serializable]
public class WaveConfiguration
{
    [SerializeField]
    public List<EnemyConfig> enemies;
}

[CreateAssetMenu(fileName = "AmmoInfo", menuName = "ScriptableObjects/RoomConfig")]
public class RoomConfigScriptable : ScriptableObject
{
    public Constants.DIFICULT difficult;
    [SerializeField]
    public List<WaveConfiguration> waves;

 
}
