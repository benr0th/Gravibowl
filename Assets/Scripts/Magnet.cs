using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    BallControl ball;
    MagnetGauge magnetGauge;
    public bool isMagnetized;

    private void Awake()
    {
        ball = GameObject.Find("Ball").GetComponent<BallControl>();
        magnetGauge = GameObject.Find("MagnetGauge").GetComponent<MagnetGauge>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<BallControl>(out BallControl ballControl) && ball.isTouching)
        {
            if (!magnetGauge.outOfMagnet)
            {
                isMagnetized = true;
                ballControl.SetTarget(transform.parent.position);
            }       
        }
    }
}
