using System;
using DG.Tweening;
using LSCore.Extensions;
using LSCore.Extensions.Unity;
using UnityEngine;

namespace Core
{
    public partial class FieldAnimator
    {
        [Serializable]
        public class Electricity : Handler
        {
            public ParticleSystem fx;

            public override void Handle()
            {
                Sequence seq = DOTween.Sequence();
                foreach (var (index, block) in fieldManager.suicidesIndexes)
                {
                    var tr = block.transform;
                    seq.Insert((index.x + index.y) / 2f * 0.05f, tr.DOScale(0, 0.5f).OnComplete(() =>
                    {
                        Destroy(block.gameObject);
                    }));
                }
                
                fx.Play();
            }
        }
    }
}