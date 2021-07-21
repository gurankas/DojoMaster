using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : BaseCharacter
{
    float runInput;

    public Animator animator;

    //LayerMask
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;

    public bool isGrounded;
    public bool onWall;
    public bool onLeftWall;
    public bool onRightWall;
    public bool isAttacking = false;

    //Jump state check
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    //wallSlide state Check
    public float collisionRadius;
    public Vector2 rightOffset;
    public Vector2 leftOffset;

    //onWall check
    public float leaveWallJumpTime = 0.2f;
    private float wallJumpTime;
    public float wallSlideSpeed = 0.5f;
    private bool isWallSliding = false;

    // attack check
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask whatIsEnemy;

    public float attackCoolDown;
    private float attackTime;

    //roll

    public float rollForce;
    public float startRollTime;
    private float currentRollTime;
    private float rollDirection;
    private bool isRolling = false;


    private void Update()
    {
        //animation
        animator.SetFloat("Speed", Mathf.Abs(runInput));
        animator.SetBool("IsGrounded", isGrounded);

        //movement
        runInput = Input.GetAxisRaw("Horizontal");

        Attack();

        Roll();
        if (!isAttacking && !isRolling)
        {
            Move(runInput);
            Jump();
            WallCheck();
        }


        //adding restart if needed during demonstration
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    private void OnDrawGizmos()
    {
        //ground check 
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

        //attack check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    protected void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        if (isGrounded == true && Input.GetButtonDown("Jump") || isWallSliding && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            isWallSliding = false;
            animator.SetBool("IsJumping", true);

            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            // rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetButton("Jump") && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                // rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }
    }

    private void WallCheck()
    {
        //wall check
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, whatIsWall) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, whatIsWall);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, whatIsWall);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, whatIsWall);

        if (onWall && !isGrounded && runInput != 0)
        {
            isWallSliding = true;

            wallJumpTime = Time.time + leaveWallJumpTime;
        }
        else if (wallJumpTime < Time.time)
        {
            isWallSliding = false;
        }

        //for animation

        if (onRightWall && !isGrounded && runInput > 0)
        {
            animator.SetBool("IsWallSliding", true);
        }
        else if (onLeftWall && !isGrounded && runInput < 0)
        {
            animator.SetBool("IsWallSliding", true);
        }
        else
        {
            animator.SetBool("IsWallSliding", false);
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideSpeed, float.MaxValue));
        }
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1") && isGrounded && !isAttacking && !isRolling)
        {
            isAttacking = true;
            attackTime = attackCoolDown;

            rb.velocity = new Vector2(0, 0);

            animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, whatIsEnemy);

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("We Hit" + enemy.name);
            }


        }

        //attack cool down
        if (attackTime <= 0f)
        {
            isAttacking = false;
        }
        else
        {
            attackTime -= Time.deltaTime;
        }
    }


    private void Roll()
    {

        if (Input.GetButtonDown("Fire2") && isGrounded && runInput != 0 && !isAttacking)
        {
            isRolling = true;
            animator.SetBool("IsRolling", true);
            currentRollTime = startRollTime;
            rb.velocity = Vector2.zero;
            rollDirection = runInput;
        }

        if (isRolling)
        {
            rb.velocity = Vector2.right * rollDirection * rollForce;

            currentRollTime -= Time.deltaTime;

            if (currentRollTime <= 0)
            {
                isRolling = false;
                animator.SetBool("IsRolling", false);
            }
        }
    }

}

