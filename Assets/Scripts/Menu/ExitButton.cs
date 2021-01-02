using UnityEngine;
using System.Collections;

public class ExitButton : MonoBehaviour
{
 
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            bool hitted = false;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name == gameObject.name)
                {
                    Debug.Log("exit");
                    Application.Quit();
                }
            }
        }
    }
}
