using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score = 100;
        public Text scoreText;
         
        public void Awake()
        {
            CalculateScore();
        }
        public void CalculateScore()
        {
            scoreText.text = $"{_score}";
        }
        
    }
}