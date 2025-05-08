using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Player;

    [Header("Movimiento Básico")]
    public float moveSpeed = 5f;
    private float lastDirectionX = 1f;

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
    public int maxJumps = 2;
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

    private Rigidbody2D rb;
    private Collider2D coll;
    private Vector2 input;
    private int jumpCount;
    private bool isGrounded;
    private bool isDashing;
    private bool isSliding;
    private float lastDashTime;
    private float lastSlideTime;

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
        if (input.x != 0) lastDirectionX = Mathf.Sign(input.x);

        CheckGrounded();
        Jump();

        SlideInput();

        if (!isGrounded)
        {
            timeSinceLastGrounded += Time.deltaTime;
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

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            bool canJump = timeSinceLastGrounded < coyoteTimeThreshold || isGrounded || jumpCount > 0;

            if (canJump)
            {
                float jumpForce = Mathf.Sqrt(jumpPower * -2 * (Physics2D.gravity.y * rb.gravityScale));
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * speedJumpMultiplier);
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

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
<<<<<<< Updated upstream:Greybox_phase/Assets/Scripts/player/PlayerMovement.cs
=======

>>>>>>> Stashed changes:OrangePhase/Assets/Scripts/player/PlayerMovement.cs
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            m_Animator.SetBool("isJumping", true);

            if (Input.GetButtonUp("Jump"))
            {
                jumpCancelled = true;
            }

            if (jumpTimer > jumpButtonTime)
            {
                isJumping = false;
            }
        }
        else if (isGrounded)
        {
            m_Animator.SetBool("isJumping", false);
            m_Animator.SetBool("isGrounded", true);
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
            jumpCount = maxJumps - 1;
            lastTimeOnGrounded = Time.time;
            coyoteTimeThreshold = 0;
        }
    }

<<<<<<< Updated upstream:Greybox_phase/Assets/Scripts/player/PlayerMovement.cs
    private void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }
=======

>>>>>>> Stashed changes:OrangePhase/Assets/Scripts/player/PlayerMovement.cs

    private void SlideInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && Time.time > lastSlideTime + slideCooldown)
        {
            StartCoroutine(Slide());
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

        isSliding = false;
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
