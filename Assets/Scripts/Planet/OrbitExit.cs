using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitExit : MonoBehaviour
{
    GameManager GameManager;
    ShipControl ship;
    Vector2 currentVel;

    private void Awake()
    {
        ship = GameObject.Find("Ship").GetComponent<ShipControl>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ship") && GameManager.checkpointHits == 1)
        {
            GameManager.tutRelease = true;
            GameManager.checkpointHits = 0;
        }
    }

    /*
    IEnumerator ExitOrbit()
    {
        currentVel = ship.rb.velocity;
        ship.rb.velocity = new Vector2(0, ship.rb.velocity.y);
        GameManager.exitOrbit = true;
        ship.isTouching = false;
        ship.transform.up = Vector3.up;
        GameManager.lockedOn = false;
        GameManager.checkpointHits = 0;
        ship.rb.AddForce(new Vector2(0, 2), ForceMode2D.Impulse);
        ship.transform.position = Vector3.Lerp(ship.transform.position, 
            new Vector3(0, ship.transform.position.y), 1f);
        yield return new WaitForSeconds(0.3f);
        GameManager.exitOrbit = false;
    }
    */
}
