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


    public GameObject prefabAttached; //sometimes is a weapon, so comes with an attached prefab of weapon
    private bool touching = false; // is the player touching?
    private bool picking = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Use") && touching)
        {
            picking = true;
        } else
        {
            picking = false;
        }
    }

    private void FixedUpdate()
    {
        if (picking)
        {
            //now check what happens, if is a weapon, assign the prefab as a weapon for the user 
            // in the actual Space
            switch (itemtype)
            {
                case Constants.PICKABLE_TYPE.AMMO:
                    break;
                case Constants.PICKABLE_TYPE.HEALTH:
                    break;
                case Constants.PICKABLE_TYPE.WEAPON:
                    EquipWeapon();
                    break;
                

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


}
