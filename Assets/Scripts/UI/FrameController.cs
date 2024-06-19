using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Damath.UI
{
    public class FrameController : MonoBehaviour
    {
        [field: SerializeField] public Frame CurrentFrame { get; set; }
        public List<Frame> Frames = new();

        bool _isTransitioning = false;
        [SerializeField] string switchTo;

        [SerializeField] GameObject inactive;

        void Start()
        {
            if (CurrentFrame == null)
            CurrentFrame = GetComponentInChildren<Frame>();

            if (Frames.Count > 0)
            {
                foreach (Frame frame in Frames)
                {
                    if (frame == null) continue;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(frame.transform as RectTransform);
                }
            }
        }

        public void SwitchFrame(string name)
        {
            Frame frame = GetFrame(name);
            if (frame == null) return;
            inactive.SetActive(false);

            if (CurrentFrame != null)
            {
                if (CurrentFrame.Name == name) return;
                if (_isTransitioning) return;
                _isTransitioning = true;

                if (Game.Main.Settings.EnableAnimations)
                {
                    LeanTween.move(CurrentFrame.rect, new Vector3(-1920f, 0f, 0f), Game.Main.Settings.AnimationFactor)
                    .setEaseOutExpo()
                    .setOnComplete( () =>
                    {
                        CurrentFrame.transform.SetParent(inactive.transform);
                        CurrentFrame.rect.anchoredPosition = new Vector3(0f, 0f, 0f);
                        CurrentFrame.gameObject.SetActive(false);
                    });

                    frame.gameObject.SetActive(true);
                    frame.gameObject.transform.SetParent(transform);
                    LeanTween.move(frame.rect, new Vector3(0f, 0f, 0f), Game.Main.Settings.AnimationFactor)
                    .setEaseOutExpo()
                    .setOnComplete( () =>
                    {
                        CurrentFrame = frame;
                        _isTransitioning = false;
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

        #region Editor

        public void SwitchFrameEditor()
        {            
            Frame frame = GetFrame(switchTo);
            if (frame == null) return;
            if (CurrentFrame.Name == name) return;

            inactive.SetActive(false);
            /// Hide current frame
            CurrentFrame.transform.SetParent(inactive.transform);

            /// Show new frame
            frame.gameObject.SetActive(true);
            frame.transform.SetParent(transform);

            var rect = frame.transform as RectTransform;
            rect.anchoredPosition = Vector2.zero;
            CurrentFrame = frame;
        }

        #endregion
    }
}

