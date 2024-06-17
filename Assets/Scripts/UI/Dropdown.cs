using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public class Dropdown : TMP_Dropdown
    {
        public float heightMin = 0f;
        public float heightMax = 300f;
        public event Action OnClick;
        
        [SerializeField] RectTransform dropdownRect;

        protected override void Awake()
        {
            base.Awake();

            OnClick += Open;
        }

        public override void OnPointerClick(PointerEventData pointerEventData)
        {
            Show();
            OnClick?.Invoke();
        }

        public override void OnCancel(BaseEventData eventData)
        {
            LeanTween.cancel(dropdownRect.gameObject);
            base.OnCancel(eventData);
        }

        protected override GameObject CreateDropdownList(GameObject template)
        {
            GameObject dropdown = base.CreateDropdownList(template);
            dropdownRect = dropdown.GetComponent<RectTransform>();
            return dropdown;
        }

        public void Open()
        {
            LeanTween.value(dropdownRect.gameObject, heightMin, heightMax, Game.Main.Settings.AnimationFactor)
            .setEaseOutExpo()
            .setOnUpdate( (i) =>
            {
                dropdownRect.sizeDelta = new Vector2 (dropdownRect.sizeDelta.x , i);
            });
        }
    }
}