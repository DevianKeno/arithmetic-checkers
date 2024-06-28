using System;

using UnityEngine;

using Aritche.Systems;
using Aritche.UI;

namespace Aritche
{
    public class TransitionEffect : MonoBehaviour, IUIElement
    {
        public bool IsVisible
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public LeanTweenType Tween;
        public float AnimationSpeed;
        public float Delay = 0.25f;
        public float Multiplier = 1.2f;
        public Vector2 Source;
        public Vector2 Middle;
        public Vector2 Target;

        event Action callbacks;

        [SerializeField] RectTransform color1;
        [SerializeField] RectTransform color2;

        void Start()
        {
            Reset();
        }

        public void SetOptions(TransitionOptions options)
        {
            Tween = options.Tween;
            AnimationSpeed = options.AnimationSpeed;
            Delay = options.Delay;
            Multiplier = options.Multiplier;
        }

        public void PlayToHalf(Action callback)
        {
            callbacks += callback;
            Play(Source, Middle);
        }

        public void PlayToEnd(Action callback)
        {
            callbacks += callback;
            callbacks += () =>
            {
                Destroy(gameObject);
            };
            Play(Middle, Target);
        }

        void Play(Vector2 from, Vector2 to)
        {
            LeanTween.value(color1.gameObject, from, to, AnimationSpeed * Multiplier)
                .setOnUpdate((Vector2 i) =>
                {
                    color1.anchoredPosition = i;
                })
                .setEase(Tween);

            LeanTween.value(color2.gameObject, from, to, AnimationSpeed)
                .setDelay(Delay)
                .setOnUpdate((Vector2 i) =>
                {
                    color2.anchoredPosition = i;
                })
                .setEase(Tween)
                .setOnComplete(() =>
                {
                    callbacks?.Invoke();
                });
        }

        public void Reset()
        {
            color1.anchoredPosition = Source;
            color2.anchoredPosition = Source;
        }

        public void ToggleVisibility()
        {
            throw new System.NotImplementedException();
        }

        public void SetVisible(bool visible)
        {
            throw new System.NotImplementedException();
        }
    }
}