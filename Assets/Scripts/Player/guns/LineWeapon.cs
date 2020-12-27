using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWeapon : AbstractWeapon
{

   

    public GameObject lineRendererPrefab; // the lineRenderer that will set the config for the múltiple lineRenderers (color, width..)
    public int numberOfLines = 1;   
    public float distance = 10f;
    public int numberOfBounces = 0;
    private List<LineRenderer> lineRenderers;
             
    /// <summary>
    /// OnStarting initializes the linerenderers
    /// </summary>
    // Start is called before the first frame update
    protected override void OnStarting()
    {
        //so we have now the lineRenderer, we create a list of linerenderers (in children gameobjects) copying the config of the master LineRenderer
        lineRenderers = new List<LineRenderer>();
        for (int i = 0; i < numberOfLines; i++)
        {
            GameObject lr = Instantiate(lineRendererPrefab, transform);
            lineRenderers.Add(lr.GetComponent<LineRenderer>());
        }
    }

    // Update is called once per frame
    protected override void OnUpdating()
    {

    }


    IEnumerator DisableMuzzle()
    {
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
        foreach(LineRenderer lr in lineRenderers)
            lr.positionCount = 0;
    }


    protected override void Shoot()
    {
        if (Time.time > nextFire)
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(true);
                StartCoroutine(DisableMuzzle());
            }
            if (shootAudio)
            {
                audioSource.PlayOneShot(shootAudio);
            }
            ammo -= bulletsPerShoot;
            int actualDamage = damage;
            if (perfectAmmo > 0)
            {
                actualDamage = (int)((float)damage * (float)((float)perfectCritic / (float)100));
                perfectAmmo -= bulletsPerShoot;
            }
            Plane plane = new Plane(Vector3.up, transform.position);
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 worldPosition;
            if (plane.Raycast(ray, out distance))
            {

                //OLD SHOOT MODE
                //worldPosition = ray.GetPoint(distance);
                //worldPosition = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
                //   transform.LookAt(worldPosition);
                //  transform.TransformDirection(Vector3.forward) <- 2º parameter raycast
             


                // now we have to cast a raycast per bullet (lineRenderer) with recoil
                //Debug.Log("worldPosition" + worldPosition);
                //Debug.Log("transform" + transform.position);
                //Vector3 target = worldPosition - transform.position; //original Target position
                
                //Debug.Log("target" + target);
                foreach (LineRenderer lr in lineRenderers)
                {
                    lr.positionCount = 2;
                    lr.SetPosition(0, transform.position);
                    //we set a new target, adding a random recoil
                    //Vector3 newTarget = new Vector3(target.x + Random.Range(-recoilX, +recoilX), target.y + Random.Range(-recoilY, +recoilY), target.z + Random.Range(-recoilX, +recoilX));
                    Vector3 newPos = new Vector3();
                    float randomRangeX = Random.RandomRange(0, +recoilX);
                    float randomRangeY = Random.RandomRange(-recoilY, +recoilY);
                    Quaternion rotation = Quaternion.AngleAxis(randomRangeX, transform.rotation.eulerAngles);

                    //if (Physics.Raycast(transform.position, newTarget, out hit, distance))
                    Vector3 dir = rotation * transform.forward;
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, dir, out hit, distance))
                    {
                        //  Debug.DrawRay(transform.position, newTarget, Color.yellow, 10f);
                        Debug.Log("distance: " + distance);
                        lr.SetPosition(1, hit.point);
                        Debug.Log(hit.rigidbody);
                        Debug.Log("hit point " + hit.point);
                        hit.collider.SendMessage("Damage", actualDamage, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                          Debug.DrawRay(transform.position, dir, Color.white, 10f);
                        Debug.Log("NOT HIT");
                        //Vector3 pos = (newTarget) * distance;
                        //lr.SetPosition(1, pos);
                    }
                }
            }
            nextFire = Time.time + fireCD;
            UIManager.instance.Shoot(ammoType, bulletsPerShoot, ammo);
        } 
    }

    protected override void Reload()
    {

    }
}
