using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Image healthBar; //health bar in case of enemy healthBar

    public AudioClip hurtAudio;
    public AudioClip dieAudio;
    public AudioClip healAudio;

    private AudioSource audioSource;

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
        StartCoroutine(InitializeHealthPanel());

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
        if (actualHealth <=0)
        {
            return; // is already dead
        }
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
                KillPlayer();
            }
        } else
        {
            if (audioSource && hurtAudio)
            {
                audioSource.PlayOneShot(hurtAudio);
            }
        }
        //if player update UI
        if (gameObject.CompareTag(Constants.TAG_PLAYER))
        {
            UpdateHealth();
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
            if (audioSource && healAudio) audioSource.PlayOneShot(healAudio);
            actualHealth += heal;
            if (actualHealth > maxHealth)
            {
                actualHealth = maxHealth;
            }
       
            //if player update UI
            if (gameObject.CompareTag(Constants.TAG_PLAYER))
            {
                UpdateHealth();
            }

            return true;
        }
        return false;
    }


    private void UpdateHealth()
    {
        if (UIManager.instance != null)        {
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
        Destroy(gameObject);
    }

    IEnumerator ShowDeadPanel()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0.0f;
        UIManager.instance.ShowDeadPanel();
    }


    IEnumerator InitializeHealthPanel()
    {
        yield return new WaitForSecondsRealtime(1f);
        UpdateHealth();
    }

    void KillPlayer()
    {
        // Player Config
        if (movePlayer) movePlayer.enabled = false;
        if (playerWeapon) playerWeapon.enabled = false;
        if (actualWeapon) actualWeapon.SetActive(false);
        //stop the time
        if (audioSource && dieAudio)
        {
            audioSource.PlayOneShot(dieAudio);
        }
        //save the experience 
        if (ExperienceManager.instance != null)
        {
            ExperienceManager.instance.SaveExp();
        }
        StartCoroutine(ShowDeadPanel());
       
    }
}
