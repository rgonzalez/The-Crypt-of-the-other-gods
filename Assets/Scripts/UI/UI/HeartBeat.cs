using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeat : MonoBehaviour
{

    public float bigScale = 1.5f;

    public float minScale = 1f;

    public float secondsToBig = 2f;
    public float secondsToMin = 1f;
    public bool isActive = false;
    private float nextHeartBeat = 0f;
    private bool isBig = false;
    private RectTransform rect;
    private Vector2 originalSize;
    // Start is called before the first frame update
    void Start()
    {        
        rect = GetComponent<RectTransform>();
        originalSize = rect.sizeDelta;
        isBig = false;
        rect.sizeDelta = originalSize * minScale;
        nextHeartBeat = Time.time + secondsToBig;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (isBig)
            {
                if (Time.time > nextHeartBeat)
                {
                    rect.sizeDelta = originalSize * minScale;
                    isBig = false;
                    nextHeartBeat = Time.time + secondsToMin;
                }
            }
            else
            {
                if (Time.time > nextHeartBeat)
                {
                    rect.sizeDelta = originalSize * bigScale;
                    isBig = true;
                    nextHeartBeat = Time.time + secondsToBig;
                }
            }
        }
    }
}
