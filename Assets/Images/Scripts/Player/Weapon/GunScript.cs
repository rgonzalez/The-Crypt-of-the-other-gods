using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField]
    List<WeaponType> weapons;

    WeaponType actualWeapon = null;

    bool firing = false;

    float lastFire = 0.0f;
    float nextFire = 0.0f;
    int bullets = 0;
    GameObject actualMuzzle;

    public LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        if (weapons.Count > 0 )
        {
            actualWeapon = weapons[0];
            if (actualWeapon.muzzle != null) {
                actualMuzzle = Instantiate(actualWeapon.muzzle, transform);
                actualMuzzle.SetActive(false);
            }
            if (line != null)
            {
                this.line.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        HandleInput();


    }

    private void FixedUpdate()
    {
        HandleFire();
    }

    void HandleInput()
    {
        this.firing = Input.GetButton("Fire1");
    }

    void HandleFire()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        if (this.firing)
        {
            if (Time.time >= this.lastFire)
            {
                // can Shoot
                if (this.actualWeapon != null)
                {
                    Transform transformFire = this.transform;
                    // Check if can muzzle 
                    //TODO: The muzzle must not SPAWN, must be activate/deactivate, only SPAWN when the user change weapon 
                    if (this.actualMuzzle != null)
                    {
                        this.actualMuzzle.SetActive(true);
                    }

                    // before get the recoil, we need the target:
                    if (this.actualWeapon.recoilX >= 0 || this.actualWeapon.recoilY >= 0)
                    {
                        Vector3 euler = transform.eulerAngles;
                        euler.x += Random.Range(-actualWeapon.recoilX, this.actualWeapon.recoilX);
                        euler.y += Random.Range(-actualWeapon.recoilX, this.actualWeapon.recoilY);
                        transformFire.eulerAngles = euler;
                    }

                    if (this.actualWeapon.bulletPrefab != null)
                    {
                        Instantiate(this.actualWeapon.bulletPrefab, transformFire);
                    }
                    else
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
                        {
                            Debug.DrawRay(transform.position, transform.forward, Color.red);
                            Debug.Log("Did Hit");
                        }
                        else
                        {
                            Debug.DrawRay(transform.position, transform.forward, Color.green);
                            Debug.Log("Did not Hit");
                        }

                        // line rendered
                        this.line.SetPosition(0, transform.position);
                        this.line.SetPosition(1, hit.point);
                        this.line.enabled = true;

                    }
                }
            }
        }
        else
        {
            if (this.actualMuzzle != null)
            {
                this.actualMuzzle.SetActive(false);
            }
            if (this.line != null)
            {
                this.line.enabled = false;
            }
        }
    }
}
