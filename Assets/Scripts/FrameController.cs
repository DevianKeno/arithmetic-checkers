using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Damath
{
    public class FrameController : MonoBehaviour
    {
        [field: SerializeField] public Frame CurrentFrame { get; set; }
        public List<Frame> Frames = new();
        private bool IsTransitioning = false;

        [SerializeField] GameObject inactive;

        void Start()
        {
            if (CurrentFrame == null)
            CurrentFrame = GetComponentInChildren<Frame>();
        }

        public void SwitchFrame(string name)
        {
            Frame frame = GetFrame(name);
            if (frame == null) return;

            if (CurrentFrame != null)
            {
                if (CurrentFrame.Name == name) return;
                if (IsTransitioning) return;
                IsTransitioning = true;

                if (Game.Settings.EnableAnimations)
                {
                    LeanTween.move(CurrentFrame.rect, new Vector3(-1920f, 0f, 0f), Game.Settings.AnimationFactor)
                    .setEaseOutExpo()
                    .setOnComplete( () =>
                    {
                        CurrentFrame.transform.SetParent(inactive.transform);
                        CurrentFrame.rect.anchoredPosition = new Vector3(0f, 0f, 0f);
                        CurrentFrame.gameObject.SetActive(false);
                    });

                    frame.gameObject.SetActive(true);
                    frame.gameObject.transform.SetParent(transform);
                    LeanTween.move(frame.rect, new Vector3(0f, 0f, 0f), Game.Settings.AnimationFactor)
                    .setEaseOutExpo()
                    .setOnComplete( () =>
                    {
                        CurrentFrame = frame;
                        IsTransitioning = false;
                    });
                }
            }
        }

        public Frame GetFrame(string name)
        {
            foreach (Frame f in Frames)
            {
                if (f.Name != name) continue;
                return f;
            }
            return null;
        }
    }
}

