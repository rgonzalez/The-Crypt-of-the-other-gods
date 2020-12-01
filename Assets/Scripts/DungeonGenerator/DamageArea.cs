using UnityEngine;
using System.Collections;

public class DamageArea : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        other.SendMessage("Damage", 9999999, SendMessageOptions.DontRequireReceiver);
    }
}
