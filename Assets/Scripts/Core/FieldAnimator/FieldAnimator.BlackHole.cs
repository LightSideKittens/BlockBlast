using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

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
                fx.Play();

                var i = 0;
                
                foreach (var (index, block) in fieldManager.UniqueSuicidesData)
                {
                    var duration = Random.Range(0.5f,0.8f);
                    var rotation = Random.Range(270,359);
                    
                    var tr = block.transform;
                    var empty = new GameObject("Empty").transform;
                    empty.position = fx.transform.position;
                    tr.SetParent(empty, true);
                    seq.Insert(i * 0.02f, empty.DORotate(new Vector3(0, 0, rotation), duration).SetEase(Ease.InExpo));   
                    seq.Insert(i * 0.02f,tr.DOScale(0, duration).SetEase(Ease.InExpo));
                    seq.Insert(i * 0.02f,tr.DOLocalMove(Vector3.zero, duration).SetEase(Ease.InExpo));
                    seq.Insert(i * 0.02f,tr.DORotate(new Vector3(0, 0, rotation), duration).SetEase(Ease.InExpo).OnComplete(() =>
                    {
                        Destroy(block.gameObject);
                    }));
                    i++;
                }
            }
        }
    }
}