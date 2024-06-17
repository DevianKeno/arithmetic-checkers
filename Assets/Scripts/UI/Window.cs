using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Damath.UI
{
    public class Window : MonoBehaviour, IUIElement
    {
        public bool IsVisible { get; set; }

        public Vector3 Position
        {
            get
            {
                return rect.transform.position;
            }
            set
            {
                rect.transform.position = value;
            }
        }

        public event Action OnOpen;
        public event Action OnClose;

        RectTransform rect;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }

        public void Show()
        {
            gameObject.SetActive(true);
            OnShow();
            IsVisible = true;
            OnOpen?.Invoke();
        }

        public void Hide()
        {
            OnHide();
            gameObject.SetActive(false);
            IsVisible = false;
            OnClose?.Invoke();
        }

        public void ToggleVisibility()
        {
            SetVisible(!IsVisible);
        }

        public void SetVisible(bool visible)
        {
            if (visible)
            {
                Hide();
            } else
            {
                Show();
            }
            IsVisible = visible;
        }

        public void SetScale(float multiplier)
        {
            Vector2 dimensions = rect.localScale;
            dimensions.x *= multiplier; 
            dimensions.y *= multiplier; 
            rect.localScale = dimensions;
        }

        public void SetScale(float width, float height)
        {
            rect.localScale = new Vector2(width, height);
        }

        // public void Open()
        // {
        //     IsVisible = true;
        //     this.gameObject.SetActive(true);

        //     if (choices.Count != 0)
        //     {
        //         foreach (var c in choices)
        //         {
        //             c.gameObject.SetActive(true);
        //         }
        //     }
        // }

        // public virtual void Open(Vector3 position)
        // {
        //     IsVisible = true;
        //     rect.position = position;
        //     rect.pivot = new Vector2(0f, 1f);
        //     this.gameObject.SetActive(true);

        //     if (choices.Count != 0)
        //     {
        //         foreach (var c in choices)
        //         {
        //             c.gameObject.SetActive(true);
        //         }
        //     }
        // }

        // public virtual void Close()
        // {
        //     if (choices.Count != 0)
        //     {
        //         foreach (var c in choices)
        //         {
        //             c.gameObject.SetActive(false);
        //         }
        //     }

        //     this.gameObject.SetActive(false);
        //     IsVisible = false;
        // }

        public void Move(Vector3 position)
        {
            rect.position = position;
        }

        public void Delete()
        {
            Destroy(gameObject);
        }
    }
}
