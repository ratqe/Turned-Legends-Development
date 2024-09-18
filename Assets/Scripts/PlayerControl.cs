using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float moveSpeed;
    float speedX, speedY;
    Rigidbody2D rb;
    private Animator anim;

    //get reference for animator from object
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        speedX = horizontalInput * moveSpeed;
        speedY = verticalInput * moveSpeed;
        rb.velocity = new Vector2(speedX, speedY);

        // Flips player when moving left or right
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(3,3,3);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-3, 3, 3);


        // Checks if the player is moving either horizontally or vertically
        bool run = (horizontalInput != 0 || verticalInput != 0);
        // If run is true set the Player1Run animation boolean
        anim.SetBool("Player1Run", run);
    }
}
