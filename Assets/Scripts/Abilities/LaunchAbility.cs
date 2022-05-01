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

}
