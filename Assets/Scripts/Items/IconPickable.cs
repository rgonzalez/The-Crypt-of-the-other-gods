using UnityEngine;
using System.Collections;

public class IconPickable : MonoBehaviour
{
    public Transform attachment; //set by the pickable that instantiate this object
    // Use this for initialization
    public float iconHeight = 5f; // also set by the pickable
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position =  new Vector3(attachment.position.x, attachment.position.y + iconHeight, attachment.position.z);

    }
}
