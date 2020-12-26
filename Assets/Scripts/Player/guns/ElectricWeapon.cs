using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;

public class ElectricWeapon : AbstractWeapon
{

    public GameObject electricPrefab;
    public float distance;
    public int maxEnemies;
    private List<GameObject> rays = new List<GameObject>();
    private List<LightningBoltScript> raysScripts = new List<LightningBoltScript>();
    public float activeTime = 0.1f;
    private bool lasersActivated = false;
    private int actualDamage; // the damage that is doing the laser NOW 
    private List<GameObject> actualEnemies = new List<GameObject>();// the actual enemies being electrocuted, to mark a DPS
    private float nextTick = 0f;

    protected override void OnStarting()
    {
        for (int i = 0; i< maxEnemies; i++)
        {
            GameObject l = Instantiate(electricPrefab, transform);
            l.SetActive(false);
            LightningBoltScript lScript = l.GetComponent<LightningBoltScript>();
            rays.Add(l);
            raysScripts.Add(lScript);
        }
    }

    protected override void OnUpdating()
    {
        if (lasersActivated)
        {
            UpdateRays();
        }
    }

    protected override void Reload()
    {
    }

    protected override void Shoot()
    {
        if (Time.time > nextFire)
        {
            if (shootAudio)
            {
                audioSource.PlayOneShot(shootAudio);
            }
            ammo -= bulletsPerShoot;
            nextFire = Time.time + fireCD;
            ammo -= bulletsPerShoot;
            lasersActivated = true;

            // the damage is calculated here, not inside the function because could be called on update... emptying the ammo
            if (perfectAmmo > 0)
            {
                actualDamage = (int)((float)damage * (float)((float)perfectCritic / (float)100));
                perfectAmmo -= bulletsPerShoot;
            }
            else
            {
                actualDamage = damage;
            }

            StartCoroutine(DisableRays());
            nextTick = 0f;
            UpdateRays();
            UIManager.instance.Shoot(ammoType, bulletsPerShoot, ammo);
        }
    }

    void UpdateRays()
    {
        actualEnemies.Clear();
        int i = 0; //to control the max Enemies
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distance);
        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject.CompareTag(Constants.TAG_ENEMY))
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
            foreach(GameObject enemy in actualEnemies)
            {
                enemy.SendMessage("Damage", actualDamage, SendMessageOptions.DontRequireReceiver);
            }
            nextTick = Time.time + 1;
        }
        //next tick of damage in time
    }
    IEnumerator DisableRays()
    {
        yield return new WaitForSecondsRealtime(activeTime);
        foreach (GameObject ray in rays)
        {
            ray.SetActive(false);
        }
        lasersActivated = false;
    }


    private void OnDisable()
    {
        //maybe this weapon can be disabled (in case of dead for example)
        foreach (GameObject ray in rays)
        {
            ray.SetActive(false);
        }
    }

}
