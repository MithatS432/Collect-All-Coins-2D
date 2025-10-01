using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private int attackDamage = 25;
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetDamage(attackDamage);
            }
        }
    }
    public void SetAttackSwordAnimation()
    {
        anim.SetTrigger("AttackSword");
    }
}
