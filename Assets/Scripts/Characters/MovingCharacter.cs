﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class MovingCharacter : MonoBehaviour
{
    public float baseSpeed;
    [HideInInspector]
    public float speed;

    protected Vector2 movement;     // Direction in which the character moves

    [HideInInspector]
    public Vector2 direction;    // Direction in which he is facing
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public CircleCollider2D circleCollider;
    protected SpriteRenderer spriteRenderer;
    protected bool canMove = true;      // Can he move?
    [HideInInspector]
    public bool canAct = true;       // Can he do anything at all?

    protected bool isFalling = false;
    public bool isFlying = false;
    private float spinningSpeed = 10f;
    private float fallingDuration = 3f;
    protected StatusEffectReceiverOld statusEffectReceiver;
    protected BuffsReceiver buffReceiver;
    private static float AnimSpeedToMoveSpeedRatio = 5f;    // The movement speed that corresponds to an animator speed of 1

    // Use this for initialization
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        statusEffectReceiver = GetComponent<StatusEffectReceiverOld>();
        buffReceiver = GetComponent<BuffsReceiver>();
        speed = baseSpeed;
    }

    protected void Start()
    {
        rb.freezeRotation = true;
    }

    protected void updateAnimations()
    {
        if (buffReceiver && buffReceiver.isStunned)
        {
            anim.speed = 0;
            return;
        }

        if (anim)
        {
            if (movement != Vector2.zero)
            {
                anim.SetBool("Moving", true);
            }
            else
                anim.SetBool("Moving", false);

            anim.SetFloat("DirectionX", direction.x);
            anim.SetFloat("DirectionY", direction.y);
            anim.speed = movingSpeed / AnimSpeedToMoveSpeedRatio;
        }
    }

    protected void updateSortingOrder()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
    }

    /// <summary>
    /// Is falling into a hole
    /// </summary>
    public void startFalling(float damageRatio)
    {
        if (isFlying)
            return;
        if (!isFalling)
        {
            StartCoroutine(fallAnimation(spinningSpeed, fallingDuration, damageRatio));
            Buff newBuff = new Buff(BuffType.Stun, fallingDuration);
            newBuff.removable = false;
            newBuff.resistable = false;
            if (buffReceiver)
                buffReceiver.addBuff(newBuff);
        }
        isFalling = true;
    }

    /// <summary>
    /// Can the character move?
    /// </summary>
    /// <param name="value"></param>
    public void enableMovement(bool value)
    {
        canMove = value;
        if (!value)
        {
            rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Can the character do anything at all?
    /// </summary>
    /// <param name="value"></param>
    public void enableAction(bool value)
    {
        canAct = value;
        enableMovement(value);
    }

    private IEnumerator fallAnimation(float spinningSpeed, float duration, float damageRatio = 0f)
    {
        float startingTime = Time.time;
        circleCollider.enabled = false;
        while (Time.time - startingTime < duration)
        {
            transform.Rotate(0, 0, spinningSpeed);
            float scale = Mathf.Lerp(1f, 0f, (Time.time - startingTime)/duration);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForFixedUpdate();
        }
        circleCollider.enabled = true;
        PlayerController pc = GetComponent<PlayerController>();
        Damageable dmg = GetComponent<Damageable>();
        if (pc)
            pc.hasFallen(damageRatio);
        else if (dmg)
            dmg.die();
        else
            Destroy(gameObject);
    }

    public void stopMovementFor(float duration)
    {
        StartCoroutine(stopMovementRoutine(duration));
    }

    private IEnumerator stopMovementRoutine(float duration)
    {
        enableMovement(false);
        yield return new WaitForSeconds(duration);
        enableMovement(true);
    }

    public void killAfterSeconds(float duration)
    {
        StartCoroutine(coroutineKillAfterSeconds(duration));
    }

    private IEnumerator coroutineKillAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        die();
        Destroy(gameObject);
    }

    public float movingSpeed
    {
        get
        {
            if (buffReceiver)
            {
                return speed * buffReceiver.speedMultiplier;
            }
            else
                return speed;
        }
    }

    public abstract void die();

    public abstract void receivesDamage();
}
