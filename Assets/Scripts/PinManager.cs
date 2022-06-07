using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinManager : MonoBehaviour
{
    ShipControl ship;
    CPUPlayer cpu;
    public bool pinFallen, pinDetected;

    private void Awake()
    {
        ship = GameObject.Find("Ship").GetComponentInParent<ShipControl>();
        cpu = ship.GetComponentInParent<CPUPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CPU"))
        {
            pinDetected = true;
            ship.isTouching = false;
            ship.stoppedTouching = true;
            cpu.hasLetGo = true;
        }
    }

}
