using UnityEngine;
using System.Collections;

public class EnemyBulletWeapon : EnemyWeapon
{

    public GameObject bulletPrefab;
    public int numberOfBullets = 1;

    public bool usePhysics = false; // if the weapon use physics to push the bullet or not. (for Grenade cases)
    public float bulletForce = 0f; // if the bullet is physics, how much force is applied (grenade force)

    public override void PrepareShoot()
    {
   
    }

    public override void Shoot()
    {
       if (Time.time > nextFire)
        {
            if (shootAudio && audioSource)
            {
                audioSource.PlayOneShot(shootAudio);
            }
            Vector3 targetPos = transform.position - target.position;
            for (int i = 0; i < numberOfBullets; i++)
            {
                // instantiate a new bullet with the recoil
                Vector3 newTarget = new Vector3(targetPos.x + Random.Range(-recoilX, +recoilX), targetPos.y + Random.Range(-recoilY, +recoilY), targetPos.z + Random.Range(-recoilX, +recoilX));
                Quaternion relativeRotation = Quaternion.LookRotation(newTarget, Vector3.up); // get the rotation to the new target

                // the position instantiation  cant be the enemy, must be with a spacing


                GameObject newBullet = Instantiate(bulletPrefab, shootPoint.position, relativeRotation);
                if (usePhysics)
                {
                    newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce);
                }
                nextFire = Time.time + fireCD;

            }

        }
    }

    protected override void OnStarting()
    {
     
    }

    protected override void OnUpdating()
    {
        
    }

}
