using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuSceneScript : MonoBehaviour
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Light light;
    [Tooltip("Minimum random light intensity")]
    public float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    public float maxIntensity = 999f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 5;
    public GameObject zombie;
    public float onLight;
    public float offLight;
    public float minOnLight;
    public float minOffLight;

    private float nextOff;
    private float nextOn;
    private bool on = true;
    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> smoothQueue;
    float lastSum = 0;

    public bool canOff = true;


    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
        // External or internal light?
        if (light == null)
        {
            light = GetComponent<Light>();
        }
        nextOn = Random.Range(0, onLight);
    }

    void Update()
    {
        if (on)
        {
            if (Time.time >= nextOn && canOff)
            {
                nextOff = Time.time + Random.Range(minOffLight, offLight);
                on = false;
            }
            else
            {
                if (zombie)
                    zombie.SetActive(true);
                if (light == null)
                    return;

                // pop off an item if too big
                if (smoothQueue != null && smoothing != null)
                {
                    while (smoothQueue.Count >= smoothing)
                    {
                        lastSum -= smoothQueue.Dequeue();
                    }
                }

                // Generate random new item, calculate new average
                float newVal = Random.Range(minIntensity, maxIntensity);
                if (smoothQueue != null)
                {
                    smoothQueue.Enqueue(newVal);
                    lastSum += newVal;

                    // Calculate new smoothed average
                    light.intensity = lastSum / (float)smoothQueue.Count;
                }
            }
        } else
        {
            if (Time.time >= nextOff)
            {
                nextOn = Time.time + Random.Range(minOnLight, onLight);
                on = true;
            }
            if (zombie)
                zombie.SetActive(false);
            light.intensity = 0;
        }
    }
}
