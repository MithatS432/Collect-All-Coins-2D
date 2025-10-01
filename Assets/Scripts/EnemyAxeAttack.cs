using UnityEngine;

public class EnemyAxeAttack : MonoBehaviour
{
    private int damage = 5;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Soldier>().GetDamage(damage);
        }
    }
}
