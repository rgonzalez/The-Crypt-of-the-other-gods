using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookMouse : MonoBehaviour
{

    public Vector3 worldPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }
        
        // in the same Y axis
        Vector3 pos = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
        transform.LookAt(pos);

    }

   /* void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(ray);
        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);

        Gizmos.DrawRay(ray2);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(worldPosition.x, worldPosition.y, worldPosition.z), 0.2f);
       // Gizmos.DrawCube(transform.position, new Vector3(5, 0.1f, 5));
    }*/
}
