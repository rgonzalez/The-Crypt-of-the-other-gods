using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{

    public int actualEnemies = 0;
    public int waves;
    public static EnemyManager instance;
    // Use this for initialization
    void Start()
    {
        if (EnemyManager.instance != null)
        {
            Destroy(this);
        } else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
