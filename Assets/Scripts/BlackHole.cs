using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    ShipControl ship;
    public Rigidbody planetRb;

    public float G;

    private void Awake()
    {
        ship = GameObject.Find("Ship").GetComponent<ShipControl>();
    }

    private void FixedUpdate()
    {
        if (ship.notAtStart)
        {
            StartCoroutine(BlackHoleInit());
        }
    }

    IEnumerator BlackHoleInit()
    {
        yield return new WaitForSeconds(1f);
        // Gravity
        Vector2 direction = transform.position - ship.transform.position;
        float distanceSqr = direction.sqrMagnitude;
        float forceMagnitude = G * (planetRb.mass * ship.rb.mass) / distanceSqr;
        Vector2 force = direction.normalized * forceMagnitude;
        ship.rb.AddForce(force);
    }
}
