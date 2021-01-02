using UnityEngine;
using System.Collections;


/// <summary>
/// A pickableScript is a script that can detect if the player is near so can pick the item attached to this pickable
/// can be an item, heal, ammo or a weapon to switch NEEDS A TRIGGER
/// </summary>
public class Pickable : MonoBehaviour
{
    public Constants.PICKABLE_TYPE itemtype;
    public GameObject weaponAttached; // the attached weapon can be a prefab (prefabAttached) o a real item inside
    // this item is in case of the player drops a weapon, is not a prefab, is a instance, with own ammo

    public bool autoPick = false;
    public GameObject prefabAttached; //sometimes is a weapon, so comes with an attached prefab of weapon
    private bool touching = false; // is the player touching?
    private bool picking = false;

    //item configuration
    public int health = 10; //if is heal, how much?
    public int ammo = 10; //if is ammo, how much?
    public Constants.AMMO_TYPE ammoType; //if is ammo, what type?

    private GameObject player;
    private bool active = false; // is pickable?
    public float secondsToActive = 1f;

    public GameObject iconPrefab;
    private GameObject icon;
    public float iconHeight = 5f;
    public AudioClip pickAudio;
    protected AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(ActiveItem());
    }

    IEnumerator ActiveItem()
    {
        yield return new WaitForSecondsRealtime(secondsToActive);
        active = true;
        if (iconPrefab)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + iconHeight, transform.position.z);
            icon = Instantiate(iconPrefab, pos, Quaternion.identity);
            icon.GetComponent<IconPickable>().attachment = this.transform;
            icon.GetComponent<IconPickable>().iconHeight = this.iconHeight;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (!autoPick) // only with items that must USE to pick
            {
                if (Input.GetButtonDown("Use") && touching)
                {
                    picking = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (active)
        {
            if (picking)
            {
                picking = false;
                PickItem();
            }
        }
    }

    void PickItem()
    {
        //now check what happens, if is a weapon, assign the prefab as a weapon for the user 
        // in the actual Space
        if (player != null) {
    
            switch (itemtype)
            {
                case Constants.PICKABLE_TYPE.AMMO:
                    AddAmmo();
                    break;
                case Constants.PICKABLE_TYPE.HEALTH:
                    AddHealth();
                    break;
                case Constants.PICKABLE_TYPE.WEAPON:
                    EquipWeapon();
                    UIManager.instance.ClearWeaponInfo();
                    break;
            }
        }
    }

    void AddAmmo()
    {
        InventoryManager.instance.AddAmmoToInventory(ammo, ammoType);
        PlayAudio();
        Destroy(gameObject);
    }

    private void PlayAudio()
    {
        if (audioSource && pickAudio)
        {
            audioSource.PlayOneShot(pickAudio);
        }
    }
    void AddHealth()
    {
        if (player)
        {
            Health health = player.GetComponent<Health>();
            if (health)
            {
                bool wasHealed = health.Heal(this.health);
                if (wasHealed)
                {
                    PlayAudio();
                    //only detroy if the heal was sucesful
                    Destroy(gameObject);
                }
            }
        }
    }

    void EquipWeapon()
    {
        if (InventoryManager.instance)
        {
            if (weaponAttached)
            {
                //there is a real Weapon attached, use this instead the prefab
                InventoryManager.instance.EquipWeapon(weaponAttached, false);
            }
            else
            {
                InventoryManager.instance.EquipWeapon(prefabAttached, true);
            }
            if (icon)
            {
                Destroy(icon);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER) && active)
        {
            player = other.gameObject;
            touching = true;
            if (autoPick)
            {
                PickItem();
            }
            //in case of weapon, show the info in the UI
            if (itemtype == Constants.PICKABLE_TYPE.WEAPON)
            {
                Constants.WEAPON_TYPE weaponType = Constants.WEAPON_TYPE.BULLET_RIFLE;
                if (weaponAttached)
                {
                    weaponType = weaponAttached.GetComponent<AbstractWeapon>().weaponType;
                } else
                {
                    weaponType = prefabAttached.GetComponent<AbstractWeapon>().weaponType;
                }
                if (UIManager.instance)
                 UIManager.instance.ShowWeaponInfo(weaponType);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER) && active)
        {
            player = null;
            touching = false;
            if (itemtype == Constants.PICKABLE_TYPE.WEAPON && UIManager.instance)
                UIManager.instance.ClearWeaponInfo();
        }
    }


}
