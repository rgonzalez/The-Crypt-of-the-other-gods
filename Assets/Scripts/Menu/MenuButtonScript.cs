using UnityEngine;
using System.Collections;

public class MenuButtonScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool hitted = false;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.name == gameObject.name) {
                gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                hitted = true;
            } 
        }
        if (!hitted)
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
