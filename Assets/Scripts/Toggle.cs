using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Damath
{
    public class Toggle : MonoBehaviour
    {
        public bool IsVisible { get; set; }   
        public bool Value { get; set; }
        public float SpeedInSeconds = 0.5f;
        public Color BackgroundColorOff = new(0.37f, 0.37f, 0.37f, 1f);
        public Color BackgroundColorOn = new(0.2f, 0.33f, 0.46f, 1f);
        public Color KnobColor = new(1f, 1f, 1f, 1f);
        public bool IsHovered { get; set; }
        [SerializeField] RectTransform knobRect;
        [SerializeField] Image backgroundImage;
        [SerializeField] UnityEngine.UI.Toggle toggle;

        void Start()
        {
            SetValue(true);
            toggle.onValueChanged.AddListener(SetValue);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = true;
        }

        public bool GetValue()
        {
            return Value;
        }

        public void SetValue(bool value)
        {
            Value = value;
            toggle.interactable = false;
            SetInteractableAfter(true, SpeedInSeconds);
            if (Value)
            {
                LeanTween.move(knobRect, new Vector3(35f, 0f, 0f), SpeedInSeconds)
                .setEaseOutExpo();

                LeanTween.value(backgroundImage.gameObject, 0.1f, 1f, SpeedInSeconds)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    backgroundImage.color = Color.Lerp(BackgroundColorOff, BackgroundColorOn, i);
                });
            } else
            {
                LeanTween.move(knobRect, new Vector3(-35f, 0f, 0f), SpeedInSeconds)
                .setEaseOutExpo();

                LeanTween.value(backgroundImage.gameObject, 0.1f, 1f, SpeedInSeconds)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    backgroundImage.color = Color.Lerp(BackgroundColorOn, BackgroundColorOff, i);
                });
            }
        }

        async void SetInteractableAfter(bool value, float delayInSeconds)
        {
            await Task.Delay((int)(delayInSeconds * 1000));
            toggle.interactable = value;
        }
    }
}
