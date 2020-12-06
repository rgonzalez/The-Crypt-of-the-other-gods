using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLevel : MonoBehaviour
{
    private bool active = true;
      // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER) && active)
        {
            active = false;
            LevelBuilder.instance.PickKey();
            //the keycard is child of a prefab with lights, icons... destory the parent
            Destroy(transform.parent.gameObject);
        }
    }
}
