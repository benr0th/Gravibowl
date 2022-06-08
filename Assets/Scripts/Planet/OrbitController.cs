using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    ShipControl ship;
    GameManager GameManager;

    private void Awake()
    {
        ship = GameObject.Find("Ship").GetComponent<ShipControl>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ship") && !GameManager.lockedOn)
        {
            GameManager.lockedOn = true;
            //GameManager.isOrbiting = true;
        }
    }
}
