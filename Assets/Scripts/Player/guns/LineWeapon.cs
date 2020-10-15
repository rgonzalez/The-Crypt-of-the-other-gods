using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWeapon : MonoBehaviour
{

    public float fireCD = 30f;
    public GameObject muzzleFlashPrefab;
    public LineRenderer lineRenderer;
    private GameObject muzzleFlash;

    public float distance = 10f;

    //variables set by Script
   // public GameObject muzzleCannon;


    private float nextFire = 0f;
    private bool firing;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (muzzleFlashPrefab && muzzleFlash == null)
        {
            muzzleFlash = Instantiate(muzzleFlashPrefab, transform);
            muzzleFlash.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {

            firing = true;
        }
        else
        {
            firing = false;
            if (muzzleFlash)
            {
            //    muzzleFlash.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (firing)
        {
            if (Time.time > nextFire)
            {
                if (muzzleFlash != null)
                {
                    Debug.Log("ACTIVE!");
                    muzzleFlash.SetActive(true);
                    StartCoroutine(DisableMuzzle());
                }

                lineRenderer.SetVertexCount(2);
                lineRenderer.SetPosition(0, transform.position);
                Plane plane = new Plane(Vector3.up, transform.position);
                float distance;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 worldPosition ;
                if (plane.Raycast(ray, out distance))
                {
                    worldPosition = ray.GetPoint(distance);
                    worldPosition = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
                  
                    Debug.Log("transform" + transform.position  + " worldPosition " + worldPosition);
                    //   transform.LookAt(worldPosition);
                    //  transform.TransformDirection(Vector3.forward) <- 2º parameter raycast
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(transform.position, worldPosition - transform.position, out hit, distance))
                    {
                        Debug.DrawRay(transform.position, worldPosition - transform.position, Color.yellow, 10f);
                        lineRenderer.SetPosition(1, hit.point);
                        Debug.Log("Did Hit");
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, worldPosition - transform.position, Color.white, 10f);
                        Debug.Log("Did not Hit " + hit.point);
                        Vector3 pos = (worldPosition - transform.position) * distance;
                        lineRenderer.SetPosition(1, pos);
                    }
                }
                nextFire = Time.time + fireCD;
            } else
            {
                lineRenderer.SetVertexCount(0);
                if (muzzleFlash)
                {
                   // muzzleFlash.SetActive(false);
                }
            }
        } else
        {
            //lineRenderer.SetVertexCount(0);
        }
    }
    IEnumerator DisableMuzzle()
    {
        yield return new WaitForSeconds(0.3f);
        muzzleFlash.SetActive(false);
    }
}
