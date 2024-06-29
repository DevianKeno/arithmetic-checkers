using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Aritche.UI
{
    public class FrameController : MonoBehaviour
    {
        [field: SerializeField] public bool IsTransitioning { get; private set; }
        [field: SerializeField] public Frame CurrentFrame { get; set; }
        public List<Frame> Frames = new();
        public LeanTweenType TweenType = LeanTweenType.easeOutExpo;

        [SerializeField] GameObject inactive;

#region Editor
        [SerializeField] string switchTo;
#endregion

        void Start()
        {
            (inactive.transform as RectTransform).anchoredPosition = new(0, 1080f);
            CurrentFrame ??= GetComponentInChildren<Frame>();

            foreach (Transform c in inactive.transform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(c as RectTransform);
            }
        }

        public void SwitchToFrame(string name)
        {
            Frame frame = GetFrame(name);
            if (frame == null) return;

            if (CurrentFrame != null)
            {
                if (CurrentFrame.Name == name) return; /// Same frame to switch to
                if (IsTransitioning) return;
                IsTransitioning = true;

                if (Game.Main.Settings.EnableAnimations)
                {
                    /// Move old frame out of the way
                    /// The value -1920 moves the frame one screen wide to the left
                    LeanTween.move(CurrentFrame.rect, new Vector2(-1920f, 0f), Game.Main.Settings.AnimationFactor)
                    .setEase(TweenType)
                    .setOnComplete(() =>
                    {
                        CurrentFrame.transform.SetParent(inactive.transform);
                        CurrentFrame.rect.anchoredPosition = Vector2.zero;
                    });

                    /// Move new frame into view
                    frame.rect.anchoredPosition = new(1670f, -1080f);
                    frame.transform.SetParent(transform);
                    frame.gameObject.SetActive(true);
                    LeanTween.move(frame.rect, Vector2.zero, Game.Main.Settings.AnimationFactor)
                    .setEase(TweenType)
                    .setOnComplete(() =>
                    {
                        CurrentFrame = frame;
                        IsTransitioning = false;
                    });
                    LayoutRebuilder.ForceRebuildLayoutImmediate(frame.transform as RectTransform);
                }
                else
                {
                    CurrentFrame.transform.SetParent(inactive.transform);
                    CurrentFrame.rect.anchoredPosition = Vector2.zero;

                    frame.transform.SetParent(transform);
                    frame.gameObject.SetActive(true);
                    frame.rect.anchoredPosition = Vector2.zero;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(frame.transform as RectTransform);
                    
                    CurrentFrame = frame;
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

        public void SwitchFrameEditor(string frameId)
        {
            switchTo = frameId;
        }

        public void SwitchFrameEditor()
        {
            Frame frame = null;
            if (int.TryParse(switchTo, out int index))
            {
                if (index >= 0 && index < Frames.Count)
                {
                    frame = Frames[index];
                }
            } else
            {
                frame = GetFrame(switchTo);
            }
            if (frame == null) return;
            if (CurrentFrame.Name == name) return;

            /// Hide current frame
            CurrentFrame.transform.SetParent(inactive.transform);
            CurrentFrame.rect.anchoredPosition = Vector2.zero;

            /// Show new frame
            frame.gameObject.SetActive(true);
            frame.transform.SetParent(transform);
            frame.rect.anchoredPosition = Vector2.zero;

            CurrentFrame = frame;
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        #endregion
    }
}

