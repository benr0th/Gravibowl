using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu]
public class LaunchAbility : Ability
{
    GameManager GameManager;

    public override void Activate()
    {
        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameManager.superLaunchActive = true;
        }

    }

    //IEnumerator LaunchPowerChanger()
    //{
    //    float launchP = hole.GetComponentInChildren<Hole>().launchPower.y;
    //    bool launched = hole.GetComponentInChildren<Hole>().hasLaunched;
    //    launchP = 100;
    //    Debug.Log("launchP=" + launchP);
    //    yield return new WaitUntil(() => launched);
    //    launchP = hole.GetComponentInChildren<Hole>().originalLaunchPower;
    //    launched = false;
    //    Debug.Log("launchP=" + launchP);
    //}
}
