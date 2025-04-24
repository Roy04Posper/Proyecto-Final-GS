using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
[System.Obsolete]
public class PlayerMovement : MonoBehaviour
{
    public class MovementMotor : MonoBehaviour
    {
        [Header("Movimiento Básico")]
        public float moveSpeed = 5f;
        public float acceleration = 10f;
        public float deceleration = 10f;
        private float lastDirectionX = 1f;

        [Header("Gravedad y Suelo")]
        public float gravityScale = 1f;
        public LayerMask groundLayer;
        public LayerMask SpecialPlatformLayer;
        public Transform groundChecker1;
        public Transform groundChecker2;
        public Transform groundChecker3;
        public float groundCheckRadius = 0.2f;

        [Header("Salto")]
        [SerializeField] private float jumpVelocity = 12f;
        public int maxJumps = 2;
        public float jumpAcceleration = 5f;
        public float jumpDeceleration = 5f;

        [Header("Coyote Time")]
        [SerializeField, Range(0f, 0.3f)] private float coyoteTime = 0.25f;
        private float coyoteCounter;


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
        private bool onSpecialPlatform;


        private bool isJumping;
        private bool isDashing;
        private bool isSliding;
        private float lastDashTime;
        private float lastSlideTime;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            rb.gravityScale = gravityScale;
        }

        private void Update()
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (input.x != 0)
                lastDirectionX = Mathf.Sign(input.x);

            CheckGrounded();
            HandleJumpInput();
            HandleDashInput();
            HandleSlideInput();
        }

        private void FixedUpdate()
        {
            if (!isDashing && !isSliding)
            {
                Move();
            }
        }

        private void Move()
        {
            float targetSpeed = input.x * moveSpeed;
            float speedDifference = targetSpeed - rb.velocity.x;
            float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelRate, 0.9f) * Mathf.Sign(speedDifference);
            rb.AddForce(new Vector2(movement, 0f));
        }

        private void CheckGrounded()
        {

            isGrounded =
                Physics2D.OverlapCircle(groundChecker1.position, groundCheckRadius, groundLayer) ||
                Physics2D.OverlapCircle(groundChecker2.position, groundCheckRadius, groundLayer) ||
                Physics2D.OverlapCircle(groundChecker3.position, groundCheckRadius, groundLayer);

            onSpecialPlatform =
                Physics2D.OverlapCircle(groundChecker1.position, groundCheckRadius, SpecialPlatformLayer) ||
                Physics2D.OverlapCircle(groundChecker2.position, groundCheckRadius, SpecialPlatformLayer) ||
                Physics2D.OverlapCircle(groundChecker3.position, groundCheckRadius, SpecialPlatformLayer);

            if (isGrounded)
            {
                jumpCount = maxJumps - 1;

                coyoteCounter = coyoteTime;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;

            }
        }
        private void HandleJumpInput()
        {
            bool jumpAble = coyoteCounter > 0f;
            bool canAirJump = jumpCount < maxJumps;

            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("boton salto pulsado");

                if (jumpAble)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
                    if (!isGrounded)
                    {
                        jumpCount++; // Contamos este salto como uno usado
                    }

                    // Reinicia el coyote
                    coyoteCounter = 0f;
                }
                // Salto adicional en el aire
                else if (canAirJump)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
                    jumpCount++;
                    Debug.Log("Salto en el aire realizado");
                }
                else
                {
                    Debug.Log("No se puede saltar");
                }
            }
        }

        private void HandleDashInput()
        {
            Debug.Log("dash performed");
            if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashCooldown)
            {
                StartCoroutine(Dash());
            }
        }

        private void HandleSlideInput()
        {
            Debug.Log("slide performed");
            if (Input.GetKeyDown(KeyCode.LeftControl)
                && isGrounded
                && Time.time > lastSlideTime + slideCooldown)
            {
                StartCoroutine(Slide());
            }
        }

        private void PassThroughtPlatformInput()
        {

        }

        private System.Collections.IEnumerator Dash()
        {
            isDashing = true;
            lastDashTime = Time.time;

            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(lastDirectionX * dashSpeed, 0);

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
                rb.velocity = new Vector2(direction * slideSpeed, rb.velocity.y);
                timer += Time.deltaTime;
                yield return null;
            }

            isSliding = false;
        }
    }
}