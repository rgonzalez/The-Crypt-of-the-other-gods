using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtils : MonoBehaviour
{
    
    public void ChangeScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    //reload the scene by death
    public void ReloadCurrentScene()
    {

        Time.timeScale = 1;
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

    //change scene destroying all objects
    public void LoadNewLevel(string scene)
    {
        Time.timeScale = 1;
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
        StartCoroutine(LoadScene(scene));
    }

    IEnumerator LoadScene(string scene)
    {

        yield return new WaitForSecondsRealtime(1.0f); 
        SceneManager.LoadScene(scene);
    }
    private IEnumerator RestartLevel()
    {

        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(1.0f);
        Debug.Log("RESTART LEVEL");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
