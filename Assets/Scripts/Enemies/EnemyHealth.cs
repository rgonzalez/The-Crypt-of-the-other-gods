using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{

    public int maxHealth = 100;
    public int actualHealth = 100;
    public bool alive = true;
    private Animator animator;
    private NavMeshAgent agent;
    private EnemyIA enemyIA;
    private EnemyWeapon weapon;
    private Rigidbody rigidbody;
    private Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        actualHealth = maxHealth;
        alive = true;
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<EnemyWeapon>();
        enemyIA = GetComponent<EnemyIA>();
        agent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();


        if (animator)
        {
            animator.SetBool("dead", false);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// function called by other sources of damage (explosions, bullets..)
    /// </summary>
    /// <param name="damage">The damage to apply to the current gameObject</param>
    public void Damage(int damage)
    {
        Debug.Log(" DAMAGE IN ENEMY " + damage);
        actualHealth -= damage;
        if (actualHealth <= 0)
        {
            if (animator)
            {
                animator.SetBool("dead", true);
                animator.SetBool("isMoving", false);
            }
            if (agent) agent.enabled = false;
            if (enemyIA) enemyIA.enabled = false;
            if (weapon) weapon.enabled = false;
            if (collider) collider.enabled = false;
            if (rigidbody) Destroy(rigidbody);
      
        }
    }

    public bool Heal(int heal)
    {
        if (actualHealth < maxHealth)
        {
            actualHealth += heal;
            if (actualHealth > maxHealth)
            {
                actualHealth = maxHealth;
            }
            return true;
        }
        return false; // isnt healed, because was full HP
    }

    /// <summary>
    /// the function Die will destroy the scripts associated to the object
    /// can be called by animation Event or some coroutine
    /// </summary>
    /// <returns></returns>
    public void Die()
    {
        Destroy(this);
    }
}
