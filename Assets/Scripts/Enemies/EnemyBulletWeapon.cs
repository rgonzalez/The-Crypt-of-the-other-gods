using UnityEngine;
using System.Collections;

public class EnemyBulletWeapon : EnemyWeapon
{

    public GameObject bulletPrefab;
    public int numberOfBullets = 1;

    public bool usePhysics = false; // if the weapon use physics to push the bullet or not. (for Grenade cases)
    public float bulletForce = 0f; // if the bullet is physics, how much force is applied (grenade force)



    public float timeActiveAttack = 1f; // the time that the attack is active (the hitbox is alive)
                                   // the attackCD will be fired after the end of the attack, if the attack is 2seconds, and the CD
                                   // is 5 seconds, the next attack will be in 7 (2 attack active, wait 5)

    private float endAttack = 0f; // when the actual attack ends (time attack still alive)

    private EnemyIA enemyIa; // the IA of the actual enemy, this weapon disables the move hability of this Enemy while is attacking
                             // so if the enemy changes state (to follow) and the attack still, follow doesnt move



    public override void PrepareShoot()
    {
   
    }

    public override void Shoot()
    {
       if (Time.time > nextFire)
        {
            if (shootAudio && audioSource)
            {
                audioSource.PlayOneShot(shootAudio);
            }
            if (enemyIa) this.enemyIa.attacking = true;
            Vector3 targetPos = transform.position - target.position;
            if (this.enemyIa && this.enemyIa.canMove)
            {
                Debug.Log("MOVE FALSE");
                this.enemyIa.moving = false; //while attacking cant move (if is a moving enemy)
            }
            for (int i = 0; i < numberOfBullets; i++)
            {
                // instantiate a new bullet with the recoil
                Vector3 newTarget = new Vector3(targetPos.x + Random.Range(-recoilX, +recoilX), targetPos.y + Random.Range(-recoilY, +recoilY), targetPos.z + Random.Range(-recoilX, +recoilX));
                Quaternion relativeRotation = Quaternion.LookRotation(newTarget, Vector3.up); // get the rotation to the new target

                // the position instantiation  cant be the enemy, must be with a spacing


                GameObject newBullet = Instantiate(bulletPrefab, shootPoint.position, relativeRotation);
                if (usePhysics)
                {
                    newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletForce);
                }
 
            }
            nextFire = Time.time + fireCD;
            endAttack = Time.time + timeActiveAttack;
            StartCoroutine(DisableAttack());

        }
    }

    IEnumerator DisableAttack()
    {
        yield return new WaitForSecondsRealtime(timeActiveAttack);
        shootPoint.gameObject.SetActive(false);
        //set the nextTime for attack (cooldown) afther teh attack is finished
        nextFire = Time.time + fireCD;
        Debug.Log("next in " + nextFire);
        //end the attack, can move again
        if (enemyIa && enemyIa.canMove)
        {
            Debug.Log("MOVE TRUE");
            enemyIa.moving = true; // is is a moving enemy, re-activate the moving skill
        }
        Debug.Log("END ATTACK");
        if (enemyIa) this.enemyIa.attacking = false;
    }


    protected override void OnStarting()
    {

        enemyIa = GetComponent<EnemyIA>();
    }

    protected override void OnUpdating()
    {
        
    }

}
