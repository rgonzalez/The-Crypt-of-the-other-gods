﻿using UnityEngine;
using System.Collections;
[RequireComponent(typeof(SceneUtils))]
public class SceneTriggerer : MonoBehaviour
{
    public bool destroyAllManagers = false; //check to true if want destroy all managers
    public string nextScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            //keys picked to unlock next Level?
            if (LevelBuilder.instance != null)
            {
                if (LevelBuilder.instance.AllKeysPicked())
                {
                    if (ExperienceManager.instance != null)
                    {
                        ExperienceManager.instance.SaveExp();
                    }
                    LoadLevel();
                }
            } else
            {
                LoadLevel();
            }
        }
    }

    private void LoadLevel()
    {
        if (LevelBuilder.instance)
        {
            nextScene = LevelBuilder.instance.nextLevel;
            if (destroyAllManagers)
            {
                LevelBuilder.instance.DestroyManagers();
            }
        }
        GetComponent<SceneUtils>().ChangeScene(nextScene);
    }
}
