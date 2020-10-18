using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  abstract class AbstractWeapon : MonoBehaviour
{

    public float fireCD = 30f;
    public GameObject muzzleFlashPrefab;
    // weapon info
    public int ammo = 30; // actual ammo in the clip
    public int maxClip = 30; //max capacity of the clip
    public int bulletsPerShoot = 1; // how much ammo per shoot
    public float reloadSconds = 2;
    public float recoilX = 1f;
    public float recoilY = 0.5f;
    public int damage = 2;

    // Audios
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    //internal variables
    protected GameObject muzzleFlash;
    protected AudioSource audioSource;
    protected float nextFire = 0f;
    protected bool firing = false;
    protected bool reload = false;
    protected bool reloading = false;



    // Start is called before the first frame update
    void Start()
    {
        if (muzzleFlashPrefab && muzzleFlash == null)
        {
            muzzleFlash = Instantiate(muzzleFlashPrefab, transform);
            muzzleFlash.SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
        // Geberal Code for start the weapon
        OnStarting(); // specific code Start() for the weapon
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && !reloading)
        {

            firing = true;
        }
        else
        {
            firing = false;
            if (muzzleFlash)
            {
                //    muzzleFlash.SetActive(false);
            }
        }
        if (Input.GetButton("Reload"))
        {
            reload = true;
        }
        else
        {
            reload = false;
        }
        OnUpdating();
    }

    private void FixedUpdate()
    {
        if (firing)
        {
            if (ammo > 0)
            {
                Shoot();
            }
        }
        if (reload)
        {
            if (!reloading)
            {
                Reload();
            }
        }
    }

    protected abstract void OnStarting();
    protected abstract void OnUpdating();
    protected abstract void Reload();
    protected abstract void Shoot();
}
