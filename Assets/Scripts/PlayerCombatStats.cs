using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRadius = 2f;
    public float attackDamage = 10f;
    public float attackRate = 1f; // выстрелов в секунду
    public float projectileSpeed = 10f;
    public LayerMask enemyLayer;

    // Можно использовать в других скриптах
    public Collider2D[] GetEnemiesInRange()
    {
        return Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}