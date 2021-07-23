using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : BaseCharacter
{
    public float jumpTime;

    //wallSlide state Check 
    [Space]
    [Header("Wall check-----------------------------------------------------")]
    public Vector2 rightOffset;
    public Vector2 leftOffset;
    public float wallCheckRadius;
    public float wallSlideSpeed = 0.5f;

    [Tooltip("Reaction time that allow player jump after leave wall")]
    public float leaveWallJumpTime = 0.2f;


    // attack check
    [Space]
    [Header("Attack check-----------------------------------------------------")]
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public float attackCoolDown;


    //wallSlide state Check 
    [Space]
    [Header("Roll-----------------------------------------------------")]
    public float rollForce;
    public float rollTime;


    //LayerMask
    [Space]
    [Header("Layer Setting")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public LayerMask whatIsEnemy;


    //checks
    //ground check
    private bool isGrounded;
    //attack check
    private bool isAttacking = false;
    //jump check
    private bool isJumping;
    //onWall check
    private bool onWall;
    private bool onLeftWall;
    private bool onRightWall;
    private bool isWallSliding = false;

    //jump 
    private float jumpTimeCounter;
    //wall
    private float wallJumpTime;
    //attack
    private float attackTime;
    //roll
    private float currentRollTime;
    private bool isRolling = false;
    //movement
    private float runInput;






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
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, wallCheckRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, wallCheckRadius);

        //attack check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    protected void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, jumpCheckRadius, whatIsGround);

        if (isGrounded == true && Input.GetButtonDown("Jump") || isWallSliding && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            isWallSliding = false;
            animator.SetBool("IsJumping", true);

            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }

        if (Input.GetButton("Jump") && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

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
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, wallCheckRadius, whatIsWall) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, wallCheckRadius, whatIsWall);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, wallCheckRadius, whatIsWall);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, wallCheckRadius, whatIsWall);

        if (onWall && !isGrounded)
        {
            isWallSliding = true;

            wallJumpTime = Time.time + leaveWallJumpTime;
        }
        else if (wallJumpTime < Time.time)
        {
            isWallSliding = false;
        }

        //for animation

        if (onRightWall && !isGrounded)
        {
            animator.SetBool("IsWallSliding", true);
        }
        else if (onLeftWall && !isGrounded)
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
        if (Input.GetButtonDown("Fire1") && !isAttacking && !isRolling && !isWallSliding)
        {

            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
            }

            isAttacking = true;
            attackTime = attackCoolDown;

            rb.velocity = new Vector2(0, 0);

            animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);

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

        // if (isAttacking)
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0.8f, float.MaxValue));
        // }

    }


    private void Roll()
    {

        if (Input.GetButtonDown("Fire3") && isGrounded && !isAttacking && !isRolling)
        {
            isRolling = true;
            animator.SetBool("IsRolling", true);
            currentRollTime = rollTime;
            rb.velocity = Vector2.zero;
        }

        if (isRolling)
        {
            if (m_FacingRight)
            {
                rb.velocity = Vector2.right * rollForce;
            }
            else
            {
                rb.velocity = Vector2.left * rollForce;
            }

            currentRollTime -= Time.deltaTime;

            if (currentRollTime <= 0)
            {
                isRolling = false;
                animator.SetBool("IsRolling", false);
            }
        }
    }

}

