using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Damath
{
    public class Window : MonoBehaviour, IToggleable
    {
        public bool IsVisible { get; set; }   
        public bool IsChoiceable = false;
        public List<Choice> choices;
        public bool IsMenu = false;
        public RectTransform rect;
        public bool IsHovered { get; set; }

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            IsHovered = true;
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            IsHovered = false;
        }

        public void Toggle()
        {
            if (IsVisible)
            {
                IsVisible = false;
                gameObject.SetActive(false);
            } else
            {
                IsVisible = true;
                gameObject.SetActive(true);
            }
        }

        public void AddChoice(UnityAction function, string text, Sprite icon=null)
        {
            var newChoice = Instantiate(Game.UI.choicePrefab);
            newChoice.transform.SetParent(transform);
            newChoice.transform.localScale = new Vector3(1f, 1f, 1f);

            Choice c_choice = newChoice.GetComponent<Choice>();
            c_choice.Init(function, text, icon);    
            c_choice.AddListener(Close);    
            choices.Add(c_choice); 
        }

        public void AddChoice(Choice choice)
        {

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

        public void ClearChoices()
        {
            if (choices.Count != 0)
            {
                foreach (Choice c in choices)
                {
                    choices.Remove(c);
                    Destroy(c.gameObject);
                }
            }
        }

        public void Open()
        {
            IsVisible = true;
            this.gameObject.SetActive(true);

            if (choices.Count != 0)
            {
                foreach (var c in choices)
                {
                    c.gameObject.SetActive(true);
                }
            }
        }

        public void Open(Vector3 position)
        {
            IsVisible = true;
            rect.position = position;
            rect.pivot = new Vector2(0f, 1f);
            this.gameObject.SetActive(true);

            if (choices.Count != 0)
            {
                foreach (var c in choices)
                {
                    c.gameObject.SetActive(true);
                }
            }
        }

        public void Close()
        {
            if (choices.Count != 0)
            {
                foreach (var c in choices)
                {
                    c.gameObject.SetActive(false);
                }
            }

            this.gameObject.SetActive(false);
            IsVisible = false;
        }

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
