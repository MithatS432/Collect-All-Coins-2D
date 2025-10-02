using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private int attackDamage = 25;
    private Animator swordAnim;
    private bool canDealDamage = false;

    private void Start()
    {
        swordAnim = GetComponent<Animator>();
    }

    public void DoSwordAttack()
    {
        swordAnim.SetTrigger("AttackSword");
        canDealDamage = true;

        Invoke("EndAttack", 0.3f);
    }

    private void EndAttack()
    {
        canDealDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canDealDamage && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetDamage(attackDamage);
            }
        }
    }
}