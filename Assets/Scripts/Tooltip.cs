using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Damath
{
    public class Tooltip : MonoBehaviour
    {
        public IHoverable Element;
        public bool IsVisible;
        public float ShowDelay = 1f;
        public float HideDelay = 0f;
        public bool FadeTransition = true;
        public float FadeInDuration = 0.25f;
        public float FadeOutDuration = 0.25f;

        [SerializeField] private RectTransform rect;
        [SerializeField] private TextMeshProUGUI tmpUGUI;
        [SerializeField] private Image image;

        void Start()
        {
            // gameObject.SetActive(false);

            image.color = new (image.color.r, image.color.g, image.color.b, 0f);
            tmpUGUI.color = new (tmpUGUI.color.r, tmpUGUI.color.g, tmpUGUI.color.b, 0f);
        }

        void Update()
        {
            if (IsVisible)
            {
                rect.anchoredPosition = new (Input.mousePosition.x + (rect.sizeDelta.x * 0.55f),
                                             Input.mousePosition.y - (rect.sizeDelta.y * 0.55f));  
            }
        }
        
        public void SetElement(IHoverable obj)
        {
            Element = obj;
        }
        
        public void SetText(string text)
        {            
            tmpUGUI.text = text;
        }

        public void SetColor(Color color)
        {
            image.color = color;
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
            // if (!Enable) return;
            // if (element == null) return;

            if (value)
            {  
                LeanTween.cancel(gameObject);

                if (IsVisible) return;
                IsVisible = true;
                
                await Task.Delay((int)ShowDelay * 1000);
                try{ if (gameObject == null) return; } catch { return; }; // Stupid but fuckkk, it's 5 am ;;
                
                LeanTween.value(gameObject, image.color.a, 1f, FadeInDuration)
                .setOnUpdate( (i) =>
                {
                    image.color = new (image.color.r, image.color.g, image.color.b, i);
                });
                    
                LeanTween.value(gameObject, tmpUGUI.color.a, 1f, FadeInDuration)
                .setOnUpdate( (i) =>
                {
                    tmpUGUI.color = new (tmpUGUI.color.r, tmpUGUI.color.g, tmpUGUI.color.b, i);
                });

            } else
            {
                LeanTween.cancel(gameObject);

                await Task.Delay((int)HideDelay * 1000);

                LeanTween.value(gameObject, image.color.a, 0f, FadeOutDuration)
                .setOnUpdate( (i) =>
                {
                    image.color = new (image.color.r, image.color.g, image.color.b, i);
                });
                    
                LeanTween.value(gameObject, tmpUGUI.color.a, 0f, FadeOutDuration)
                .setOnUpdate( (i) =>
                {
                    tmpUGUI.color = new (tmpUGUI.color.r, tmpUGUI.color.g, tmpUGUI.color.b, i);
                })
                .setOnComplete( () =>
                {
                    IsVisible = false;
                    Delete();
                });
            }
        }

        async void Delete()
        {
            gameObject.SetActive(false);
            await Task.Delay((int)FadeOutDuration * 1000);
            Destroy(gameObject);
        }
    }
}