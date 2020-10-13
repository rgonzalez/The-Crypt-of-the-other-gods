using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponType", order = 1)]
public class WeaponType : ScriptableObject
{

    [Header("Bullet")]
    [Tooltip("The prefab for the bullet")]
    [SerializeField]
    public GameObject bulletPrefab = null;

    [Header("Firing")]
    [SerializeField]
    [Tooltip("Time between bullets in seconds")]
    public float fireTime = 1.0f;

    [SerializeField]
    public int ammoClip = 30;

    [SerializeField]
    public float reloadTime = 1.0f;

    [SerializeField]
    public float recoilY = 1.0f;

    [SerializeField]
    public float recoilX = 1.0f;


    [Header("Effects")]
    [SerializeField]
    public GameObject muzzle = null;

}