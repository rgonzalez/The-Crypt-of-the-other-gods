using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SubScene
{
    public GameObject scene;
    public List<GameObject> panels; //list of panels ordered, each panel will check with every enter or click
}
public class intro : MonoBehaviour
{

    public List<SubScene> scenes; //list ordered of scenes
    public string nextScene;

    //the first scene and the first panel is enabled by default.
    //then every Enter or click will change to the next panel, or scene (if there is not more panels in the actual scene)
    //when all panels ends, change to the scene
    // Start is called before the first frame update

    private int sceneIndex =0;
    private int panelIndex = 0;
    void Start()
    {

        if (scenes.Count > 0)
        {

            scenes[0].scene.SetActive(true);
            if (scenes[0].panels.Count > 0)
            {
                scenes[0].panels[0].SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || 
            Input.GetKeyDown(KeyCode.Return) || 
            Input.GetKeyDown(KeyCode.Mouse0))
        {
            Next();
        }
    }

    public void Next()
    {
        //first try move panel
        if (scenes[sceneIndex].panels.Count > (panelIndex + 1))
        {
            //there is more scenes
            scenes[sceneIndex].panels[panelIndex].SetActive(false);
            panelIndex++;
            scenes[sceneIndex].panels[panelIndex].SetActive(true);
        } else
        {
            //end of panel, move to next scene or change map
            if (scenes.Count > (sceneIndex + 1))
            {
                //there is more scenes
                if (scenes[sceneIndex].panels.Count > 0)
                {
                    scenes[sceneIndex].panels[panelIndex].SetActive(false);
                }
                scenes[sceneIndex].scene.SetActive(false);
                sceneIndex++;
                panelIndex = 0;
                scenes[sceneIndex].scene.SetActive(true);
                if (scenes[sceneIndex].panels.Count > 0)
                {
                    scenes[sceneIndex].panels[0].SetActive(true);
                }
            } else
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}
