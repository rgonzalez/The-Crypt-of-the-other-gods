using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Ammo script with the content ammo to show in the main weapon equiped, and animations of reload/shoot in the UI
/// </summary>
public abstract class AbstractUIAmmo : MonoBehaviour
{
    //the prefab ammo UI that contains this script, also must have the ammo Text to show 
    public Text text;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }

    /// <summary>
    ///  Reload the UI 
    /// </summary>
    /// <param name="perfectAmmo"> how much is critical ammo? can be represented different</param>
    /// <param name="actualAmmo"> the full ammo that is loaded (contains the perfectAmmo inside)</param>
    /// <param name="maxClip"> the max clipe, maybe the full clip is 30, but only can load 15 bullets</param>
    public abstract void Reload(int perfectAmmo, int actualAmmo, int maxClip);
    public abstract void Shoot(int wasterAmmo);

    protected abstract void OnStart();
    protected abstract void OnUpdate();

}
