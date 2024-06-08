using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed = 1.7f;
    private float jump = 3.3f;
    private bool isInitialJump = true;

    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // For PC
        // if (Input.GetButton("Jump") && Mathf.Abs(rb.velocity.y) < 0.001f)
        // {
        //     // For jumping and moving the player to the right
        //     rb.AddForce(new Vector2(rb.velocity.x, jump), ForceMode2D.Impulse);
        //     rb.velocity = new Vector2(speed, rb.velocity.y);
        // }
    }

    public void Move()
    {
        rb.AddForce(new Vector2(rb.velocity.x, jump), ForceMode2D.Impulse);
        rb.velocity = new Vector2(speed, rb.velocity.y);

        if (isInitialJump) {
            isInitialJump = false;
            speed = 1.5f;
        }
    }
}
