using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserWeapon : AbstractWeapon
{
    [Header("Laser config")]
    public GameObject laserPrefab;
    public int numberOfLasers = 1;
    public int numberOfBounces = 0;
    public float activeTime = 0.1f;
    private bool lasersActivated = false;
    private int actualDamage; // the damage that is doing the laser NOW 

    private List<GameObject> lasers = new List<GameObject>();

    protected override void OnStarting()
    {
       for (int i = 0; i < numberOfLasers; i++)
        {
            GameObject l = Instantiate(laserPrefab, transform);
            l.SetActive(false);
            lasers.Add(l);
            // if bounces on this weapon, then generate the bouncing lasers
            for (i = 0; i < numberOfBounces; i++)
            {

                GameObject bl = Instantiate(laserPrefab, transform);
                bl.SetActive(false);
                lasers.Add(bl);
            }
        }
    }

    protected override void OnUpdating()
    {
      if (lasersActivated)
        {
            ActiveLasers();
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
            lasersActivated = true;

            // the damage is calculated here, not inside the function because could be called on update... emptying the ammo
            if (perfectAmmo > 0)
            {
                actualDamage = damage * perfectCritic;
                perfectAmmo -= bulletsPerShoot;
            } else
            {
                actualDamage = damage;
            }
            ActiveLasers(); 
            StartCoroutine(DisableLasers());
            nextFire = Time.time + fireCD;
        }
    }

    private void ActiveLasers()
    {
        for (int i = 0; i < numberOfLasers; i++)
        {
            int index = i * numberOfBounces;
            GenerateLaser(index, numberOfBounces, transform.position, transform.forward, true);
        }
    }
    private void GenerateLaser(int index, int bouncesLeft, Vector3 startPos, Vector3 direction, bool addRecoil)
    {
        GameObject l = lasers[index];
        l.transform.position = startPos;

        RaycastHit hit;
        Vector3 newTarget = Quaternion.Euler(recoilX, recoilY, 0) * direction;

        l.transform.rotation = Quaternion.LookRotation(newTarget);

        l.SetActive(true);
        if (Physics.Raycast(startPos, newTarget, out hit, Mathf.Infinity))
        {

            hit.collider.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
            if (bouncesLeft > 0)
            {
                //hit something, bounce
                Vector3 incoming = hit.point - startPos;
                Vector3 reflectVec = Vector3.Reflect(incoming, hit.normal);

                //there is left bounces, so calculate the new direction
                GenerateLaser(++index, --bouncesLeft, hit.point, reflectVec, false);
            }
        }
    }

    IEnumerator DisableLasers()
    {
        yield return new WaitForSecondsRealtime(activeTime);
        foreach (GameObject laser in lasers)
        {
            laser.SetActive(false);
        }

        lasersActivated = false;
    }

    private void OnDisable()
    {
        //maybe this weapon can be disabled (in case of dead for example)
        foreach(GameObject laser in lasers)
        {
            laser.SetActive(false);
        }
    }
}
