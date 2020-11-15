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
        Plane plane = new Plane(Vector3.up, 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
        }
        // in the same Y axis
        Vector3 pos = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
        transform.LookAt(pos);

        Vector3 lookDirection = new Vector3(Input.GetAxisRaw("RightHoriz"), 0, Input.GetAxisRaw("RightVert"));
        if (lookDirection.x > 0.1f || lookDirection.x <- 0.1f || lookDirection.z < -0.1f || lookDirection.z > 0.1f)
        {
            lookDirection.z = -lookDirection.z;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
