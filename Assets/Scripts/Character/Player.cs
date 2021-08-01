using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : BaseCharacter
{
    //making player a singleton
    public static Player instance;

    public float jumpTime;

    //wallSlide state Check 
    [Space]
    [Header("Wall check-----------------------------------------------------")]
    public Vector2 rightOffset;
    public Vector2 leftOffset;

    public float wallCheckRadius;
    public float wallSlideSpeed;
    public float wallJumpCoolDown;

    [Tooltip("Reaction time that allow player jump after leave wall")]
    public float wallJumpReactionTime;


    // attack check
    [Space]
    [Header("Attack check-----------------------------------------------------")]
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public float attackCoolDown;


    //roll state Check 
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
    private bool isInWallJumpCoolDown;
    //roll check
    private bool isRolling = false;

    private bool jumpAttackSwitch = false;

    //jump 
    private float jumpTimeCounter;
    //wall
    private float wallJumpTime;
    private float wallJumpCoolDownCounter;
    private int previousWall;
    //attack
    private float attackTime;
    //roll
    private float currentRollTime;
    //movement
    private float runInput;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    private void Start()
    {
        wallJumpCoolDownCounter = wallJumpCoolDown;
    }

    private void Update()
    {

        //animation
        _anim.SetFloat("Speed", Mathf.Abs(runInput));
        _anim.SetBool("IsGrounded", isGrounded);

        //movement
        runInput = Input.GetAxisRaw("Horizontal");

        Attack();

        Roll();

        WallCheck();

        Jump();



        if (!isAttacking && !isRolling)
        {
            Move(runInput);
        }

        //adding restart if needed during demonstration
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //jumpAttackSwitch
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (jumpAttackSwitch)
            {
                jumpAttackSwitch = false;
            }
            else
            {
                jumpAttackSwitch = true;
            }
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

        //wallJump cooldown
        if (!isInWallJumpCoolDown)
        {

            //store current wall side
            if (onLeftWall)
            {
                previousWall = -1;
            }
            else if (onRightWall)
            {
                previousWall = 1;
            }

            if (isWallSliding && Input.GetButtonDown("Jump"))
            {

                //jump
                isJumping = true;
                // isWallSliding = false;
                _anim.SetBool("IsJumping", true);

                jumpTimeCounter = jumpTime;

                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);

                isInWallJumpCoolDown = true;
            }

        }
        else
        {
            //check if new wall side same with previous wall side
            if (onLeftWall && previousWall == 1)
            {
                wallJumpCoolDownCounter = wallJumpCoolDown;
                isInWallJumpCoolDown = false;
            }
            else if (onRightWall && previousWall == -1)
            {
                wallJumpCoolDownCounter = wallJumpCoolDown;
                isInWallJumpCoolDown = false;
            }
            else if (isGrounded)
            {
                wallJumpCoolDownCounter = wallJumpCoolDown;
                isInWallJumpCoolDown = false;
            }
            //wall jump cool down on same wall side
            else if (wallJumpCoolDownCounter > 0)
            {
                wallJumpCoolDownCounter -= Time.deltaTime;
            }
            else if (wallJumpCoolDownCounter <= 0)
            {
                wallJumpCoolDownCounter = wallJumpCoolDown;
                isInWallJumpCoolDown = false;
            }
        }

        //groundedjump
        if (isGrounded == true && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            // isWallSliding = false;
            _anim.SetBool("IsJumping", true);

            jumpTimeCounter = jumpTime;
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            // rb.velocity = Vector2.up * jumpForce;
        }

        //hold 'space bar' jump higher
        if (Input.GetButton("Jump") && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                // rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
                _anim.SetBool("IsJumping", false);
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            _anim.SetBool("IsJumping", false);
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

            wallJumpTime = Time.time + wallJumpReactionTime;
        }
        else if (wallJumpTime < Time.time)
        {
            isWallSliding = false;
        }

        //for animation

        if (onRightWall && !isGrounded)
        {
            _anim.SetBool("IsWallSliding", true);
        }
        else if (onLeftWall && !isGrounded)
        {
            _anim.SetBool("IsWallSliding", true);
        }
        else
        {
            _anim.SetBool("IsWallSliding", false);
        }

        if (isWallSliding)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, wallSlideSpeed, float.MaxValue));
        }
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking && !isRolling && !isWallSliding)
        {

            // if (isJumping)
            // {
            //     isJumping = false;
            //     animator.SetBool("IsJumping", false);
            // }

            isAttacking = true;
            attackTime = attackCoolDown;


            if (isGrounded || jumpAttackSwitch)
            {
                _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -0.7f, 0.7f), _rb.velocity.y);
            }

            _anim.SetTrigger("Attack");
            _anim.SetBool("IsJumping", false);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);


            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("We Hit " + enemy.name);
                enemy.GetComponent<Boss>().TakeDamage(attackDamage);

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
        //Ignore enemy collision when rolling
        Physics2D.IgnoreLayerCollision(10, 11, isRolling);


        if (Input.GetButtonDown("Fire3") && !isAttacking && !isRolling)
        {
            isRolling = true;
            _anim.SetBool("IsRolling", true);
            currentRollTime = rollTime;
            _rb.velocity = Vector2.zero;
        }

        if (isRolling)
        {

            if (m_FacingRight)
            {
                _rb.velocity = Vector2.right * rollForce;
            }
            else
            {
                _rb.velocity = Vector2.left * rollForce;
            }

            currentRollTime -= Time.deltaTime;

            if (currentRollTime <= 0)
            {
                isRolling = false;
                _anim.SetBool("IsRolling", false);
            }
        }
    }

}
