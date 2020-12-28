using UnityEngine;
using System.Collections;

public class UsableDoor : MonoBehaviour
{
    private AudioSource audioSource;
    public GameObject door;
    public float speed = 4f;
    public Transform openPos; //the gameobject that pos the object in opened pos
    public Vector3 closedPos; // the initial pos
    public bool startOpened = true;
    public bool moving = false;
    public bool opening = false;
    // Use this for initialization

    public Vector3 pos1;
    public Vector3 pos2;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        closedPos = door.transform.position;
        if (startOpened)
        {
            door.transform.position = openPos.position;
            opening = false;
            door.SetActive(false);
        } else
        {
            opening = true;
        }
        SetPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (door.transform.position == pos2)
            {
                moving = false;
                if (opening)
                //the door is opened, we must deactive, to not molest the user
                {
                    door.SetActive(false);
                } else
                {
                    door.SetActive(true);
                }
                opening = !opening;
            
                SetPos();
            }
            else
            {
                door.transform.position = Vector3.MoveTowards(door.transform.position, pos2, Time.deltaTime * speed);
            }
        }
    }

    public void MoveDoor()
    {
        SetPos();
        if (audioSource)
        {
            audioSource.Play();
        }
        door.SetActive(true);
        moving = true;
    }

    public void SetPos()
    {
        if (opening)
        {
            pos2 = openPos.position;
            pos1 = closedPos;
        } else
        {
            pos1 = openPos.position;
            pos2 = closedPos;
        }
    }

    public void CloseDoor()
    {
        door.transform.position = openPos.position;
        opening = false;
        MoveDoor();
    }

    public void OpenDoor()
    {
        Debug.Log("open door");
        door.transform.position = closedPos;
        opening = true;
        MoveDoor();
    }
}
