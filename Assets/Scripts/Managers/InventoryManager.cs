using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class InventoryManager : MonoBehaviour
{
    public Constants.WEAPON_TYPE basicWeapon; // the basic weapon equiped if the user starts empty
    public GameObject emptyPickable;
    /// <summary>
    /// Actual Ammo in the bag
    /// </summary>
    public int bullets = 0;
    public int plasma = 0;
    public int rockets = 0;
    public int laser = 0;

    public int maxWeapons = 2;

    private bool usingObject = false;

    private List<GameObject> equipedWeapons = new List<GameObject>();
    public GameObject actualWeapon;
    private int changingWeapon = 0; // -1 prev, 1 next, 0  not moving
    private int indexWeapon = 0;
    private GameObject player;
    private Transform objectSpawner; //where the weapons spawns inside the player

    //reloadBar config
    private Transform startBar;
    private Transform endBar;
    private GameObject reloadBar;
    private SpriteRenderer slider;
    private SpriteRenderer perfectRange;
    private SpriteRenderer activeRange;

    public static InventoryManager instance;
    // Start is called before the first frame update
    void Start()
    {
        if (InventoryManager.instance != null)
        {
            Destroy(this);
        } else
        {
            InventoryManager.instance = this;
        }
        // create space for weapons
        player = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER);
        //get the reloadBar Graphs
        reloadBar = player.transform.Find("reloadBar").gameObject;
        startBar = reloadBar.transform.Find("StartBar");
        endBar = reloadBar.transform.Find("endBar");
        slider = reloadBar.transform.Find("SlideSprite").GetComponent<SpriteRenderer>();
        activeRange = reloadBar.transform.Find("activeSprite").GetComponent<SpriteRenderer>();
        perfectRange = reloadBar.transform.Find("perfectSprite").GetComponent<SpriteRenderer>();
        ConfigInitialWeapons(); //configure initial weapons and systems
        EquipBasicWeapon(); // equip a basic weapon if the player doesnt have anything

    }

    /// <summary>
    /// 
    /// config the actualgameobjects of the user inside the manager
    /// </summary>
    void ConfigInitialWeapons()
    {
        // maybe the player has a weapon or weapons already equiped
        //looks for child LightPivot -> GunSpawn
        //  Player
        //     |-> LightPivot
        //            |-> GunSpawn
        objectSpawner = player.transform.Find("LightPivot/GunSpawn");
        // all inside the player weaponSpawn is already a weapon
        if (objectSpawner.childCount > 0)
        {
            for (int i = 0; i < objectSpawner.childCount; i++)
            {
                GameObject w = objectSpawner.GetChild(i).gameObject;
                w.SetActive(false);
                w.GetComponent<AbstractWeapon>().active = true; // are loaded weapons
                ConfigReloadBar(w.GetComponent<AbstractWeapon>());
                equipedWeapons.Add(w);

            }
            actualWeapon = equipedWeapons[0];
            actualWeapon.SetActive(true);
            indexWeapon = 0;
        }
    }

    void ConfigReloadBar(AbstractWeapon weapon)
    {
        weapon.reloadBar = reloadBar;
        weapon.slider = slider;
        weapon.perfectImage = perfectRange;
        weapon.activeImage = activeRange;
        weapon.startBar = startBar;
        weapon.endBar = endBar;
        weapon.ConfigWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Use"))
        {
            usingObject = true;
        } else
        {
            usingObject = false;
        }
        float changWeaponDir = Input.GetAxis("Mouse ScrollWheel");
        if (changWeaponDir > 0)
        {
            changingWeapon = 1;
        } else if (changWeaponDir < 0)
        {
            changingWeapon = -1;
        } else
        {
            changingWeapon = 0;
        }
    }

    private void FixedUpdate()
    {
        if (changingWeapon !=0)
        {
            indexWeapon += changingWeapon;
            if (indexWeapon < 0) indexWeapon = equipedWeapons.Count - 1;
            actualWeapon.SetActive(false);
            indexWeapon = (indexWeapon % equipedWeapons.Count);
            actualWeapon = equipedWeapons[indexWeapon];
            actualWeapon.SetActive(true);
            ConfigReloadBar(actualWeapon.GetComponent<AbstractWeapon>());


        }
    }

    #region WEAPON


    public void EquipBasicWeapon()
    {
        //if the player doesnt have weapon... equip a rifle
        if (actualWeapon == null)
        {
            GameObject prefab = WeaponSpawnManager.instance.GetWeaponPrefab(basicWeapon);
            if (prefab != null)
            {
                EquipWeapon(prefab, true);
            }
        }
    }
    /// <summary>
    ///  Add the weapon to the inventory as active weapon, can be a prefab, or instantiated weapon
    /// </summary>
    /// <param name="pickerWeapon"> The weapon to add, can be a PREFAB (so we need create a instance) or a real weapon</param>
    /// <param name="isPrefab">if is true, we need instantiate the new weapon, if not, just attach</param>
    public void EquipWeapon(GameObject pickerWeapon, bool isPrefab)
    {

        if (objectSpawner)
        {
            GameObject newWeapon = null;
            if (isPrefab) {
                newWeapon = Instantiate(pickerWeapon, objectSpawner);
            } else
            {
                newWeapon = pickerWeapon;
                //pos in the right pos/rot
                newWeapon.transform.SetParent(objectSpawner);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            newWeapon.GetComponent<AbstractWeapon>().active = true;
            if (equipedWeapons.Count < maxWeapons)
            {
                //can pick a weapon keeping the previous!
                // so add to the list, and move the actualWeapon to the new;
                if (actualWeapon != null)
                    actualWeapon.SetActive(false);
                equipedWeapons.Add(newWeapon);
                indexWeapon = equipedWeapons.Count - 1;
                actualWeapon = equipedWeapons[indexWeapon];
            }
            else
            {
                // the bag is full of weapons... so we must replace the actual

                // GEnerate a new pickable droping the actual weapon,
                //drop the actual weapon
                AbstractWeapon wToDrop = actualWeapon.GetComponent<AbstractWeapon>();
                wToDrop.active = false; // just to not shoot the droped weapon
                if (wToDrop.haveLaserSight)
                {
                   Transform laserSight = actualWeapon.transform.FindChild("laserSight(Clone)");
                    if (laserSight)
                    {
                        laserSight.gameObject.SetActive(false);
                    }
                }
                WeaponSpawnManager.instance.InstantiateWeapon(objectSpawner, false, wToDrop.weaponType, actualWeapon);

                equipedWeapons[indexWeapon] = newWeapon;
                actualWeapon = newWeapon;
            }
            actualWeapon.SetActive(true);
            AbstractWeapon weapon = actualWeapon.GetComponent<AbstractWeapon>();
            ConfigReloadBar(weapon);
            //we must wait to the instance of UI is created, to this is in a coroutine
            StartCoroutine(EquipWeaponUI(weapon));
        }
    }

    IEnumerator EquipWeaponUI(AbstractWeapon weapon)
    {
        yield return new WaitForEndOfFrame();
        UIManager.instance.EquipAmmo(weapon.ammoType);
        //load the clip, maybe is changed (new weapon for example)
        UIManager.instance.ReloadAmmo(weapon.ammoType, weapon.maxClip, weapon.ammo, weapon.perfectAmmo);
    }
    #endregion WEAPON

    #region AMMO
    /// <summary>
    /// Get the max available ammo from inventory depending on 
    /// how much left in the bag, for example, if we want 30 bullets, but 
    /// only 3 left, this function will extract the 3 bullets from bag and set actualBullets to 0
    /// </summary>
    /// <returns>
    /// Then number of ammo that was extracted successfully
    /// </returns>
    public int ExtractAmmoFromInventory(int maxAmmo, AMMO_TYPE ammoType) 
    {
        int availableAmmo = GetAvailableAmmo(ammoType);
        int extractedAmmo = 0;
        if (availableAmmo > maxAmmo)
        {
            extractedAmmo = maxAmmo;
            availableAmmo -= maxAmmo;
        } else
        {
            // cant extract the full clip of ammo, get the left ammo available
            extractedAmmo = availableAmmo;
            availableAmmo = 0;
        }
        SetAvailableAmmo(ammoType, availableAmmo);
        return extractedAmmo;
    }

    /// <summary>
    /// Add ammo to the inventory
    /// </summary>
    /// <param name="ammo"> the ammo to be added, no max limit</param>
    /// <param name="ammoType">kind of ammo added</param>
    public void AddAmmoToInventory(int ammo, AMMO_TYPE ammoType)
    {
        int actualAmmo = GetAvailableAmmo(ammoType);
        actualAmmo += ammo;
        SetAvailableAmmo(ammoType, actualAmmo);
        UIManager.instance.UpdateAmmoBagInfo(ammoType, actualAmmo);
    }



    public int GetAvailableAmmo(AMMO_TYPE ammoType)
    {
        switch (ammoType)
        {
            case AMMO_TYPE.BULLET:
                    return bullets;
            case AMMO_TYPE.PLASMA:
                    return plasma;
            case AMMO_TYPE.ROCKET:
                    return rockets;
            case AMMO_TYPE.LASER:
                    return laser;
            default:
                return 0;
        }
    }

    public void SetAvailableAmmo(AMMO_TYPE ammoType, int ammo)
    {
        switch (ammoType)
        {
            case AMMO_TYPE.BULLET:
                 bullets = ammo;
                break;
            case AMMO_TYPE.PLASMA:
                 plasma = ammo;
                break;
            case AMMO_TYPE.ROCKET:
                rockets = ammo;
                break;
            case AMMO_TYPE.LASER:
                laser = ammo;
                break;
        }

        UIManager.instance.UpdateAmmoBagInfo(ammoType, ammo);
    }
    #endregion AMMO
}
