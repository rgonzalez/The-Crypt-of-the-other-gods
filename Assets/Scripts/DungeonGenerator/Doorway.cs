using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
    //this door can have multiple REAL doors associated, that stay when the Quad is destroyed
    // if the Quad stays, is a blocked door, and must destroy the usable doors
    public List<UsableDoor> doorsAssociated = new List<UsableDoor>();
	void OnDrawGizmos ()
	{
		Ray ray = new Ray (transform.position, transform.rotation * Vector3.forward);

		Gizmos.color = Color.red;
		Gizmos.DrawRay (ray);
	}

    public void DestroyRealDoors()
    {
        foreach(UsableDoor door in doorsAssociated)
        {
            Destroy(door.gameObject);
        }
    }
}
