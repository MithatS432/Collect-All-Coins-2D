using UnityEngine;

public class EnemyAxeAttack : MonoBehaviour
{
    private int damage = 5;
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Soldier>().GetDamage(damage);
        }
    }
    public void AxeAnimation()
    {
        anim.SetTrigger("AxeAttack");
    }
}
