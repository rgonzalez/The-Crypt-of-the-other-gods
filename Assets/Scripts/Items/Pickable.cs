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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!autoPick) // only with items that must USE to pick
        { 
            if (Input.GetButtonDown("Use") && touching)
            {
                picking = true;
            }
            else
            {
                picking = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (picking)
        {

            PickItem();
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
                    break;
            }
        }
    }

    void AddAmmo()
    {
        InventoryManager.instance.AddAmmoToInventory(ammo, ammoType);
        Destroy(gameObject);
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
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            player = other.gameObject;
            touching = true;
            if (autoPick)
            {
                PickItem();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            player = null;
            touching = false;
        }
    }


}
