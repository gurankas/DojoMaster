using UnityEngine;

public class TriggerBossFight : MonoBehaviour
{
    [SerializeField]
    private Boss _bossToEnable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            _bossToEnable.Init();
            Destroy(gameObject);
        }
    }
}
