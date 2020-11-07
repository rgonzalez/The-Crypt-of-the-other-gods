using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuChangeScene : MonoBehaviour
{

    public string scene;
    // Use this for initialization
    void Start()
    {

    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            bool hitted = false;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name == gameObject.name) {
                    SceneManager.LoadScene(scene);
                }
            } 
        }
    }
}
