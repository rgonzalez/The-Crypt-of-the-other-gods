using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{



    [HideInInspector] public IStateEnemy actualState;
    [HideInInspector] public StateFire firingState;
    [HideInInspector] public StateFollow followingState;
    [HideInInspector] public StatePatrol patrolingState;
    // Start is called before the first frame update



    public bool active = true; //is the IA active?
    public NavMeshAgent agent;
    public GameObject target; //the gameobject target (sure... the player)

    public float shootRange; //range attack
    public float followRange; // range to follow (enter in shootMode)
    public float coolDownAttack; //time between attacks

    public float t = 4; // interception time
    public float time;
    public float actualDistance; // the distance to the target

    void Awake()
    {
        followingState = new StateFollow(this);
        patrolingState = new StatePatrol(this);
        firingState = new StateFire(this);
        actualState = patrolingState;
    }


    void Start()
    {
        //load the agent for load it in the states
        agent = GetComponent<NavMeshAgent>();
        time = Time.time;
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // the states only are using the AI when the 'active' is true, so the tank cant move until
        // GameManager say it
        if (active)
        {
            //all the states wants to know the distance, 
            this.actualDistance = Vector3.Distance(transform.position, target.transform.position);
            actualState.UpdateState();
        }
    }
}
