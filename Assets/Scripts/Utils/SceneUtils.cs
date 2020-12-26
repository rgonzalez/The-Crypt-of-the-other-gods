using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtils : MonoBehaviour
{
    
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    //reload the scene by death
    public void ReloadCurrentScene()
    {

        // disable the inventory, weaponManager...
        GameObject levelManager = GameObject.FindGameObjectWithTag(Constants.TAG_LEVELGENERATOR);
        //detroy the managers to reset them
        if (levelManager)
        {
            LevelBuilder levelBuilder = levelManager.GetComponent<LevelBuilder>();
            if (levelBuilder)
            {
                //the levelGenerator can destroy all the managers
                levelBuilder.DestroyManagers();
            }
        }
        StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Debug.Log("RESTART LEVEL");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
