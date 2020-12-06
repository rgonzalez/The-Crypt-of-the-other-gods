using UnityEngine;
using System.Collections;

public class ShopTable : MonoBehaviour
{
    public int charges = 1;
    public GameObject spawnPoint;
    private bool active = true; // is this table active
    private bool touching = false;
    private bool picking = false;
    private GameObject player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (Input.GetButtonDown("Use") && touching)
            {
                picking = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (touching && picking && active)
        {
            ShopMenuScript.instance.OpenShop(spawnPoint, charges, this);
            picking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER) && active)
        {
            player = other.gameObject;
            touching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER) && active)
        {            
            touching = false;
        }
    }
}
