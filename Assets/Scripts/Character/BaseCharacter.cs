using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    public Rigidbody2D rb;
    SpriteRenderer sr;

    public float speed = 5;
    // bool value about flip character
    private bool m_FacingRight = true;

    public float jumpForce;
    public Transform feetPos;
    public float checkRadius;



    public void OnEnable()
    {

        rb = GetComponent<Rigidbody2D>();

        sr = GetComponent<SpriteRenderer>();

        //anim = GetComponent<Animator>();

    }

    protected void Move(float horizontalInput)
    {
        //We set the velocity based on the input of the player
        //We set the y to rb.velocity.y, because if we set it to 0 our object does not move down with gravity
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

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
    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        //if put camera in children, camera view will change. so i have to let the camera follow the character.
        transform.Rotate(0f, 180f, 0f);
    }

    
}
