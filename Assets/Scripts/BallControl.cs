using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallControl : MonoBehaviour
{
    public float power;
    public float maxDrag;
    public float moveSpeed;

    public Rigidbody2D rb;
    public LineRenderer lr;
    public AudioSource hitAudio;
    GameManager GameManager;
    //public ParticleSystem hitEffect = null;

    private bool notMoving;
    private bool ready;
    private float stoppedMoving;
    private bool notMovingUp;
    public bool inputEnabled = true;

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
    }

    private void Update()
    {
        // Logic for moving ball when not moving
        if (rb.velocity.magnitude <= 0.25f)
        {
            notMoving = true;
            rb.velocity = Vector2.zero;
        }
        else { notMoving = false; }

        // Prevents moving ball left and right infinitely
        if (rb.velocity.y <= 0.5f)
        {
            notMovingUp = true;
            stoppedMoving += Time.deltaTime;
            //rb.velocity = Vector2.zero;
        } else { stoppedMoving = 0f; notMovingUp = false; }

        if (GameManager.isPaused)
        {
            if (rb.velocity.y <= 0.5f)
            {
                rb.velocity = Vector2.zero;
            }
        }

        if (Input.touchCount > 0 && !GameManager.isPaused)
        {
            touch = Input.GetTouch(0);

            // Only move when ball is not moving, uses voodoo magic, do not touch or it will break
            if (notMoving)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    ready = true;
                }
            }
            else { ready = false; }

            // Drag and shoot
            if (ready || GameManager.grabbedPowerUp && inputEnabled == true
                && touch.position.y < Screen.height / 1.1f)
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
                    DragRelease();
                }
            }

            // Move left and right while moving
            if (!notMovingUp && stoppedMoving < 1f && touch.position.y < Screen.height / 1.2f)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Move();
                }
            }
        }

        // Prevents bug when holding screen while grabbing power up - possible to make neater
        if (touch.phase == TouchPhase.Ended)
        {
            inputEnabled = true;
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
        // Only play sound effect after hitting ball
        if (lr.positionCount == 2)
        {
            hitAudio.Play();
            //hitEffect.Play();
        }

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
