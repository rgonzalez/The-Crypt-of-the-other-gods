using UnityEngine;

public class Room : MonoBehaviour
{
	public Doorway[] doorways;
	public MeshCollider meshCollider;

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
}
