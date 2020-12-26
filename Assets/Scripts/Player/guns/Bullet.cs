using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxTimeAlive = 15f; //times to explode if not impact
    public int damage = 1;
    public float speed = 10;
    public int bounces = 0;
    private int actualBounces = 0;
    public GameObject explosionPrefab; // if the bullet can explode, set here the prefab of the explosion, if not
                                       // the bullet will hit 

    public bool keepAfterExplosion = false; //sometimes we need the bullet keeps after explosion (like a mine)
                                            // turn true to keep the bullet gameObject, but remember deactivate in the explotion

    private RFX4_PhysicsMotion physicsMotion; // maybe the bullet is created by the prefab FX
    // this package use custom collision system, we must add it to our script

    private bool hasExploded = false; // with explosions in long time that keeps the grenade active 
                                      // this script disables itself, but sometimes multiple updates are fired before
                                      //so this bullet keeps instantiating explosions, this var controls this situation

    private float timeToExplode;
    // Start is called before the first frame update
    void Start()
    {
        physicsMotion = GetComponentInChildren<RFX4_PhysicsMotion>(true);
        if (physicsMotion != null) physicsMotion.CollisionEnter += CollisionEnter;

        var raycastCollision = GetComponentInChildren<RFX4_RaycastCollision>(true);
        if (raycastCollision != null) raycastCollision.CollisionEnter += CollisionEnter;
        if (maxTimeAlive > 0)
        {
            timeToExplode = Time.time + maxTimeAlive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeToExplode)
        {
            // the time to explode has come, maybe the grenade, rocket, bullet didnt impacted
            actualBounces = bounces; //not more bounces, just collide to explode
            Collide(null, null);
        }
        else
        {
            transform.position -= transform.forward * Time.deltaTime * speed;
        }
    }

    //normal collision
    private void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject, collision);
    }


    //collision by RFX4 package
    private void CollisionEnter(object sender, RFX4_PhysicsMotion.RFX4_CollisionInfo e)
    {

        if (bounces > 0)
        {
            if (actualBounces < bounces)
            {
                // is an bouncing bullet, check if is the last bouncing or must keep bouncing
                ((RFX4_PhysicsMotion)sender).canExplode = false;
             
            } else
            {
                // the RFX4 can explode the bullet, is not bouncing anymore
                ((RFX4_PhysicsMotion)sender).canExplode = true;
            }
        } else
        {
            ((RFX4_PhysicsMotion)sender).canExplode = true;
        }
        Collide(e.HitGameObject, e.Collision);
    }

    private void Collide(GameObject other, Collision collision )
    {
        if (actualBounces < bounces && bounces > 0)
        {
            //is bouncing, change the normal direction
            Vector3 newDirection = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
            newDirection.y = 0;
            // we keep the same vertical value, with some recoilY, the bounces can be in discontrol bouncing to the ceiling
            transform.rotation = Quaternion.LookRotation(newDirection);
            actualBounces++;
        } else 
        {
            // the actual bullet can do Damage, or can have a explosion Prefab to instantiate (in case of being a explosive bullet)
            if (explosionPrefab)
            {
                if (hasExploded) // this bullet has exploded? then do  nothing until destroy
                {
                    return;
                }
                // instantiate the explosion, the explosion has the logic to damage
                GameObject explode = Instantiate(explosionPrefab, transform.position,new Quaternion(0,0,0,0));
                // if the explosion has a Explosion Script we will send the Damage to the explotion
                Explosion explosionScript = explode.GetComponent<Explosion>();
                if (explosionScript)
                {
                    explosionScript.damage = this.damage;
                    if (keepAfterExplosion)
                    {

                        //pass the gameObject to the explosion, to destroy it in the explosionScript
                        explosionScript.bullet = this.gameObject;
                        //but we have to deactivate the grenadeScript, or it will keep spamming explosions
                        this.enabled = false;
                    }
                }
                hasExploded = true;
            }
            else
            {
                //the bullet dont explode, just hit enemy or walls
                //
            }
            if (!keepAfterExplosion)
            {
                Destroy(gameObject);
            } 
        }
        if (other) // maybe the explosion was created by time, not by collition
            other.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);

    }
}
