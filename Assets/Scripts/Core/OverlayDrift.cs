using System;
using UnityEngine;

namespace Core
{
    public class OverlayDriftAndFlicker : MonoBehaviour
    {
        public float moveSpeed = 20f;
        private RectTransform rect;
            // public CanvasGroup overlayGroup;
        public float flickerSpeed = 5f;
        public float intensity = 0.1f;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            float x = Mathf.Sin(Time.time * 1.5f) * moveSpeed;
            float y = Mathf.Cos(Time.time * 1.2f) * moveSpeed;
            rect.anchoredPosition = new Vector2(x, y);

           // overlayGroup.alpha = 1f - (Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * intensity);
        }
    }
}