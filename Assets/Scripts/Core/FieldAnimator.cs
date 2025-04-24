using System;
using LSCore.DataStructs;
using UnityEngine;

namespace Core
{
    public class FieldAnimator : MonoBehaviour
    {
        [Serializable]
        public abstract class Handler
        {
            public abstract void Handle();
        }
        
        [Serializable]
        public class BlockHole : Handler
        {
            public ParticleSystem fx;
            public override void Handle()
            {
                fx.Play();
            }
        }
        
        [Serializable]
        public class Electricity : Handler
        {
            public ParticleSystem fx;
            public override void Handle()
            {
                fx.Play();
            }
        }
        
        [Serializable]
        public class HandlerWrapper
        {
            [SerializeReference] public Handler handler;
        }
        
        public FieldManager fieldManager;
        public UniDict<SpriteRenderer, HandlerWrapper> handlers = new();

        private void Awake()
        {
            fieldManager.BlocksDestroying += OnDestroyBlocks;
        }
        
        public void OnDestroyBlocks(SpriteRenderer blockPrefab)
        {
            handlers[blockPrefab].handler.Handle();
        }
    }
}