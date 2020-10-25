using UnityEngine;
using System.Collections;

public class LookAtPlayer : MonoBehaviour
{

    Transform target;
    // Use this for initialization
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER);
        if (player)
        {
            target = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.LookAt(target);
        }
    }
}
