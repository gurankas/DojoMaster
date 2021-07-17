using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : BaseCharacter
{
    float runInput = 0;

    static float health;
    public BoxCollider2D player;

    public LayerMask whatIsGround;
    public bool isGrounded;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    public LayerMask whatIsWall;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int side;

    public float collisionRadius;
    public Vector2 rightOffset;
    public Vector2 leftOffset;

    public float leaveWallJumpTime = 0.2f;
    private float wallJumpTime;
    public float wallSlideSpeed = 0.5f;
    public float wallDistance = 0.5f;
    private bool isWallSliding = false;


    // Start is called before the first frame update
    void Start()
    {
        player = player.GetComponent<BoxCollider2D>();

        Cursor.lockState = CursorLockMode.Locked;
        health = 5;
    }

    private void FixedUpdate()
    {
        //movement
        runInput = Input.GetAxisRaw("Horizontal");

        Move(runInput);

    }

    private void Update()
    {

        Jump();

        //wall check

        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, whatIsWall) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, whatIsWall);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, whatIsWall);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, whatIsWall);
        side = onRightWall ? 1 : -1;


        if (onWall && !isGrounded && runInput != 0)
        {
            isWallSliding = true;
            wallJumpTime = Time.time + leaveWallJumpTime;
        }
        else if (wallJumpTime < Time.time)
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideSpeed, float.MaxValue));
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }

    protected void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        if (isGrounded == true && Input.GetKeyDown(KeyCode.Space) || isWallSliding && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }
}
