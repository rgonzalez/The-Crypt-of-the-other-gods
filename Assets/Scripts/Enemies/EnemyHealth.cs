using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{


    public int maxHealth = 100;

    public int actualHealth = 100;
    public bool alive = true;
    private Animator animator;
    private AbstractWeapon playerWeapon;
    private GameObject actualWeapon;

    public RoomManager room; // in case of enemy, must notify in case of dead

    public Image healthBar; //health bar in case of enemy healthBar

    public GameObject canvas; // the canvas Object to show/hide the health
    private float nextHideTime= 0f; //the next time to hide the canvas is can be hided
    public float secondsShowHealth = 2f;

    public ParticleSystem blood;
    public EnemyIA enemyIA;
    private NavMeshAgent agent;
    private Collider collider;
    private Rigidbody rigidbody;
    public int experience = 0;

    private AudioSource audioSource;

    public AudioClip hurtAudio;
    public AudioClip dieAudio;
    // Start is called before the first frame update
    void Start()
    {
        if (canvas) canvas.SetActive(false);
        actualHealth = maxHealth;
        alive = true;
        //player config
        animator = GetComponent<Animator>();
        playerWeapon = GetComponentInChildren<AbstractWeapon>();
        AbstractWeapon w = transform.GetComponentInChildren<AbstractWeapon>();
        agent = GetComponent<NavMeshAgent>();
        enemyIA = GetComponent<EnemyIA>();
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1f;

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
        if (Time.time > nextHideTime && canvas != null && canvas.activeSelf)
        {
            canvas.SetActive(false);
        }
    }

    /// <summary>
    /// function called by other sources of damage (explosions, bullets..)
    /// </summary>
    /// <param name="damage">The damage to apply to the current gameObject</param>
    public void Damage(int damage)
    {
        if (blood) blood.Play();
        actualHealth -= damage;
        if (healthBar)
        {
      
            if (canvas)
            {
                canvas.SetActive(true);
                nextHideTime = Time.time + secondsShowHealth;
            }
            //calculate the actual life in 0-1 fill
            float actualbarHealth = (float)actualHealth / (float)maxHealth;
            healthBar.fillAmount = actualbarHealth;

        }
        if (actualHealth <= 0) //DEAD CASE!
        {
            if (animator)
            {
                animator.SetBool("dead", true);
                animator.SetBool("isMoving", false);
            }
            if (canvas) canvas.SetActive(false);
            if (gameObject.CompareTag(Constants.TAG_ENEMY))
            {
                if (ExperienceManager.instance && this.experience > 0)
                {
                    Debug.Log("ADD exp");
                    ExperienceManager.instance.AddExperience(this.experience);
                }
                //EnemyConfig
                if (room)
                {
                    room.NotifyDead();
                }
                if (audioSource && dieAudio)
                {
                    audioSource.PlayOneShot(dieAudio);
                }
                //the room must be set by the room itself
                Die();
            }
        } else
        {
            if (audioSource && hurtAudio)
            {
                audioSource.PlayOneShot(hurtAudio);
            }
        }

    }

    /// <summary>
    ///  Heal the actual gameobject if the actual life is < max
    /// </summary>
    /// <param name="heal"> ammount of health to add</param>
    /// <returns>TRUE if the gameObject has been healed, FALSE if was maxhealth</returns>
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
        return false;
    }


    /// <summary>
    /// the function Die will destroy the scripts associated to the object
    /// can be called by animation Event or some coroutine
    /// </summary>
    /// <returns></returns>
    public void Die()
    {
        if (enemyIA) Destroy(enemyIA);
        if (agent) Destroy(agent);
        if (collider) Destroy(collider);
        if (canvas) Destroy(canvas);
        if (rigidbody)
        {
            rigidbody.freezeRotation = true;
            rigidbody.isKinematic = true;
        }
        Destroy(this);
    }

}
