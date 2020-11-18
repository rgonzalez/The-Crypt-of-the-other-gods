using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;
    public float speed = 1.0f;
    public int direction = 0; // 1 -> pos1 ->pos2, -1 pos2->pos1
    void Update()
    {
        if (direction != 0)
        {
            Vector3 b;
            if (direction < 0)
            {
                b = pos1.transform.position;
            } else
            {
                b = pos2.transform.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, b, speed * Time.deltaTime ); //(Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f
        }
    }
}
