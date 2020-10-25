using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public enum RELOADSTATE { READY, RELOADING, FAILED, PERFECT, ACTIVE}; // the status available for the weapon

public  abstract class AbstractWeapon : MonoBehaviour
{
    public bool active = false; // is the weapon active?
    public AMMO_TYPE ammoType;
    public float fireCD = 30f;
    public GameObject muzzleFlashPrefab;
    // weapon info
    public int ammo = 30; // actual ammo in the clip
    public int maxClip = 30; //max capacity of the clip
    public int bulletsPerShoot = 1; // how much ammo per shoot
    protected int perfectAmmo = 0; //the actual Perfect ammo (with critic) for an perfect reload

    public float recoilX = 1f;
    public float recoilY = 0.5f;
    public int damage = 2; // normal Damage
    public int perfectCritic = 150; // the damage for an perfect reload in % (a 150% of the normal damage)


    //laserSight logic
    public bool haveLaserSight = false;
    public GameObject laserSightPrefab;
    private LineRenderer laserSight;

    public RELOADSTATE weaponStatus = RELOADSTATE.READY;
    // RELOAD LOGIC
    [SerializeField]
    public SpriteRenderer slider;

    [SerializeField]
    public GameObject reloadBar;

    [SerializeField]
    public SpriteRenderer perfectImage;
    [SerializeField]
    public SpriteRenderer activeImage;
    public Transform startBar;
    public Transform endBar;

    [Header("ReloadInfo")]
    public float reloadSconds = 2;
    public float standardReload = 3f;
    public float activeReload = 2.25f;
    public float activeReloadEnd = 2.25f;
    public float perfectReload = 1.8f;
    public float failedReload = 4.0f;
    // the times are order in the GUI like this
    // |=====|perfectReload        |activeReload          activeReloadEnd|=====standarReload|
    // if the user fails to manualLoad, will spend the failedReload time



    // Audios
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    //internal variables
    protected GameObject muzzleFlash;
    protected AudioSource audioSource;
    protected float nextFire = 0f;
    protected bool firing = false;
    protected bool reload = false;
    protected bool reloading = false; // says if the system is reloading
    protected float actualTimeInReload = 0f; //how is the actual time in the reload system

    Coroutine reloadRoutine;


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

        ConfigWeapon();

        OnStarting(); // specific code Start() for the weapon
    }

    public void ConfigWeapon()
    {
        // --> set the activeReloadBar and PerfectReloadBar in the size depending of the weapon
        // the reloadBar is 0-1, so we escalate the 0 - standardReloadTime, and then place the bars
        if (activeReload > 0f && perfectReload > 0 && activeReloadEnd > 0 &&
            perfectImage != null && activeImage != null)
        {

            float startPerfectRange = perfectReload / standardReload;
            float endPerfectRange = activeReload / standardReload;
            SetReloadRange(startPerfectRange, endPerfectRange, perfectImage);
            float startActiveRange = endPerfectRange;
            float endActiveRange = activeReloadEnd / standardReload;
            SetReloadRange(startActiveRange, endActiveRange, activeImage);
        }
        if (haveLaserSight && laserSightPrefab)
        {
            GameObject laser = Instantiate(laserSightPrefab, transform);
            laserSight = laser.GetComponent<LineRenderer>();
        }
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
        if (Input.GetButtonDown("Reload"))
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
                BeginReload();
                reload = false; 
            } else if (weaponStatus == RELOADSTATE.RELOADING) // only can re-reload (aply 'active reload') if is reloading
                                                               // just in case the user tries to reload and the weapon is failing
            {

                //is RELOADING!! is a perfect?
                ManualReload();
                reload = false;
            }
        }

    }

    /// <summary>
    /// Set the ranges in the visual Bar
    /// </summary>
    public void SetReloadRange(float startRange, float endRange, SpriteRenderer imageRange)
    {

        //the size is set relative to parent, so we pass the range in 0-1 scale
        imageRange.transform.localScale = new Vector3(endRange - startRange, 1, 1);
        // get the relative positions between start and end positions in 3d world
        // the startBar ALWAYS IS NEGATIVE (is left to the 0) and endBar is POSITIVE, so we must get the total range adding the ABS values
        float range = Math.Abs(startBar.localPosition.x) + Math.Abs(endBar.localPosition.x);
        float startPosition = range * startRange; // the start is in 0-1 range, so just multiply to get the relative position
                                                  // the position will be the new position in the segment, NOTE: THE SEGMENT HAS A PIVOT TO THE LEFT; SO THE CENTER/POSITION IS LEFT OF THE SEGMENT

        imageRange.transform.localPosition = startBar.localPosition + new Vector3(startPosition, imageRange.transform.localPosition.y, imageRange.transform.localPosition.z);
 
    }


    public void ManualReload()
    {

        Debug.Log("stop reloading");
        // float value = slider.rectTransform.anchoredPosition.x;
        float value = actualTimeInReload;
        if (value >= perfectReload && value <= activeReload)
        {
            Debug.Log("PERFECT!!!");
            weaponStatus = RELOADSTATE.PERFECT;
            PerfectReload(value);
            
        } else if (value >= activeReload && value <= activeReloadEnd)
        {

            Debug.Log("ACTIVE!!!");
            weaponStatus = RELOADSTATE.ACTIVE;
            ActiveReload(value);
        } else
        {

            weaponStatus = RELOADSTATE.FAILED;
            Debug.Log("FAIL!!!");
            FailedReload(value);
        }
        if (reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
        }
    }

    IEnumerator FinishReload(float duration, bool perfect)
    {
        Debug.Log("reload -> " + perfect + " wait: " + duration);
        yield return new WaitForSeconds(duration);
        Debug.Log("endreload");
        reloading = false;
        slider.transform.localPosition = new Vector3(startBar.transform.localPosition.x, slider.transform.localPosition.y, slider.transform.localPosition.z);
        // Now we are going to extract the leftAmmo from inventory, and add to the actualCLip
        //NOTE: only the new ammo added will be critic if is a perfect reload (so if we reload with 29/30 ammo, only 1 shoot wil be critical)
        if (InventoryManager.instance)
        {
            int ammoToFill = maxClip - ammo; // how much to fill the actual clip?
            int addedAmmo = InventoryManager.instance.ExtractAmmoFromInventory(ammoToFill, ammoType);
            // maybe the inventory has not enough, so just add how much left
            ammo = addedAmmo;
            if (perfect)
            {
                //IS A PERFECT RELOAD SO ADD CRITICAL AMMO!!!
                perfectAmmo = addedAmmo;
            }
            else
            {
                // IS A NORMAL RELOAD
                perfectAmmo = 0;
            }
        }
        weaponStatus = RELOADSTATE.READY;
        reloadBar.SetActive(false);       
    }

    void PerfectReload(float value)
    {

        slider.color = Color.green;
        float t = Mathf.InverseLerp(0, 300, value);
        float remaining = perfectReload - value;
        StartCoroutine(FinishReload(remaining, true));
    }
    void ActiveReload(float value)
    {
        float remaining = activeReload - value;
        StartCoroutine(FinishReload(remaining, false));
    }

    void FailedReload(float value)
    {

        slider.GetComponent<SpriteRenderer>().color = Color.red;
        float remaining = failedReload - value;
        StartCoroutine(FinishReload(remaining, false));
    }

    // start the reload coroutine
    public void BeginReload()
    {
        weaponStatus = RELOADSTATE.RELOADING;
        reloading = true;
        actualTimeInReload = 0f;
        slider.color = Color.white;
        reloadBar.SetActive(true);
        reloadRoutine = StartCoroutine(Reloading());
    }

    // routine that moves the bar in the slider
    IEnumerator Reloading()
    {
        yield return new WaitForEndOfFrame();

        //Start the Slider in the start position

        slider.transform.localPosition = new Vector3(startBar.localPosition.x, slider.transform.localPosition.y, slider.transform.localPosition.z);
        float range = Math.Abs(startBar.localPosition.x) + Math.Abs(endBar.localPosition.x);
        for (float t = 0f; t < 1f; t += (Time.deltaTime / standardReload))
       // while (t < 1f)
        {
            //    t += (Time.deltaTime / standardReload);
            // moves the time of this frame, depending on the total time reload
            // float value = Mathf.Lerp(0, 300, curve.Evaluate(t));
            // slider.rectTransform.anchoredPosition = new Vector2(t, 0);
            // we have to move a relation in time (now t is the movement in 0-1 scale) in the world transform location
            slider.transform.localPosition = new Vector3(startBar.transform.localPosition.x + (t * range), slider.transform.localPosition.y, slider.transform.localPosition.y);
            actualTimeInReload += Time.deltaTime; //update the time that has been passed in the reload
            yield return null;
        }
        StartCoroutine(FinishReload(0.0f, false));
    }

    protected abstract void OnStarting();
    protected abstract void OnUpdating();
    protected abstract void Reload();
    protected abstract void Shoot();

 }
