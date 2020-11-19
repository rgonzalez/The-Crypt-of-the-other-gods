using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{

    public GameObject key;
    public GameObject keySpawn;
	public Doorway[] doorways;

    // the walls ordered by side to clean the mesh
    public MeshRenderer[] topWalls; 
    public MeshRenderer[] botWalls; 
    public MeshRenderer[] leftWalls; 
    public MeshRenderer[] rightWalls;

    //THE SURFACES TO MOVE
    public NavMeshSurface[] surfaces;

    public Bounds RoomBounds {
		get {      
            //calculate the bounds about the most left, most right, most bot and most top colliders, so we can have the max 
            // size of the room

            // as the room is componed as top level gameObject, all the elements are childrens; just for check
            Vector3 center = Vector3.zero;

            foreach (Transform child in transform)
            {
                Renderer renderer = child.gameObject.GetComponent<Renderer>();
                if (renderer)
                    center += renderer.bounds.center;
            }
            center /= transform.childCount; //center is average center of children

            //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
            Bounds bounds = new Bounds(center, Vector3.zero);

            foreach (Transform child in transform)
            {
                Renderer renderer = child.gameObject.GetComponent<Renderer>();
                if (renderer)
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
        Debug.Log("PLACE KEY AT " + gameObject.name);
        if (keySpawn && key)
        {
            Instantiate(key, keySpawn.transform);
            Debug.Log("PLACED KEY " + gameObject.name);
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
    {
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
                        fakeWall.transform.parent = null;
                        fakeWall.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

}
