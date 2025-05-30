﻿using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Player;

    [Header("Movimiento Básico")]
    public float moveSpeed = 5f;
    private float lastDirectionX = 1f;

    [Header("Animaciones Controlador")]
    [SerializeField] private  Animator m_Animator;

    [Header("Gravedad y Suelo")]
    public float gravityScale = 1f;
    public LayerMask groundLayer;
    public Transform groundChecker1;
    public Transform groundChecker2;
    public Transform groundChecker3;
    private float lastTimeOnGrounded;
    public float groundCheckRadius = 0.2f;

    [Header("Salto")]
    public float jumpPower = 5f;
    public float cancelRate = 100f;
    public float jumpButtonTime = 0.25f;
    public float coyoteTime;
    public float coyoteTimeThreshold = 0.2f;
    public int maxJumps;
    public float fallMultiplier = 2.5f;
    public float speedJumpMultiplier;

    private bool isJumping = false;
    private bool jumpCancelled = false;
    private float jumpTimer = 0f;
    private float timeSinceLastGrounded = Mathf.Infinity;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Slide")]
    public float slideSpeed = 8f;
    public float slideDuration = 0.5f;
    public float slideCooldown = 1.5f;
    public Collider2D collSlide;

    private Rigidbody2D rb;
    public Collider2D coll;
    private Vector2 input;
    private int jumpCount;
    private bool isGrounded;
    private bool isDashing;
    private bool isSliding;
    private float lastDashTime;
    private float lastSlideTime;
    private float xAxis;


    private void Awake()
    {
        Player = this;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.x != 0)
        {
            lastDirectionX = Mathf.Sign(input.x);
            m_Animator.SetBool("isRunning", true);

        }
        else
        {
            m_Animator.SetBool("isRunning", false);
        }

        CheckGrounded();
        InputsHandler();
        FlipDirection();
        Jump();
        SlideInput();

        if (!isGrounded)
        {
            timeSinceLastGrounded += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashCooldown)
            {
                StartCoroutine(Dash());
                Debug.Log("Pulsado boton de dash");
            }

        }
        else
        {

            timeSinceLastGrounded = 0f;
        }   
    }

    private void FixedUpdate()
    {
        if (!isDashing && !isSliding)
        {
            rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);
        }

        if (jumpCancelled && isJumping && rb.linearVelocity.y > 0)
        {
            rb.AddForce(Vector2.down * cancelRate);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }
    private void InputsHandler () 
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }
    private void FlipDirection() 
    {

        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (xAxis > 0 )
        { 
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }
    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            m_Animator.SetBool("isJumping", true);
            Debug.Log("Pulsado boton salto");
            bool canJump = timeSinceLastGrounded < coyoteTimeThreshold || isGrounded || jumpCount > 0;
            if (canJump)
            {
                float jumpForce = Mathf.Sqrt(jumpPower * -2 * (Physics2D.gravity.y * rb.gravityScale));
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * speedJumpMultiplier);
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                Debug.Log("Salto realizado");


                isJumping = true;
                jumpCancelled = false;
                jumpTimer = 0f;

                if (!isGrounded)
                {
                    if (timeSinceLastGrounded < coyoteTimeThreshold)
                    {
                        timeSinceLastGrounded = Mathf.Infinity;
                        coyoteTime = 1f;
                    }
                    else
                    {
                        jumpCount--;
                    }
                }


            }

        }
        else
        {
            m_Animator.SetBool("isJumping", false);
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;

            if (Input.GetButtonUp("Jump"))
            {
                jumpCancelled = true;
            }

            if (jumpTimer > jumpButtonTime)
            {
                isJumping = false;
            }
        }
    }

    private void CheckGrounded()
    {
        isGrounded =
            Physics2D.OverlapCircle(groundChecker1.position, groundCheckRadius, groundLayer) ||
            Physics2D.OverlapCircle(groundChecker2.position, groundCheckRadius, groundLayer) ||
            Physics2D.OverlapCircle(groundChecker3.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {

            jumpCount = maxJumps;
            lastTimeOnGrounded = Time.time;
            coyoteTimeThreshold = 0;
        }
    }

    private void SlideInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && Time.time > lastSlideTime + slideCooldown)
        {
            StartCoroutine(Slide());
            Debug.Log("Pulsado boton de slide");
            if (input.x != 0)
            {
                m_Animator.SetBool("isSliding", true);
                coll.enabled = false;
                collSlide.enabled = true;
                Debug.Log("Collider normal deactivado y slide collider activado");
            }

        }
        else
        {
            m_Animator.SetBool("isSliding", false);

        }
    }
    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(lastDirectionX * dashSpeed * moveSpeed, 0);

        yield return new WaitForSeconds(dashDuration);
        Debug.Log("dash hecho");
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private System.Collections.IEnumerator Slide()
    {
        isSliding = true;
        lastSlideTime = Time.time;

        float direction = input.x != 0 ? Mathf.Sign(input.x) : lastDirectionX;

        float timer = 0f;
        while (timer < slideDuration)
        {

            rb.linearVelocity = new Vector2(direction * slideSpeed, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null;
        }


        yield return new WaitForSeconds(slideDuration);
        coll.enabled = true;
        collSlide.enabled = false;
        Debug.Log("Collider normal activado y slide collider desactivado");

        isSliding = false;
        Debug.Log("slide hecho");
    }
    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;
    }
    public void Load(PlayerSaveData data)
    {
        transform.position = data.Position;
    }
}
[System.Serializable]
public struct PlayerSaveData
{
    public Vector2 Position;
}
