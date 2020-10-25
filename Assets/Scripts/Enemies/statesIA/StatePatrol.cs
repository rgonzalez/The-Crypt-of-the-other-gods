using UnityEngine;
using UnityEditor;

public class StatePatrol: IStateEnemy
{
    private readonly EnemyIA enemy;

    public StatePatrol(EnemyIA enemy)
    {
        this.enemy = enemy;
    }

    public void ToFiringState()
    {
        enemy.actualState = enemy.firingState;
    }

    public void ToFollowState()
    {
        enemy.actualState = enemy.followingState;
    }

    public void ToPatrolState()
    {
        //already in state
    }

    public void UpdateState()
    {
        if (enemy.moving && enemy.canMove)
        {
            enemy.agent.enabled = true;
            //try to intercept the actual direction of the target
            Vector3 posB = enemy.target.transform.position;
            Vector3 velB = enemy.target.gameObject.GetComponent<CharacterController>().velocity;
            Vector3 posA = enemy.transform.position;
            // use the formula for interception
            Vector3 velA = velB + (posB - posA) / enemy.t;
            enemy.agent.velocity = velA;
            //move to the interception point
        }
        //check if the enemy has other ranges to move
        if (enemy.actualDistance < enemy.shootRange)
        {
            ToFiringState();
        }
        else if (enemy.actualDistance < enemy.followRange)
        {
            ToFollowState();
        }

    }
}