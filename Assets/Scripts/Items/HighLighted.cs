using UnityEngine;
using System.Collections;

//change the color of the item when the player touch it
public class HighLighted : MonoBehaviour
{

    Material m_Material;
    public bool disabled = false;
    // Use this for initialization
    void Start()
    {
        m_Material = GetComponent<Renderer>().material;
    }
 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER) && !disabled)
        {
            //change to visible
            if (m_Material)
            { 
                //GLOW!! seems pickable!
                m_Material.SetColor("_EmissiveColor", Color.green);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            SetNormalColor();
        }
    }

    private void SetNormalColor()
    {
        if (m_Material)
        {
            m_Material.SetColor("_EmissiveColor", Color.black);
        }
    }

    public void Disable()
    {
        disabled = true;
        SetNormalColor();
    }
}
