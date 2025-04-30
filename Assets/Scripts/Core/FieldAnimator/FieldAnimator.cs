using System;
using LSCore.DataStructs;
using UnityEditor;
using UnityEngine;


namespace Core
{
    public partial class FieldAnimator : MonoBehaviour
    {
        [Serializable]
        public abstract class Handler
        {
            [NonSerialized] public FieldManager fieldManager;
            
            public abstract void Handle();
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

            foreach (var handler in handlers.Values)
            {
                handler.handler.fieldManager = fieldManager;
            }
        }

        private void OnDestroyBlocks(SpriteRenderer blockPrefab)
        {
            handlers[blockPrefab].handler.Handle();
        }
    }
}