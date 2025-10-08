using UnityEngine;
using System.Collections;

public class SwordAttack : MonoBehaviour
{
    public int attackDamage = 25;
    public Collider2D swordCollider;

    private void Start()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    public void StartAttack()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
            // safety: auto-disable after a short window in case animation event is missing
            StopAllCoroutines();
            StartCoroutine(AutoEndAttack(0.25f));
        }
    }

    public void EndAttack()
    {
        if (swordCollider != null)
            swordCollider.enabled = false;
        StopAllCoroutines();
    }

    private IEnumerator AutoEndAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (swordCollider == null || !swordCollider.enabled) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
            enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            enemy.GetDamage(attackDamage);
        }
    }
}
