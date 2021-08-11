using UnityEngine;

public class Sword : MonoBehaviour
{
    private PolygonCollider2D _swordCollider;
    private int _attackSpecificDamage = 0;
    private float knockBackDuration;
    private float knockBackPower;
    private bool m_FacingRight;

    private void Start()
    {
        _swordCollider = GetComponent<PolygonCollider2D>();
    }

    public void ToggleCollider(bool enable, int attackSpecificDamage, float duration, float power, bool facingRight)
    {
        if (_swordCollider != null)
        {
            _swordCollider.enabled = enable;
            _attackSpecificDamage = attackSpecificDamage;
            knockBackDuration = duration;
            knockBackPower = power;
            m_FacingRight = facingRight;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {

            Player.instance.TakeDamage(_attackSpecificDamage);
            other.gameObject.GetComponent<Player>().KnockBack(knockBackDuration, knockBackPower * 1.5f, m_FacingRight ? Vector2.right : Vector2.left);

        }
    }
}
