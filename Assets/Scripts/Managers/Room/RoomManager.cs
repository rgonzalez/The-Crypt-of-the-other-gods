using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    /// <summary>
    /// The config for a specific room, and control the waves
    /// </summary>
    /// 

    public List<RoomConfigScriptable> roomConfigs;
    private RoomConfigScriptable roomConfig;

    public List<GameObject> spawns; // the spawns in this room

    public float secondBetweenSpawns = 5; // how much seconds between a spawn works (inside wave)
    public float secondsBetweenWaves = 5;

    private WaveConfiguration actualConfiguration;

    private bool activeRoom = true; // if the room is work, so the room can spawn
    private bool spawningRoom = false; //just a check var, maybe by error the user can enter twice in the trigger

    public List<UsableDoor> doors = new List<UsableDoor>();

    //actual Wave info

    private int actualWave;
    private int maxEnemiesWave = 0;
    private int enemiesLeft = 0;

    private bool started = false; // if the room is already active

    //is a chest when the waves ends? must be a child inside the room, so we can configure it
    //must be disabled at start
    public GameObject chest;

    public GameObject SpawnEffect; //special effect for every spawn, can be changed to a list (random or by spawn) in a future

    //list of cloths in the room
    public List<Cloth> cloths = new List<Cloth>();

    // Use this for initialization
    void Start()
    {
        actualWave = 0;
        activeRoom = true;
        cloths = new List<Cloth>(transform.parent.GetComponentsInChildren<Cloth>()); // the triggerRoom is inside the room, we must go up 1 level
        //add the player capsulle to the system
       
    }


    private void UpdateClothes(CapsuleCollider newCollider)
    {
        foreach (Cloth cloth in cloths)
        {
            //get the list of colliders of this cloth
            List<CapsuleCollider> colliders = new List<CapsuleCollider>(cloth.capsuleColliders);
            colliders.Add(newCollider);
            cloth.capsuleColliders = colliders.ToArray();
            Debug.Log("cloth collider:" + cloth.capsuleColliders.Length);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // move the doors instead of activate
        if (activeRoom && other.CompareTag(Constants.TAG_PLAYER))
        {
            if (!started)
            {
                started = true;
                Debug.Log("ENTER TRIGGER");
                GameObject player = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER);

                if (player)
                {
                    CapsuleCollider col = player.GetComponentInChildren<CapsuleCollider>();
                    Debug.Log("get user collider");
                    UpdateClothes(col);
                }
                if (!spawningRoom)
                {
                    spawningRoom = true;
                    CloseRoom();
                    if (roomConfigs.Count > 0)
                    {
                        // choose a random config of enemies
                        roomConfig = roomConfigs[Random.Range(0, roomConfigs.Count - 1)];
                        // start spawn ticks until all enemies or the actual wave is dead
                        ConfigWave(true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Create a new wave of enemies
    /// </summary>
    /// <param name="inmediate">if TRUE, the next wave doesnt wait the usual time, is a instaspawn</param>
    private void ConfigWave(bool inmediate)
    {
        if (roomConfig.waves.Count > actualWave)
        {
            Debug.Log("config wave " + actualWave);
            actualConfiguration = roomConfig.waves[actualWave].Clone(); //clone the scriptable, so we dont edit the original
            enemiesLeft = 0;
            //get the total of enemies
            foreach (EnemyConfig enemyConfig in actualConfiguration.enemies)
            {
                enemiesLeft += enemyConfig.ammount;
            }
            maxEnemiesWave = enemiesLeft;
            Debug.Log("Ready to spawn " + maxEnemiesWave);
            StartCoroutine(SpawnWave(inmediate));
        } else
        {
            // now waves left, open the room
            // FINISH ROOM

            activeRoom = false;
            OpenRoom();            
        }
    }

    /// <summary>
    /// Spawns a wave of enemies
    /// </summary>
    /// <param name="inmediate"> if TRUE, doesnt wait for the initialTime</param>
    /// <returns></returns>
    IEnumerator SpawnWave(bool inmediate)
    {
        Debug.Log("SPAWN WAVE!");
        int indexSpawn = 0;
        int enemiesSpawned = 0;
        if (!inmediate)
        {
            yield return new WaitForSecondsRealtime(secondBetweenSpawns);
        }
        //just spawn enemies as many spawn points
        //iterate of each type of enemy
        while (enemiesSpawned < maxEnemiesWave) {
            Debug.Log("ITERATION LEFT: " + enemiesLeft + " OF " + maxEnemiesWave);
            bool spawnsCompleted = false; // 
            foreach (EnemyConfig enemyConfig in actualConfiguration.enemies)
            {
                Debug.Log("TRYIN SPAWN  " + enemyConfig.ammount + " _" + enemyConfig.enemy);
                //spawn each type of enemy until is 0
                if (enemyConfig.ammount > 0)
                {
                    if (SpawnEffect)
                    {
                        GameObject effect = Instantiate(SpawnEffect, spawns[indexSpawn].transform.position, Quaternion.Euler(Vector3.zero));
                    }
                    GameObject enemy = Instantiate(enemyConfig.enemy, spawns[indexSpawn].transform.position, Quaternion.Euler(Vector3.zero));
                    EnemyHealth healthEnemy = enemy.GetComponent<EnemyHealth>();
                    CapsuleCollider enemyCollider = enemy.GetComponent<CapsuleCollider>();
                    if (enemyCollider)
                    {
                        UpdateClothes(enemyCollider);
                    }
                    healthEnemy.room = this;
                    Debug.Log("SPAWNED AT " + indexSpawn);
                    indexSpawn++;

                    enemyConfig.ammount--; // an enemy is dead here
                    enemiesSpawned++;
                    if (indexSpawn >= spawns.Count)
                    {
                        Debug.Log("MAX SPAWN REACHED");
                        indexSpawn = 0;
                        yield return new WaitForSecondsRealtime(secondBetweenSpawns); //wait until the spawns are free again
                    }
                }
            }
            Debug.Log("END ITERATION ENEMIES");
        }
        Debug.Log("END WHILE");
        //the wave has spawned, the next will spawn when all enemies of the actual spawn are dead
    }

    //an enemy has dead
    public void NotifyDead()
    {
        enemiesLeft--;
        if (enemiesLeft <= 0)
        {
            actualWave++;
            ConfigWave(false);
            //the wave is finished, spawn other wave (the wave checks itself)

        }
    }

    //TODO
    private void OpenRoom()
    {
        foreach(UsableDoor door in doors)
        {
            //TODO: animate door
            //door.SetActive(false);
            if (door)
            {
                door.OpenDoor();
            }
        }
        if (chest != null)
        {
            chest.SetActive(true);
        }
    }

    private void CloseRoom()
    {
        foreach (UsableDoor door in doors)
        {
            //TODO: animate door
            // door.SetActive(true);
            if (door) // maybe is a destroyed door (by a Quad/doorway)
            {
                door.CloseDoor();
            }
        }
    }

}
