using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class ChestScript : MonoBehaviour
{

    Animator animator;
    bool touching = false;
    bool open = false;
    bool active = true;
    [SerializeField]
    GameObject spawnPoint;

    public bool randomWeapon = false;
    public GameObject prefabToCreate;
    public int weaponsToCreate = 0;
    public int ammoToCreate = 0; // random ammo to create
    public int healToCreate = 0; // how heal items to create;
    public float secondsBetweenSpawn = 1f;
    public GameObject healthPrefab;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (touching && Input.GetButtonDown("Use") && active)
        {
            open = true;
        }
    }

    private void FixedUpdate()
    {
        if (active && open)
        {
            active = false;
            animator.SetBool("Open", true);
            HighLighted hl = GetComponent<HighLighted>();
            HighLighted[] hls = GetComponentsInChildren<HighLighted>();
            foreach(HighLighted hlchild in hls)
            {
                hlchild.Disable();
            }
            StartCoroutine(CreateItem());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            touching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            touching = false;
        }
    }

    IEnumerator CreateItem()
    {
        // create the random ammo, and heals, then the weapons

          List<Constants.AMMO_TYPE> ammos = Enum.GetValues(typeof(Constants.AMMO_TYPE))
            .Cast<Constants.AMMO_TYPE>()
            .ToList();
        List<Constants.WEAPON_TYPE> weapons = Enum.GetValues(typeof(Constants.WEAPON_TYPE))
           .Cast<Constants.WEAPON_TYPE>()
           .ToList();

        for (int i = 0; i < ammoToCreate; i++)
        {

            yield return new WaitForSecondsRealtime(secondsBetweenSpawn);
            int randomIndex = UnityEngine.Random.Range(0,(ammos.Count-1));
            Constants.AMMO_TYPE newAmmo = ammos[randomIndex];
            AmmoInfoScriptable ammo = WeaponSpawnManager.instance.GetAmmoInfo(newAmmo);
            CreateInstance(ammo.pickableAmmoPrefab);
        }
        for (int i = 0; i < healToCreate; i++)
        {

            yield return new WaitForSecondsRealtime(secondsBetweenSpawn);
            // only one type of heal at this point
            CreateInstance(healthPrefab);
        }
        for (int i = 0; i < weaponsToCreate; i++)
        {

            yield return new WaitForSecondsRealtime(secondsBetweenSpawn);
            GameObject weaponToCreate = null;
            if (randomWeapon)
            {
                int randomIndex = UnityEngine.Random.Range(0, (weapons.Count - 1));
                Constants.WEAPON_TYPE wType = weapons[randomIndex];
                WeaponInfoScriptable weaponInfo = WeaponSpawnManager.instance.GetWeaponInfo(wType);
                weaponToCreate = weaponInfo.pickableWeaponPrefab;
            } else
            {
                weaponToCreate = prefabToCreate;
            }
            CreateInstance(weaponToCreate);
        }
    }

    void CreateInstance(GameObject prefab)
    {
        //ad a force to drop 
        Vector3 direction = new Vector3(UnityEngine.Random.Range(0, 1), 1, 0);

        GameObject instantiatedAmmo = Instantiate(prefab, spawnPoint.transform.position, Quaternion.Euler(direction));

        var tempRotation = Quaternion.identity;
        var tempVector = Vector3.zero;
        tempVector = tempRotation.eulerAngles;

        tempVector.x = UnityEngine.Random.Range(0, 359);
        tempVector.y = UnityEngine.Random.Range(0, 180);
        tempRotation.eulerAngles = tempVector;
        instantiatedAmmo.transform.rotation = tempRotation;

        instantiatedAmmo.GetComponent<Rigidbody>().AddForce(Vector3.up * 6, ForceMode.Impulse);
    }
}
