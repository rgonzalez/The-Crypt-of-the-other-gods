using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{

    //TEST PHYSICS PURPOSE
    public Vector3 centerPhysics = Vector3.zero;
    public Vector3 sizePhysics = Vector3.zero;


    public GameObject key;
    public GameObject keySpawn;
	public Doorway[] doorways;

    // the walls ordered by side to clean the mesh
    public MeshRenderer[] topWalls; 
    public MeshRenderer[] botWalls; 
    public MeshRenderer[] leftWalls; 
    public MeshRenderer[] rightWalls;


    public List<Room> connectedRooms = new List<Room>(); // the connected Rooms to this room directly
    // used to activate o disable the rooms

    //THE SURFACES TO MOVE
    public NavMeshSurface[] surfaces;

    public Bounds RoomBounds {
		get {      
            //calculate the bounds about the most left, most right, most bot and most top colliders, so we can have the max 
            // size of the room

            // as the room is componed as top level gameObject, all the elements are childrens; just for check
            Vector3 center = Vector3.zero;
            int elementsFound = 0;
            /* foreach (Transform child in transform)
             {
                 Renderer renderer = child.gameObject.GetComponent<Renderer>();
                 if (renderer)
                 {
                     center += renderer.bounds.center;
                     elementsFound++;
                 }
             }
             center /= elementsFound; //center is average center of children

             //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
             Bounds bounds = new Bounds(center, Vector3.zero);

             foreach (Transform child in transform)
             {
                 Renderer renderer = child.gameObject.GetComponent<Renderer>();
                 if (renderer)
                 {
                     Debug.Log("ADD " + renderer.name);
                     bounds.Encapsulate(renderer.bounds);
                 }
             }*/

            //New system 
            Bounds bounds = new Bounds(transform.position, Vector3.zero);
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach(Renderer renderer in renderers)
            {            
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;

        }
	}

    public void ClearWalls()
    {
        // clear the mesh depending the rotation of the room
        if (transform.rotation.eulerAngles.y > -0.1 && transform.rotation.eulerAngles.y < 0.1)
        {
            //the room is in default position
            foreach(MeshRenderer wall in botWalls)
            {
                wall.enabled = false;
            }
        } else if (transform.rotation.eulerAngles.y > 85 && transform.rotation.eulerAngles.y < 100)
        {
            //rotate 1/4 to right, right walls must dessapear
            foreach (MeshRenderer wall in rightWalls)
            {
                wall.enabled = false;
            }
        } else if (transform.rotation.eulerAngles.y > 150 && transform.rotation.eulerAngles.y < 200)
        {
            // the room is full rotated
            foreach (MeshRenderer wall in topWalls)
            {
                wall.enabled = false;
            }
        } else if (transform.rotation.eulerAngles.y > 200 && transform.rotation.eulerAngles.y < 300)
        {
            //rotated to left
            foreach (MeshRenderer wall in leftWalls)
            {
                wall.enabled = false;
            }
        }
    }
    // place the key in this room
    public void PlaceKey()
    {

        if (keySpawn && key)
        {
            Instantiate(key, keySpawn.transform);
        }
    }


    /// <summary>
    /// Generate the NavMesh in the room
    /// </summary>
    public void BuildNavMesh()
    {
       
        foreach(NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
        }
    }

    //if a room keeps doorways (not connected to other room) we must clean the DoorWay, but not the child, that can be a fake Wall or Door
    public void CleanDoorWays()
    { //the doorways are the Quads, that have child static
        foreach (Doorway doorway in doorways)
        {
            if (doorway.gameObject.active == true)
            {
                //is an active Doorway, enable the child if exists
                doorway.gameObject.SetActive(false);
                if (doorway.transform.childCount > 0)
                {
                    for (int i = 0; i < doorway.transform.childCount; i++)
                    {
                        GameObject fakeWall = doorway.transform.GetChild(i).gameObject;
                        fakeWall.transform.parent = this.transform; //extract from the quad, now is a independent gameobject
                        fakeWall.gameObject.SetActive(true);
                    }
                }
                //the fakewall is ON, but now disable the real doors associated if exists
                doorway.DestroyRealDoors();
            }
        }
    }


    /// <summary>
    /// hide or enable all the rooms connected EXCEPT the 'exception' room if exists
    /// </summary>
    /// <param name="enabled"></param>
    /// <param name="exception"></param>
    private void ChangeStatusConnected(bool enabled, Room exception)
    {
        foreach(Room room in connectedRooms)
        {
            if (exception != room)
                room.gameObject.SetActive(enabled);
        }
    }

    //enable all neighbour rooms, except the 'exception' room
    public void ShowConnectedRooms(Room exception)
    {
        ChangeStatusConnected(true, exception);
    }
    //hide all neighbour rooms, except the 'exception' room
    public void HideConnectedRooms(Room exception)
    {
        ChangeStatusConnected(false, exception);
    }

    //if there is a trigger in this room, it will hide the 2º grade rooms, to optimize:

   // we have rooms: A->B->C->D
   // when the user enters in C, it will say to B and D hide rooms, so A is hide 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            foreach(Room room in connectedRooms)
            {
                room.gameObject.SetActive(true);
                room.HideConnectedRooms(this);
            }
        }
    }
    private void OnDrawGizmos() {

      /*  Gizmos.color = Color.red;
        Handles.Label(transform.position, gameObject.name);
        Gizmos.DrawIcon(centerPhysics, "alertDialog");
        Gizmos.DrawWireCube(centerPhysics, sizePhysics);*/
    }

}
