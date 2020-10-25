using UnityEngine;
using UnityEditor;
using System.Collections;

public class DamageDealer: MonoBehaviour
{
    public int damage = 0; // the damage passed by the weapon
    public bool canDamage = false; //to avoid deal multliple damage in one attack (maybe multiple collisions)


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (canDamage)
            {
                //the hit is with the player, deal Damage
                other.transform.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
                canDamage = false;
            }
        }
    }

}


public class EnemyMeleeWeapon : EnemyWeapon
{

   
                             // the hitbox child gameObject, must be a collider in the correct position, is the transform 'shootPoint' from the father class

    public float timeActiveAttack; // the time that the attack is active (the hitbox is alive)
                                    // the attackCD will be fired after the end of the attack, if the attack is 2seconds, and the CD
                                    // is 5 seconds, the next attack will be in 7 (2 attack active, wait 5)

    private float endAttack = 0f; // when the actual attack ends (time attack still alive)

    private EnemyIA enemyIa; // the IA of the actual enemy, this weapon disables the move hability of this Enemy while is attacking
                             // so if the enemy changes state (to follow) and the attack still, follow doesnt move

    private DamageDealer ddealer; // the script in the child component



    public override void PrepareShoot()
    {
        
    }

    /// <summary>
    ///  shoot will do a melee attack, but for this, the enemy must have some conditions
    ///  like have a child gameObject with the hitbox (a collider), this weapon will 
    ///  create a script in that child with the logic to hurt
    /// </summary>
    //
    public override void Shoot()
    {
        //only attack if the actual attack finished (endAttack) and the CD is ok
        if (Time.time > nextFire && Time.time > endAttack)
        {
            if (shootAudio && audioSource)
            {
                audioSource.PlayOneShot(shootAudio);
            }
            // the hitbox must activate the damage option (maybe is disabled by a previous attack)
            if (ddealer)
            {
                ddealer.canDamage = true;
                shootPoint.gameObject.SetActive(true);
            }
            if (this.enemyIa && this.enemyIa.canMove)
            {
                this.enemyIa.moving = false; //while attacking cant move (if is a moving enemy)
            }
            endAttack = Time.time + timeActiveAttack; // the time when this attack finishes
            Debug.Log("ends in " + endAttack);
            StartCoroutine(DisableAttack());

        }
    }



    // disable the hitbox after the specified time
    IEnumerator DisableAttack()
    {
        yield return new WaitForSecondsRealtime(timeActiveAttack);
        shootPoint.gameObject.SetActive(false);
        //set the nextTime for attack (cooldown) afther teh attack is finished
        nextFire = Time.time + fireCD;
        Debug.Log("next in " + nextFire);
        //end the attack, can move again
        if (enemyIa && enemyIa.canMove) enemyIa.moving = true; // is is a moving enemy, re-activate the moving skill
        Debug.Log("END ATTACK");
    }

    protected override void OnStarting()
    {
        //create the hitboxlogic to spam damage to the player
        ddealer = shootPoint.gameObject.AddComponent<DamageDealer>();
        ddealer.damage = this.damage;
        ddealer.canDamage = true;
        enemyIa = GetComponent<EnemyIA>();
    }


    protected override void OnUpdating()
    {

    }
}