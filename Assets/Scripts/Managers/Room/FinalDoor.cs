using UnityEngine;
using System.Collections;

public class FinalDoor : MonoBehaviour
{
    public GameObject[] doors;
    //This represent the Final Door, only Open if the user has all the keys
    // Use this for initialization
    private bool closed = true;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (LevelBuilder.instance && other.CompareTag(Constants.TAG_PLAYER))
        {
            if (closed)
            {
                if (LevelBuilder.instance.AllKeysPicked())
                {
                    closed = false;
                    UIManager.instance.ShowKeysInfo(true);
                    OpenDoor();
                }
                else
                {
                    UIManager.instance.ShowKeysInfo(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (LevelBuilder.instance && other.CompareTag(Constants.TAG_PLAYER))
        {
            UIManager.instance.CleanPanel();
        }
    }

    private void OpenDoor()
    {
       foreach(GameObject door in doors)
        {
            Destroy(door);
        }
    }
}
