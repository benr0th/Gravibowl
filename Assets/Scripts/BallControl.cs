using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    public float power = 10f;
    public float maxDrag = 5f;
    public float moveSpeed = 7f;

    public Rigidbody2D rb;
    public LineRenderer lr;
    public AudioSource audioClip;
    GameManager GameManager;
    //public ParticleSystem hitEffect = null;

    private bool notMoving;
    private bool ready;
    private float stoppedMoving = 0f;
    private bool notMovingUp;

    Vector3 difference = Vector3.zero;
    Vector3 draggingPos;
    Vector3 dragStartPos;
    Touch touch;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();    
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        notMoving = false;
        notMovingUp = false;
        ready = false;
    }

    private void Update()
    {
        if (rb.velocity.magnitude <= 0.25f)
        {
            notMoving = true;
            rb.velocity = Vector2.zero;
        }
        else { notMoving = false; }

        if (rb.velocity.y <= 0.5f)
        {
            notMovingUp = true;
            stoppedMoving += Time.deltaTime;
            rb.velocity = Vector2.zero;
        } else { stoppedMoving = 0f; notMovingUp = false; }

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            // Only move when ball is not moving, uses voodoo magic, do not change or it will break
            if (notMoving)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    ready = true;
                }
            }
            else { ready = false; }

            // Drag and shoot
            if (ready || GameManager.grabbedPowerUp)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    DragStart();
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    Dragging();
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    audioClip.Play();
                    DragRelease();
                    //hitEffect.Play();
                }
            }

            // Move left and right while moving
            if (!notMovingUp && stoppedMoving < 1f && !GameManager.isPaused)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
                {
                    Move();
                }
            }
        }
    }

    private void FixedUpdate()
    {

    }

    private void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(touch.position);
        dragStartPos.z = 0f;
        lr.positionCount = 1;
        lr.SetPosition(0, dragStartPos);
    }

    private void Dragging()
    {
        draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
        draggingPos.z = 0f;
        lr.positionCount = 2;
        lr.SetPosition(1, draggingPos);
    }

    private void DragRelease()
    {
        lr.positionCount = 0;
        
        Vector3 dragReleasePos = Camera.main.ScreenToWorldPoint(touch.position);
        dragReleasePos.z = 0f;

        Vector3 force = (dragStartPos - dragReleasePos);
        Vector3 clampedForce = Vector3.ClampMagnitude(force, maxDrag) * power;

        rb.AddForce(clampedForce, ForceMode2D.Impulse);
    }

    private void Move()
    {
        Vector2 vel = rb.velocity;
        draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
        draggingPos.z = 0f;
        difference = draggingPos - transform.position;
        vel.x = 0f;
        rb.velocity = vel;

        transform.position = Vector3.Lerp(transform.position,
                new Vector3(draggingPos.x, transform.position.y, 0f), 0.3f);

        /* Alternative moving methods
        transform.position = new Vector3(draggingPos.x, transform.position.y, transform.position.z);
        rb.transform.Translate(draggingPos.x * 0.01f, 0f, 0f);
        rb.MovePosition(new Vector2(draggingPos.x, transform.position.y));
        transform.position = Vector3.MoveTowards(transform.position,
                                new Vector3(draggingPos.x, transform.position.y, transform.position.z),
                                1f * Time.deltaTime);

        vel.x = difference.x * moveSpeed;
        rb.velocity = vel;
        */
    }
}
