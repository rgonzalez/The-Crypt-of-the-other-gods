using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipUI : MonoBehaviour
{

    Rigidbody2D rigidBody;
    // Start is called before the first frame update
    public float force = 95000f;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.isKinematic = true; //ignore the physics
    }


    // The bullet is installed in the spawn system, so cant Move (is a layout)
    // so we move out of the parent (the layout) and the rigid body can have physics
    public void Unload()
    {
        Vector3 pos = transform.position;
        transform.SetParent(transform.parent.transform.parent);
        transform.position = pos;
        if (rigidBody)
        {
            rigidBody.isKinematic = false;
            rigidBody.AddForce(new Vector2(1,1) * force, ForceMode2D.Impulse);
        }
    }

    public IEnumerator DestroyBullet()
    {
        yield return new WaitForSecondsRealtime(1f);
        Destroy(gameObject);
    }
}
