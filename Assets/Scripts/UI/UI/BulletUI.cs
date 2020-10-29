using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletUI : AbstractUIAmmo
{

    public GameObject normalBulletPrefab;
    public GameObject perfectBulletPrefab;

    public GameObject clipSPawn; // the transform where instantiate the bullets (can have a layour, grid..)
    private List<GameObject> bullets = new List<GameObject>(); // the list of bullets, contains perfect and normal bullets

    public override void Reload(int perfectAmmo, int actualAmmo, int maxClip)
    {
         if (bullets.Count > 0)
        {
            //already have ammo instantiated, just clean all to clear the perfect ammo just in case
            foreach (GameObject bullet in bullets)
                Destroy(bullet);
            bullets.Clear();
        }

         //add normal bullets
         for (int i = 0; i < (actualAmmo - perfectAmmo); i++)
        {
            GameObject bullet = Instantiate(normalBulletPrefab, clipSPawn.transform);
            bullets.Add(bullet);
        }
        //now the perfect Ammo in case, then the normal Ammo
        for (int i = 0; i < perfectAmmo; i++)
        {
            GameObject perfectBullet = Instantiate(perfectBulletPrefab, clipSPawn.transform);
            bullets.Add(perfectBullet);
        }
    }

    public override void Shoot(int wastedAmmo)
    {
        //ADD ANIMATION FOR SHOOT
        if (wastedAmmo <= bullets.Count)
        {
            for (int i = 0; i < wastedAmmo; i++)
            {
                int pos = bullets.Count - 1;
                Destroy(bullets[pos]);
                bullets.RemoveAt(pos);
            }
        }
    }

    protected override void OnStart()
    { 
    }

    protected override void OnUpdate()
    { 
    }

 
}
