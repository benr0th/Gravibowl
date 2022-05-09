using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    BallControl ball;
    public Rigidbody planetRb;

    public float G;

    private void Awake()
    {
        ball = GameObject.Find("Ball").GetComponent<BallControl>();
    }
    private void FixedUpdate()
    {
        // Gravity
        //Vector2 direction = transform.position - ball.transform.position;
        //float distanceSqr = direction.sqrMagnitude;
        //float forceMagnitude = G * (planetRb.mass * ball.rb.mass) / distanceSqr;
        //Vector2 force = direction.normalized * forceMagnitude;
        //ball.rb.AddForce(force);
        //ball.rb.AddForce(new Vector2(0, -1));
    }
}
