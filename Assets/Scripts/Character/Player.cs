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

    public static bool isInputEnabled = true;

    public static bool isInBossFight;


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

    [SerializeField]
    private HealthBarScript _bossHealthBar;
    [SerializeField]
    private HealthBarScript _playerHealthBar;


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

    private Material matWhite;
    private List<Material> matDefault = new List<Material>();

    private int _currentHealth;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    private void Start()
    {
        isInputEnabled = true;
        _currentHealth = maxHealth;

        _playerHealthBar.SetMaxHealth(maxHealth);

        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;

        wallJumpCoolDownCounter = wallJumpCoolDown;

        for (int i = 0; i < _sr.Length; i++)
        {
            matDefault.Add(_sr[i].material);
        }
    }

    private void Update()
    {

        _playerHealthBar.ToggleHealthBarVisibility(true);

        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }

        //animation
        _anim.SetFloat("Speed", Mathf.Abs(runInput));
        _anim.SetBool("IsGrounded", isGrounded);

        if (isInputEnabled == true)
        {
            //movement
            runInput = Input.GetAxisRaw("Horizontal");

            Attack();

            Dash();

            WallCheck();

            Jump();

            if (!isAttacking && !isRolling)
            {
                Move(runInput);
            }

            //adding restart if needed during demonstration
            if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Submit"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // //jumpAttackSwitch
            // if (Input.GetKeyDown(KeyCode.Q))
            // {
            //     if (jumpAttackSwitch)
            //     {
            //         jumpAttackSwitch = false;
            //     }
            //     else
            //     {
            //         jumpAttackSwitch = true;
            //     }
            // }
        }
        else
        {
            runInput = 0;
            _rb.velocity = new Vector2(0, _rb.velocity.y);

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

            if (isGrounded)
            {
                _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -0.7f, 0.7f), _rb.velocity.y);
            }

            if (_attackPS != null)
            {
                _attackPS.gameObject.SetActive(true);
            }

            _anim.SetTrigger("Attack");
            _anim.SetBool("IsJumping", false);

            Invoke("HitDetection", 0.15f);
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

    private void HitDetection()
    {

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We Hit " + enemy.name);
            if (enemy.GetComponent<Boss>())
            {
                enemy.GetComponent<Boss>().TakeDamage(attackDamage);
                SoundManagerScript.PlaySound("Hit");
            }
        }
    }

    private void Dash()
    {
        //Ignore enemy collision when rolling
        Physics2D.IgnoreLayerCollision(10, 11, isRolling);

        if (Input.GetButtonDown("Fire3") && !isAttacking && !isRolling)
        {
            isRolling = true;
            _anim.SetBool("IsRolling", true);
            SoundManagerScript.PlaySound("DashSound");
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

    public void SetInputMode(bool enabled)
    {
        _anim.SetBool("IsRolling", false);
        _anim.SetTrigger("Idle");
        isInputEnabled = enabled;
        isInBossFight = true;

        _bossHealthBar.ToggleHealthBarVisibility(enabled);

    }

    public void StopAttackPS()
    {
        if (_attackPS != null)
        {
            _attackPS.gameObject.SetActive(false);
        }
    }

    public void KnockBack(float knockBackDuration, float knockBackPower, Vector2 newdirection)
    {
        float timer = 0;

        if (!isAttacking)
        {
            while (knockBackDuration > timer)
            {
                timer += Time.deltaTime;
                _rb.AddForce(newdirection * knockBackPower);
            }

        }

    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _playerHealthBar.SetHealth(_currentHealth);
        //camera shake effect
        CameraShake.instance.StartShake(.2f, .1f);
        _anim.SetTrigger("KnockBack");

        StartCoroutine("GetInvincible");

        //hit feedback
        for (int i = 0; i < _sr.Length; i++)
        {
            _sr[i].material = matWhite;
        }

        //player health check
        if (_currentHealth <= 0)
        {
            Die();
            Invoke("ResetMaterial", 0.15f);
        }
        else
        {
            Invoke("ResetMaterial", 0.15f);
        }


    }

    private void ResetMaterial()
    {
        for (int i = 0; i < _sr.Length; i++)
        {
            _sr[i].material = matDefault[i];
        }
    }

    private void Die()
    {
        _anim.SetTrigger("Die");
        SetInputMode(false);
        _bc.enabled = false;
        _rb.gravityScale = 0;
        Invoke("Lose", 2);
    }

    private void Lose()
    {
        SceneManager.LoadScene(4);
    }

    private IEnumerator GetInvincible()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);

        yield return new WaitForSeconds(1f);

        Physics2D.IgnoreLayerCollision(10, 11, false);
    }

}
