using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Damath.UI
{
    public class Tooltip : MonoBehaviour, IUIElement
    {
        public bool IsVisible { get; set; }

        public string Text
        {
            get
            {
                return text.text;
            }
            set
            {
                text.text = value;
            }
        }

        public Color Color
        {
            get
            {
                return image.color;
            }
            set
            {
                image.color = value;
            }
        }

        public float ShowDelay = 1f;
        public float HideDelay = 0f;
        public bool FadeTransition = true;
        public float FadeInDuration = 0.25f;
        public float FadeOutDuration = 0.25f;

        [SerializeField] RectTransform rect;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Image image;

        void Start()
        {
            image.CrossFadeAlpha(0f, 0f, false);
            text.CrossFadeAlpha(0f, 0f, false);
        }

        void Update()
        {
            if (IsVisible)
            {
                rect.anchoredPosition = new(
                    Input.mousePosition.x + (rect.sizeDelta.x * 0.55f),
                    Input.mousePosition.y - (rect.sizeDelta.y * 0.55f)
                );  
            }
        }
        
        public void Show()
        {
            SetVisible(true);
        }
        
        public void Hide()
        {
            SetVisible(false);
        }

        public async void SetVisible(bool value)
        {
            if (value)
            {  
                if (IsVisible) return;
                IsVisible = true;
                
                await Task.Delay((int) ShowDelay * 1000);

                image.CrossFadeAlpha(1f, FadeInDuration, false);
                text.CrossFadeAlpha(1f, FadeInDuration, false);

            } else
            {
                await Task.Delay((int) HideDelay * 1000);

                image.CrossFadeAlpha(0f, FadeInDuration, false);
                text.CrossFadeAlpha(0f, FadeInDuration, false);
                
                await Task.Delay((int) FadeInDuration * 1000);
                IsVisible = false;
                Delete();
            }
        }

        async void Delete()
        {
            gameObject.SetActive(false);
            await Task.Delay((int)FadeOutDuration * 1000);
            Destroy(gameObject);
        }

        public void ToggleVisibility()
        {
            throw new System.NotImplementedException();
        }
    }
}