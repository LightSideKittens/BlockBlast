using System.Collections.Generic;
using UnityEngine;

namespace StarSavers
{
    public class CombatSystem : MonoBehaviour
    {
        public PlayerCombatStats stats;
        public List<AttackData> attacks;
        private float attackCooldown = 0f;

        void Update()
        {
            attackCooldown -= Time.deltaTime;

            Collider2D[] enemies = stats.GetEnemiesInRange();
            if (enemies.Length > 0 && attackCooldown <= 0f && attacks.Count > 0)
            {
                Attack(enemies[0].transform);
                attackCooldown = 1f / stats.attackRate;
            }
        }

        void Attack(Transform target)
        {
            AttackData attack = attacks[Random.Range(0, attacks.Count)];
            GameObject damageSP = Instantiate(attack.damageSourcePrefab, transform.position, Quaternion.identity);
            damageSP.GetComponent<DamageSource>().Init(target, attack.damage, attack.speed, attack.effectOnHit);

        }
    }
}