using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateEnemy
{
    void UpdateState();
    void ToFiringState();
    void ToFollowState();
    void ToPatrolState();
}