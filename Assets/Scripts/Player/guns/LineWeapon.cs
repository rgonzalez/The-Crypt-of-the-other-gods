using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWeapon : MonoBehaviour
{

    public float fireCD = 30f;
    public GameObject muzzleFlashPrefab;
    public LineRenderer lineRenderer;
    private GameObject muzzleFlash;
    private AudioSource audioSource;

    public float distance = 10f;

    //variables set by Script
   // public GameObject muzzleCannon;


    private float nextFire = 0f;
    private bool firing = false;
    private bool reload = false;
    private bool reloading = false;


    // weapon info
    public int ammo = 30; // actual ammo in the clip
    public int maxClip = 30; //max capacity of the clip
    public int bulletsPerShoot = 1; // how much ammo per shoot
    public float reloadSconds = 2;
    public int damage = 2;

    // Audios
    public AudioClip shootAudio;
    public AudioClip reloadAudio;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (muzzleFlashPrefab && muzzleFlash == null)
        {
            muzzleFlash = Instantiate(muzzleFlashPrefab, transform);
            muzzleFlash.SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
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
        } else
        {
            reload = false;
        }
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
    IEnumerator DisableMuzzle()
    {
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);

        lineRenderer.positionCount = 0;
    }


    void Shoot()
    {
        if (Time.time > nextFire)
        {
            if (muzzleFlash != null)
            {
                Debug.Log("ACTIVE!");
                muzzleFlash.SetActive(true);
                StartCoroutine(DisableMuzzle());
            }
            if (shootAudio)
            {
                audioSource.PlayOneShot(shootAudio);
            }
            ammo -= bulletsPerShoot;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
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
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, worldPosition - transform.position, out hit, distance))
                {
                   // Debug.DrawRay(transform.position, worldPosition - transform.position, Color.yellow, 10f);
                    lineRenderer.SetPosition(1, hit.point);
                    Debug.Log("Did Hit");
                    if (hit.collider.CompareTag(Constants.TAG_ENEMY))
                    {
                        // try to check if has health to apply damage
                        HealthScript enemyHealth = hit.collider.GetComponent<HealthScript>();
                        if (enemyHealth) enemyHealth.Damage(damage);
                    }
                }
                else
                {
                  //  Debug.DrawRay(transform.position, worldPosition - transform.position, Color.white, 10f);
                    Debug.Log("Did not Hit " + hit.point);
                    Vector3 pos = (worldPosition - transform.position) * distance;
                    lineRenderer.SetPosition(1, pos);
                }
            }
            nextFire = Time.time + fireCD;
        } 
    }

    void Reload()
    {
        reloading = true;
        if (reloadAudio)
        {
            audioSource.PlayOneShot(reloadAudio);
        }
        StartCoroutine(CompleteReload());
    }

    IEnumerator CompleteReload()
    {
        yield return new WaitForSecondsRealtime(reloadSconds);
        reloading = false;
        ammo = maxClip; //TODO: reload with real ammo from inventory
    }
}
