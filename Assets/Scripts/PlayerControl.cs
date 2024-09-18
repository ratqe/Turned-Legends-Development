using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 1f;       // Slower overall movement
    public float dashSpeed = 8f;      // Dash speed, set to a reasonable high value
    public float dashDuration = 2f;  // Dash lasts for half a second
    public float smoothing = 0.1f;     // For smoothing movement direction
    private float speedX, speedY;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 originalScale;
    private bool isDashing = false;
    private float dashTime;            // To track how long the dash lasts
    private Vector2 dashDirection;     // The direction the player will dash

    // Reference for animator from object
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Freeze the rotation on Z axis so that player doesn't spin
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        originalScale = transform.localScale; // Store the original scale of the player
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Update normal movement
        if (!isDashing)
        {
            // Normal movement is slow
            speedX = horizontalInput * moveSpeed;
            speedY = verticalInput * moveSpeed;

            // Smooth out the movement
            rb.velocity = Vector2.SmoothDamp(rb.velocity, new Vector2(speedX, speedY), ref currentVelocity, smoothing);

            // Smoothly flip player when moving left or right using Lerp
            if (horizontalInput > 0.01f)
            {
                Vector3 targetScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10);
            }
            else if (horizontalInput < -0.01f)
            {
                Vector3 targetScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10);
            }

            // Save the current direction for dashing (facing left or right)
            dashDirection = new Vector2(transform.localScale.x, 0).normalized;
        }

        // Check for dash input (Spacebar)
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            StartDash();
        }

        // Handle dashing
        if (isDashing)
        {
            rb.velocity = dashDirection * dashSpeed; // Dash in the direction player is facing
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                EndDash();
            }

            Debug.Log("Dashing! Speed: " + dashSpeed); // Debug message to verify dash
        }

        // Checks if the player is moving either horizontally or vertically
        bool run = (horizontalInput != 0 || verticalInput != 0);
        // If run is true set the Player1Run animation boolean
        anim.SetBool("Player1Run", run);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration; // Set the dash time to the dash duration
        Debug.Log("Dash started");
    }

    private void EndDash()
    {
        isDashing = false;
        Debug.Log("Dash ended");
    }
}
