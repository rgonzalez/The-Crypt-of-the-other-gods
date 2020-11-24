using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The enemy weapons can be of multiple forms (bullets, melee attacks..) so we create an abstract class
/// to be used by the Enemy 
/// </summary>
public  abstract class EnemyWeapon : MonoBehaviour
{

    public float fireCD = 30f;
    public int damage = 2;
    public Transform target;
    public float nextFire = 0;
    public float recoilX = 1f;
    public float recoilY = 0.5f;

    public Transform shootPoint;

    public AudioSource audioSource;

    public AudioClip shootAudio;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OnStarting();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdating();
    }

    public abstract void Shoot();
    public abstract void PrepareShoot(); //the var for animator
    protected abstract void OnStarting();
    protected abstract void OnUpdating();

}
