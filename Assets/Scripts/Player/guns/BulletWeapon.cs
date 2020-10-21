using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Weapon that instantiate bullets, the bullet damage and specification can be defined in the bullet Prefab
/// </summary>
public class BulletWeapon : AbstractWeapon
{

    public GameObject bulletPrefab;
    public int numberOfBullets = 1;
    public bool usePhysics = false; // if the weapon use physics to push the bullet or not.
    public float bulletForce = 0f; // if the bullet is physics, how much force is applied

    protected override void OnStarting()
    {

    }

    protected override void OnUpdating()
    {

    }

    protected override void Reload()
    {
       /* reloading = true;
        if (reloadAudio)
        {
            audioSource.PlayOneShot(reloadAudio);
        }
        StartCoroutine(Completereload());*/
    }

    IEnumerator Completereload()
    {
        yield return new WaitForSecondsRealtime(reloadSconds);
       // - reloading = false;
        ammo = maxClip; //TODO: reload with real ammo from inventory
    }

    /// <summary>
    /// Instantiate a new bullet to the mouse position
    /// </summary>
    protected override void Shoot()
    {
        if (Time.time > nextFire)
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(true);
                StartCoroutine(DisableMuzzle());
            }
            if (shootAudio)
            {
                audioSource.PlayOneShot(shootAudio);
            }
            ammo -= bulletsPerShoot;
            Plane plane = new Plane(Vector3.up, transform.position);
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPosition;
            if (plane.Raycast(ray, out distance))
            {
                worldPosition = ray.GetPoint(distance);
                worldPosition = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);

                Debug.Log("transform" + transform.position + " worldPosition " + worldPosition);
                //   transform.LookAt(worldPosition);
                //  transform.TransformDirection(Vector3.forward) <- 2º parameter raycast
                RaycastHit hit;


                // now we have to cast a raycast per bullet (lineRenderer) with recoil
                Vector3 target = transform.position - worldPosition; //original Target position
                for (int i = 0; i < numberOfBullets; i++)
                {
                    // instantiate a new bullet with the recoil
                    Vector3 newTarget = new Vector3(target.x + Random.Range(-recoilX, +recoilX), target.y + Random.Range(-recoilY, +recoilY), target.z + Random.Range(-recoilX, +recoilX));
                    Quaternion relativeRotation = Quaternion.LookRotation(newTarget, Vector3.up); // get the rotation to the new target
                    GameObject newBullet = Instantiate(bulletPrefab, transform.position, relativeRotation);
                    if (usePhysics)
                    {
                        /* float maxDistance = 1; //distance for grenade to to reach
                         float hSpeed = bulletForce; // horizontal speed

                         float g = Physics.gravity.magnitude; // get the gravity value
                                                              //then we calculating other variables to set our "fly" 
                         float totalTime = maxDistance / hSpeed; //time of fly
                         float vSpeed = (totalTime * g) / 2;       //vertical speed
                                                                   //then we "launch" our grenade
                                                                   // the bullet use the physics system, so apply a initialforce
                         newBullet.GetComponent<Rigidbody>().velocity = new Vector3(newBullet.transform.forward.x * hSpeed, vSpeed, newBullet.transform.forward.z * hSpeed);
                         */
                        newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce);
                    }
                    
                }

            }

            nextFire = Time.time + fireCD;
        }
    }

    IEnumerator DisableMuzzle()
    {
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }
}
