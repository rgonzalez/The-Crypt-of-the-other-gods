using UnityEngine;
using System.Collections;

//change the color of the item when the player touch it
public class HighLighted : MonoBehaviour
{

    Material m_Material;
    // Use this for initialization
    void Start()
    {
        m_Material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            //change to visible
            if (m_Material)
            {
                Debug.Log("SET COLOR");
                //GLOW!! seems pickable!
                m_Material.SetColor("_EmissiveColor", Color.green);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            if (m_Material)
            {
                m_Material.SetColor("_EmissiveColor", Color.black);
            }
        }
    }
}
