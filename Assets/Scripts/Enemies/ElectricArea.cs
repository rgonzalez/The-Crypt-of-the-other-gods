using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;

public class ElectricArea : MonoBehaviour
{
    // the area electric is ALWAYS on
    public GameObject electricPrefab;
    public float distance;
    public int maxEnemies;
    public int actualDamage;
    private List<GameObject> rays = new List<GameObject>();
    private List<LightningBoltScript> raysScripts = new List<LightningBoltScript>();
    private float nextTick = 0f;
    private List<GameObject> actualEnemies = new List<GameObject>();// the actual enemies being electrocuted, to mark a DPS

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject l = Instantiate(electricPrefab, transform);
            l.SetActive(false);
            LightningBoltScript lScript = l.GetComponent<LightningBoltScript>();
            rays.Add(l);
            raysScripts.Add(lScript);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRays();
    }

    void UpdateRays()
    {
        actualEnemies.Clear();
        int i = 0; //to control the max Enemies
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distance);
        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject.CompareTag(Constants.TAG_ENEMY) || hit.gameObject.CompareTag(Constants.TAG_PLAYER))
            {
                if (hit.gameObject != this.gameObject)
                {
                    if (i < maxEnemies)
                    {
                        rays[i].SetActive(true);
                        raysScripts[i].StartObject = this.gameObject;
                        raysScripts[i].EndObject = hit.gameObject;
                        actualEnemies.Add(hit.gameObject);
                        i++;
                    }
                }
            }
        }
        //maybe there is not enough enemies, clear the rays that had before targets
        for (int j = i; j < maxEnemies; j++)
        {
            rays[j].SetActive(false);
            raysScripts[j].StartObject = null;
            raysScripts[j].EndObject = null;
        }

        if (nextTick < Time.time)
        {
            //time to deal damage per second!
            foreach (GameObject enemy in actualEnemies)
            {
                enemy.SendMessage("Damage", actualDamage, SendMessageOptions.DontRequireReceiver);
            }
            nextTick = Time.time + 1;
        }
        //next tick of damage in time
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, distance);
    }
}
