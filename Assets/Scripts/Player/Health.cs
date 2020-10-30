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

    public RoomManager room; // in case of enemy, must notify in case of dead

    // Start is called before the first frame update
    void Start()
    {
        actualHealth = maxHealth;
        alive = true;
        //player config
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
        if (actualHealth <= 0) //DEAD CASE!
        {
            if (animator)
            {
                animator.SetBool("dead", true);
                animator.SetBool("isMoving", false);
            }
            if (gameObject.CompareTag(Constants.TAG_PLAYER))
            {
                // Player Config
                if (movePlayer) movePlayer.enabled = false;
                if (playerWeapon) playerWeapon.enabled = false;
                if (actualWeapon) actualWeapon.SetActive(false);
            }
            if (gameObject.CompareTag(Constants.TAG_ENEMY))
            {
                //EnemyConfig
                if (room)
                {
                    //the room must be set by the room itself
                    room.NotifyDead();
                }
            }
        }
        UpdateHealth();
    }

    public void Heal(int heal)
    {
        if (actualHealth < maxHealth)
        {
            actualHealth += heal;
            if (actualHealth > maxHealth)
            {
                actualHealth = maxHealth;
            }
        }
        //if player update UI
        if (gameObject.CompareTag(Constants.TAG_PLAYER))
        {
            UpdateHealth();
        }
    }


    private void UpdateHealth()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.maxHealth = maxHealth;
            UIManager.instance.actualHealth = actualHealth;
            UIManager.instance.ConfigureHealth();
        }
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
