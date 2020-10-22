using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Explosion that check if there is colliders in range 
/// </summary>
public class Explosion : MonoBehaviour
{
    // the force to aplly to push the objects, if this var has a value !=0, a force will push objects
    public float powerForce = 0f;
    public float radius = 1f;
    public int damage = 0; // the damage can be transfered by the bullet in case of explosion by bullet/rocket

    /// <summary>
    /// Some explosions can be in time DPS (tics per seconds) or just in 1 explosion
    /// so if we specify time explosion, will do Damage (by bullet) in every Tic
    /// </summary>
    /// 
    public bool dps = false;
    public float damageSeconds = 0f; //every X apply a damage
    public float maxTime = 0f; // how much time is this explosion active


    private float nextDamage = 0f;
    public GameObject bullet; //sometimes the bullet pass itself to the explosion to be destroyed

    // Start is called before the first frame update
    void Start()
    {
        if (damageSeconds == 0)
        {
            //only at the start if is a normal explosion
            ApplyExplosion();
        }
        if (maxTime > 0)
        {
            StartCoroutine(DestroyAll());
        }
    }

    private void Update()
    {
        if (damageSeconds > 0)
        { 
            if (nextDamage < Time.time)
            {
                ApplyExplosion();
            }

        }
    }


    // explosion logic
    private void ApplyExplosion()
    {
        Debug.Log("BANG!");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hitCollider in hitColliders)
        {
            hitCollider.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver); //send the message to addDamage to all items/enemys in the range
            //also add Force of explosion
            if (powerForce != 0)
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(powerForce, transform.position, radius, 13f);
                }
            }
        }
        if (damageSeconds > 0)
        {
            nextDamage = Time.time + damageSeconds;
        }

    }


    IEnumerator DestroyAll()
    {
        yield return new WaitForSecondsRealtime(maxTime);
        if (bullet != null)
        {
            Destroy(bullet);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Draw the Gizmo of the explosion
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
