using UnityEngine;

public class EndRoom : Room
{
    public string nextLevel; //set by the Level Generator


    public void SetNextLevel(string nextLevel)
    {
        //set the trigger to move
        this.nextLevel = nextLevel;

        SceneTriggerer endSceneTriggerer = transform.GetComponentInChildren<SceneTriggerer>();
            if (endSceneTriggerer)
                endSceneTriggerer.nextScene = nextLevel;
    }
}