using LSCore;
using UnityEngine;

namespace StarSavers
{
    public class DamageSource : MonoBehaviour
    {
        public float speed;
        public float damage;
        public GameObject effectOnHit;

        private Transform target;

        public void Init(Transform target, float damage, float speed, GameObject effect = null)
        {
            this.target = target;
            this.damage = damage;
            this.speed = speed;
            this.effectOnHit = effect;
        }

        void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
                HitTarget();
        }

        void HitTarget()
        {
            if (effectOnHit != null)
                Instantiate(effectOnHit, transform.position, Quaternion.identity);

            EnemyStats enemy = target.GetComponent<EnemyStats>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}