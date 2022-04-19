using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public Transform leftWall;
    public Transform rightWall;
    public Transform bottomWall;
    public Transform bg;
    public Transform effect;

    private void Update()
    {
        if (target != null)
        {
            if (target.position.y > transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
            }

            if (target.position.y > leftWall.position.y)
            {
                leftWall.position = new Vector3(leftWall.position.x, transform.position.y, transform.position.z);
            }

            if (target.position.y > rightWall.position.y)
            {
                rightWall.position = new Vector3(rightWall.position.x, transform.position.y, transform.position.z);
            }

            if (target.position.y > bottomWall.position.y)
            {
                bottomWall.position = new Vector3(bottomWall.position.x, transform.position.y - 5.5f, transform.position.z);
            }

            if (target.position.y > bg.position.y)
            {
                bg.position = new Vector3(bg.position.x, transform.position.y, bg.position.z);
            }

            //if (target.position.y > effect.position.y)
            //{
            //    effect.position = new Vector3(effect.position.x, target.position.y, effect.position.z);
            //}
        }
    }
}
