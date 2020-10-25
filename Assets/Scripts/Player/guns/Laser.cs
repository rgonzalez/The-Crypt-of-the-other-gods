using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{


    public float laserDistance = 9999;
    public LineRenderer mLineRenderer;
    public string bounceTag;
    public int maxBounce = 2;
    private float timer = 0;
    private bool firing = false;
    public int damage = 200;

    // Use this for initialization
    void Start()
    {
        mLineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firing)
        {
            if (timer < 0)
            {
                mLineRenderer.enabled = false;
                firing = false;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }

    public void FireLaser(Vector3 startPosition, Vector3 direction)
    {
        if (mLineRenderer == null)
        {
            mLineRenderer = GetComponent<LineRenderer>();
        }
        timer = 0.2f;
        firing = true;
        StartCoroutine(GenerateLaser(startPosition, direction));
    }
    IEnumerator GenerateLaser(Vector3 startPosition, Vector3 direction)
    {
        mLineRenderer.enabled = true;
        int laserReflected = 1; //How many times it got reflected
        int vertexCounter = 1; //How many line segments are there
        bool loopActive = true; //Is the reflecting loop active?

        Vector2 laserDirection = transform.forward; //direction of the next laser
        Vector2 lastLaserPosition = transform.localPosition; //origin of the next laser
        if (startPosition != null)
        {
            mLineRenderer.SetPosition(0, startPosition);
            lastLaserPosition = startPosition;
        }
        else
        {
            mLineRenderer.SetPosition(0, transform.position);
        }
        if (direction != null)
        {
            laserDirection = direction;
        }

        mLineRenderer.positionCount = 1;
        //RaycastHit hit;

        while (loopActive)
        {
            RaycastHit2D hit = Physics2D.Raycast(lastLaserPosition, laserDirection, laserDistance);
            if (hit != null)
            {
                laserReflected++;
                vertexCounter += 3;
                mLineRenderer.positionCount = vertexCounter;
                mLineRenderer.SetPosition(vertexCounter - 3, Vector3.MoveTowards(hit.point, lastLaserPosition, 0.1f));
                mLineRenderer.SetPosition(vertexCounter - 2, hit.point);
                mLineRenderer.SetPosition(vertexCounter - 1, hit.point);
                mLineRenderer.startWidth = 0.1f;
                mLineRenderer.endWidth = 0.1f;
                lastLaserPosition = hit.point + hit.normal;
                laserDirection = Vector2.Reflect(laserDirection, hit.normal);
                Debug.Log("col" + hit);
                //the laser some times can not hit a wall/enemy
                if (hit.collider && hit.collider.gameObject)
                {
                    hit.collider.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                laserReflected++;
                vertexCounter++;
                mLineRenderer.positionCount = vertexCounter;
                mLineRenderer.SetPosition(vertexCounter - 1, lastLaserPosition + (laserDirection.normalized * laserDistance));

                loopActive = false;
            }
            if (laserReflected > maxBounce)
            {
                loopActive = false;
            }
        }
        //the laser is alive for a time
        yield return null;
        Destroy(gameObject, 1f);

    }
}
