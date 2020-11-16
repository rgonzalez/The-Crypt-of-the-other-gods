using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{

    public GameObject key;
    public GameObject keySpawn;
	public Doorway[] doorways;
	public MeshCollider meshCollider;

    // the walls ordered by side to clean the mesh
    public GameObject[] topWalls; 
    public GameObject[] botWalls; 
    public GameObject[] leftWalls; 
    public GameObject[] rightWalls;

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
            foreach(GameObject wall in botWalls)
            {
                MeshRenderer rendered = wall.GetComponent<MeshRenderer>();
                if (rendered)
                    rendered.enabled = false;
            }
        } else if (transform.rotation.eulerAngles.y > 85 && transform.rotation.eulerAngles.y < 100)
        {
            //rotate 1/4 to right, right walls must dessapear
            foreach (GameObject wall in rightWalls)
            {
                MeshRenderer rendered = wall.GetComponent<MeshRenderer>();
                if (rendered)
                    rendered.enabled = false;
            }
        } else if (transform.rotation.eulerAngles.y > 150 && transform.rotation.eulerAngles.y < 200)
        {
            // the room is full rotated
            foreach (GameObject wall in topWalls)
            {
                MeshRenderer rendered = wall.GetComponent<MeshRenderer>();
                if (rendered)
                    rendered.enabled = false;
            }
        } else if (transform.rotation.eulerAngles.y > 200 && transform.rotation.eulerAngles.y < 300)
        {
            //rotated to left
            foreach (GameObject wall in leftWalls)
            {
                MeshRenderer rendered = wall.GetComponent<MeshRenderer>();
                if (rendered)
                    rendered.enabled = false;
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

}
