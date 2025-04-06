using UnityEngine;

namespace StarSavers
{ 
    [System.Serializable]
    public class AttackData : MonoBehaviour
    {
        public GameObject damageSourcePrefab;
        public float damage;
        public float speed;
        public GameObject effectOnHit;
    }
}