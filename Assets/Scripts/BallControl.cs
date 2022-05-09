using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BallControl : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] LineRenderer lr;
    [SerializeField] SkinManager skinManager;
    [SerializeField] MagnetGauge magnetGauge;
    [SerializeField] float power, maxDrag, moveSpeed;
    AudioSource hitAudio;
    GameManager GameManager;
    Hole hole;
    //public ParticleSystem hitEffect = null;

    bool notMoving, notMovingUp, launchButtonPressed;
    public float stoppedMoving, magnetSpeed, rotateSpeed;
    public bool isTouching, notAtStart, ready, hasTarget, inputEnabled = true;

    Vector3 difference = Vector3.zero;
    Vector3 draggingPos, dragStartPos;
    public Vector3 targetPos;

    Touch touch;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); 
        hitAudio = GetComponent<AudioSource>();
        GetComponent<SpriteRenderer>().sprite = skinManager.GetSelectedSkin().sprite;
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Start()
    {
        transform.position = new Vector3(0.07f, -0.57f).normalized * 0.5f;
        //transform.position = new Vector3(0.15f, -1.12f).normalized * 1.13f;
    }
    /*
     vel=(12.76, 2.22)
    velMag=12.95249
    distance=0.6406599
    */
    private void FixedUpdate()
    {

        // Magnet ability
        if (hasTarget && Vector3.Distance(targetPos, transform.position) <= 3)
        {
            //Vector2 targetDirection = (targetPos - transform.position).normalized;
            //rb.velocity += targetDirection * magnetSpeed * Time.fixedDeltaTime;
            //transform.DOMove(targetPos, magnetSpeed * Time.fixedDeltaTime);
            //transform.position = Vector3.MoveTowards(transform.position, targetPos,
            //    magnetSpeed * Time.fixedDeltaTime);

            //rotateSpeed += Time.fixedDeltaTime * 5;
            //transform.RotateAround(targetPos, Vector3.back, rotateSpeed);
            //rb.AddForce(Vector2.up * Time.deltaTime, ForceMode2D.Force);
        }
        if (isTouching)
        {
            // Force of object
            //rb.AddRelativeForce(new Vector2(0, 4));
            GetComponent<ConstantForce2D>().relativeForce = new Vector2(0, 5);
            /*vel=(12.65, 4.45)
            velMag=13.40864
            PosVector=(0.07, -0.57, 0.00)
            PosFloat=0.5714426
            normalized=(0.12, -0.99, 0.00)
            */
            rb.velocity = transform.up.normalized * 13.0058f;

        }
        else
            GetComponent<ConstantForce2D>().relativeForce = Vector2.zero;


    }

    private void Update()
    {
        notAtStart = transform.position.y != GameManager.ballStartPos;
        #region legacy logic
        // Logic for moving ball when not moving
        /*
        if (rb.velocity.magnitude <= 0.25f && !GameManager.inHole)
        {
            notMoving = true;
            rb.velocity = Vector2.zero;
            if (notAtStart && !GameManager.canHitAgain)
                stoppedMoving += Time.deltaTime;
        }
        else { notMoving = false; stoppedMoving = 0; }
        */

            // Prevents moving ball left and right infinitely
            /*
            if (rb.velocity.y <= 0.5f)
            {
                notMovingUp = true;
                stoppedMoving += Time.deltaTime;
                //rb.velocity = Vector2.zero;
            } else { stoppedMoving = 0f; notMovingUp = false; }
            */
#endregion
            if (Input.touchCount > 0 && !GameManager.isPaused 
            && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            touch = Input.GetTouch(0);
            #region drag&shoot mechanism (legacy)
            // Only move when ball is not moving, uses voodoo magic, do not touch or it will break
            /*
            if (notMoving)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    ready = true;
                }
            }
            else { ready = false; }
            */
            // Code for power up logic - GameManager.grabbedPowerUp && inputEnabled == true

            // Drag and shoot
            /*
            if (ready && !notAtStart || ready && GameManager.canHitAgain)
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
            */

            // Move left and right while moving
            /*
            if (!notMovingUp && stoppedMoving < 1f && touch.position.y < Screen.height / 1.2f)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Move();
                }
            }
            */
            #endregion
            // When touching
            if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId) 
                /*&& notAtStart */&& !GameManager.gameOver && !GameManager.canHitAgain)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    magnetGauge.UseMagnet(100f);
                    isTouching = true; 
                }
            }

            // Prevents bug when holding screen while grabbing power up - possible to make neater
            if (touch.phase == TouchPhase.Ended)
            {
                isTouching = false;
                inputEnabled = true;
            }
        }

    }

    void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(touch.position);
        dragStartPos.z = 0f;
        lr.positionCount = 1;
        lr.SetPosition(0, dragStartPos);
    }

    void Dragging()
    {
        draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
        draggingPos.z = 0f;
        lr.positionCount = 2;
        lr.SetPosition(1, draggingPos);
    }

    void DragRelease()
    {
        // Only after hitting ball
        if (lr.positionCount == 2)
        {
            hitAudio.Play();
            //hitEffect.Play();
            if (GameManager.hasRespawned)
            {
                //Physics.IgnoreLayerCollision(10, 3, false);
                GameManager.canHitAgain = false;
            }
        }

        lr.positionCount = 0;
        Vector3 dragReleasePos = Camera.main.ScreenToWorldPoint(touch.position);
        dragReleasePos.z = 0f;

        Vector3 force = (dragStartPos - dragReleasePos);
        Vector3 clampedForce = Vector3.ClampMagnitude(force, maxDrag) * power;
        
        rb.AddForce(clampedForce, ForceMode2D.Impulse);
    }

    void Move()
    {
        Vector2 vel = rb.velocity;
        draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
        draggingPos.z = 0f;
        difference = draggingPos - transform.position;
        vel.x = 0f;
        rb.velocity = vel;

        transform.position = Vector3.Lerp(transform.position,
                new Vector3(draggingPos.x, transform.position.y, 0f), 0.5f);

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

    public void SetTarget(Vector3 position)
    {
        targetPos = position;
        hasTarget = true;
    }

    public void LaunchButton()
    {
        GameManager.checkpointHits = 0;
        SceneManager.LoadSceneAsync("Game");
    }
}
