using UnityEngine;

public class BaseCharacter : MonoBehaviour
{

    [HideInInspector]
    public int Space = 10;
    protected Rigidbody2D _rb;
    protected Animator _anim;
    protected SpriteRenderer _sr;


    public float speed = 5;

    [HideInInspector]
    public bool m_FacingRight = true;
    public int attackDamage;
    public int maxHealth;
    public float runSpeed = 5;

    [Space]
    [Header("Jump Raycast-----------------------------------------------------")]
    public Transform feetPos;
    public float jumpCheckRadius;
    public float jumpForce;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    protected void Move(float horizontalInput)
    {
        //We set the velocity based on the input of the player
        //We set the y to rb.velocity.y, because if we set it to 0 our object does not move down with gravity
        _rb.velocity = new Vector2(horizontalInput * speed, _rb.velocity.y);

        //If moving left...
        if (horizontalInput > 0 && !m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (horizontalInput < 0 && m_FacingRight)
        {
            // ... flip the player.
            Flip();
        }

        //We send this information to the animator, which handles the transition between animations
        //We send the Absolute (= Always positive) value of horizontalInput, so even when cherecter moves left, his animation plays
        //        anim.SetFloat("MoveSpeed", Mathf.Abs(horizontalInput * speed));

    }

    //I do not want to use 'flip x' because my attack point will not filp with character
    protected void Flip()
    {
        m_FacingRight = !m_FacingRight;
        //if put camera in children, camera view will change. so i have to let the camera follow the character.
        transform.Rotate(0f, 180f, 0f);
    }


}
