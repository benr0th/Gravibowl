using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target, bg1, bg2;
    public Transform leftWall;
    public Transform rightWall;
    public Transform bottomWall;
    public Transform blackHole;

    float size;

    private void Start()
    {
        size = bg1.GetComponent<BoxCollider2D>().size.y;
    }

    private void Update()
    {
        if (target != null)
        {
            // Camera follows ship
            if (target.position.y > transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
            }

            //if (target.position.y > transform.position.y)
            //{
            //    transform.position = Vector3.Lerp(transform.position,
            //        new Vector3(transform.position.x, target.position.y, transform.position.z), 1f);
            //}

            // Scrolling background
            if (transform.position.y >= bg2.position.y)
            {
                bg1.position = new Vector3(bg1.position.x, bg2.position.y + size, bg1.position.z);
                SwitchBg();
            }

            // Bounds follow camera, causes glitch - need to change
            if (target.position.y > leftWall.position.y)
                leftWall.position = new Vector3(leftWall.position.x, transform.position.y, transform.position.z);

            if (target.position.y > rightWall.position.y)
                rightWall.position = new Vector3(rightWall.position.x, transform.position.y, transform.position.z);

            if (target.position.y > bottomWall.position.y)
                bottomWall.position = new Vector3(bottomWall.position.x, transform.position.y - 5.5f, transform.position.z);

            // Black hole follows camera
            if (transform.position.y - 5.31f > blackHole.position.y)
                blackHole.position = new Vector3(blackHole.position.x, transform.position.y - 5.31f, transform.position.z);
        }
    }

    void SwitchBg() => (bg2, bg1) = (bg1, bg2);

}
