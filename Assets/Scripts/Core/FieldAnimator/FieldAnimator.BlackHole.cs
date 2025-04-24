using System;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public partial class FieldAnimator
    {
        [Serializable]
        public class BlockHole : Handler
        {
            public ParticleSystem fx;
            
            public override void Handle()
            {
                Sequence seq = DOTween.Sequence();
                int i = 0;
                foreach (var (index, block) in fieldManager.suicidesIndexes)
                {
                    seq.Insert(i * 0.05f, block.DOFade(0, 0.5f)).OnComplete(() =>
                    {
                        Destroy(block.gameObject);
                    });
                    i++;
                }
                
                fx.Play();
            }
        }
    }
}