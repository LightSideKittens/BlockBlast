using UnityEngine;

namespace Core
{
    public partial class FieldAnimator
    {
        public class Cookie : Handler
        {
            public ParticleSystem fx;
            public override void Handle()
            {
                foreach (var (index, block) in fieldManager.UniqueSuicidesData)
                {
                    Destroy(block.gameObject);
                }
            }
        }
    }
}