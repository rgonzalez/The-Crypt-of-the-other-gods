using UnityEngine;
using System.Collections;
[RequireComponent(typeof(SceneUtils))]
public class SceneTriggerer : MonoBehaviour
{
    public string nextScene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
           SceneUtils sceneUtils = GetComponent<SceneUtils>();
            sceneUtils.ChangeScene(nextScene);
        }
    }
}
