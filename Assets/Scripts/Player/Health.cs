using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int maxHealth = 100;

    public int actualHealth = 100;
    public bool alive = true;
    private Animator animator;
    private MovePlayer movePlayer;
    private AbstractWeapon playerWeapon;
    private GameObject actualWeapon;

    // Start is called before the first frame update
    void Start()
    {
        actualHealth = maxHealth;
        alive = true;
        movePlayer = GetComponent<MovePlayer>();
        animator = GetComponent<Animator>();
        playerWeapon = GetComponentInChildren<AbstractWeapon>();
        AbstractWeapon w = transform.GetComponentInChildren<AbstractWeapon>();
        if (w)
        {
            actualWeapon = w.gameObject;
        }
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
        actualHealth -= damage;
        if (actualHealth <= 0)
        {
            if (animator)
            {
                animator.SetBool("dead", true);
                animator.SetBool("isMoving", false);
            }
            if (movePlayer) movePlayer.enabled = false;            
            if (playerWeapon)  playerWeapon.enabled = false;            
            if (actualWeapon) actualWeapon.SetActive(false);
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
