using System;
using DG.Tweening;
using LSCore;
using LSCore.AnimationsModule;
using UnityEngine;

namespace Core
{
    public class LoseWindow : BaseWindow<LoseWindow>
    {
        [SerializeField] private LSButton watchButton;
        [SerializeReference] private AnimSequencer timerAnim;
        public static Action onReviveClicked;

        private Sequence sequence;
        protected override void OnShowing()
        {
            base.OnShowing();
            // добавить рекламу
            watchButton.Clicked += onReviveClicked;
            watchButton.Clicked += UIViewBoss.GoBack;
            sequence = timerAnim.Animate();
        }

        protected override void OnHiding()
        {
            base.OnHiding();
            watchButton.Clicked -= onReviveClicked;
            watchButton.Clicked -= UIViewBoss.GoBack;

            sequence?.Kill();
        }
    }
}