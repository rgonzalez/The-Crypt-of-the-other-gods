using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFire : IStateEnemy
{

    private readonly EnemyIA enemy;
    public EnemyWeapon weapon;

    public StateFire(EnemyIA enemy)
    {
        this.enemy = enemy;
    }

    public void ToFiringState()
    {
        // already in this state
    }

    public void ToFollowState()
    {
        enemy.actualState = enemy.followingState;
    }

    public void ToPatrolState()
    {
        enemy.actualState = enemy.patrolingState;
    }

    public void UpdateState()
    {
        // this is a shooting state
        // so we are going to pass the shoot to a script based in the enemy weapon
        // this is useful to change the weapon script of the enemy independently of this script
        enemy.agent.enabled = false; // stop moving
        // is the coolDown ?
        if (Time.time > enemy.time)
        {
            //check Range
            if (weapon)
            {
                RaycastHit hit;
                Vector3 directionTarget = enemy.target.transform.position - enemy.transform.position;
                if (Physics.Raycast(enemy.transform.position,  enemy.transform.TransformDirection(directionTarget), out hit, Mathf.Infinity))
                {
                    weapon.target = enemy.target.transform;
                    //is direct range, shoot
                    weapon.PrepareShoot();
                    weapon.Shoot();

                }
            }
        }

        // after shoot check if the enemy has change state
        if (enemy.actualDistance > enemy.followRange)
        {
            ToPatrolState();
        } else if ( enemy.actualDistance < enemy.shootRange)
        {
            ToFollowState(); //alternates between follow and shoot
        }


    }
}
