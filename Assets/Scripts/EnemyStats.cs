using UnityEngine;

namespace StarSavers
{
    public class EnemyStats : MonoBehaviour
    {
        public float maxHealth; 
        private float currentHealth;

        void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            Debug.Log($"{gameObject.name} получил {amount} урона. Осталось: {currentHealth}");

            if (currentHealth <= 0)
                Die();
        }

        void Die()
        {
            Debug.Log($"{gameObject.name} погиб!");
            Destroy(gameObject);
        }
    }
}