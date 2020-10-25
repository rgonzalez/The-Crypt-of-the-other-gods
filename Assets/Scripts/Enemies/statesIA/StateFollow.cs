using UnityEngine;
using System.Collections;

public class StateFollow : IStateEnemy
{

    private readonly EnemyIA enemy;

    public StateFollow(EnemyIA enemy)
    {
        this.enemy = enemy;
    }

    public void ToFiringState()
    {
        enemy.actualState = enemy.firingState;
    }

    public void ToFollowState()
    {
        //actual State
    }

    public void ToPatrolState()
    {
        enemy.actualState = enemy.patrolingState;
    }

    public void UpdateState()
    {
        if (enemy.moving && enemy.canMove)
        {
            this.enemy.agent.enabled = true;
            this.enemy.agent.SetDestination(enemy.target.transform.position);
        }
        if (enemy.actualDistance > enemy.followRange)
        {
            ToPatrolState();
        } else if (enemy.actualDistance < enemy.shootRange)
        {
            ToFiringState();
        }
    }

}
