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

    private EnemyWeapon weapon;
    private int lookDirection = 1;
    private Animator animator;
    public bool active = true; //is the IA active?
    public bool canMove = true; // can move? or is a static enemy?

    [HideInInspector]
    public bool moving = true; //an enemy can be blocked (if canMove = true) in some conditions
                               //for example, his weapon
    [HideInInspector]
    public bool attacking = false; //an enemy can be blocked (if canMove = true) in some conditions
                               //for example, his weapon
    public NavMeshAgent agent;
    public GameObject target; //the gameobject target (sure... the player)

    public float shootRange; //range attack
    public float followRange; // range to follow (enter in shootMode)
    public float coolDownAttack; //time between attacks

    public float t = 4; // interception time

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
        animator = GetComponent<Animator>();
        weapon = GetComponent<EnemyWeapon>();
        firingState.weapon = weapon;
        //load the agent for load it in the states
        agent = GetComponent<NavMeshAgent>();
        moving = canMove; // set the state of moving, if the enemy canMove or is a static
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
            if (target)
            {
                this.actualDistance = Vector3.Distance(transform.position, target.transform.position);
                if (transform.position.x < target.transform.position.x) //direction of the target in X
                {
                    lookDirection = 1;
                } else
                {
                    lookDirection = -1;
                }
                transform.localScale =  new Vector3( Mathf.Abs(transform.localScale.x) * lookDirection, transform.localScale.y, transform.localScale.z);
            } else
            {
                target = GameObject.FindGameObjectWithTag("Player");
                this.actualDistance = 0;
            }
            if (actualState != null) //just check in case
            {
                actualState.UpdateState();
            } else
            {
                Debug.Log("Error in some state change, REDIRECTING TO PATROL STATE AT " + gameObject.name);
                actualState = patrolingState;
            }

            animator.SetBool("run", moving && (agent.velocity.magnitude != 0f));           
            animator.SetBool("attack", attacking);

        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, followRange);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, shootRange);

    }
}
