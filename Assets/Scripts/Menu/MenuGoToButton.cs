using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class MenuGoToButton : MonoBehaviour
{

    public PlayableDirector director;
    public PlayableAsset playable;
    public MoveBetweenPoints follow;
    public int direction = 1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            bool hitted = false;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name == gameObject.name)
                {
                    if (follow)
                        follow.direction = direction;
                    director.Play(playable);
                }
            }
            if (!hitted)
            {
                gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
