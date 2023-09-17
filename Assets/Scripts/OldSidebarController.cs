using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OldSidebarController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject SideBar;
    public GameObject TitleScreen;
    public Image SideBarImage;
    [SerializeField] private RectTransform rect;
    public RectTransform TitleScreenRectTransform;

    public Vector2 SideBarSize;
    public Vector2 TitleScreenSize;
    public float ScreenWidth;

    public float ExitedWidth;       // captures the width when the pointer exits
    public float StartingWidth = 250f;     // collapsed sidebar width
    public float FinalWidth = 400f;        // expanded sidebar width

    // two boolean variables to check every update
    // ChangedValue will only be true once IsHovered variable changes,
    // and will immediately be set back to false after the Delay
    public bool IsHovered = false;
    public bool ChangedValue = false;

    // stores the variables needed for the hover animation
    public float Duration = 0.3f;
    public float ElapsedTime = 0;
    private float _percent = 0f;

    // animation Delay when first hovered (pointer exits will not have delays)
    public float WaitingTime = 0;
    public float Delay = 0.1f;

    public Color DefaultColor;
    public Color HoveredColor = new Color(38/255f, 60/255f, 82/255f, 1f);

    void Start()
    {
        // stores the reference to the GameObject components
        SideBarImage = SideBar.GetComponent<Image>();
        rect = SideBar.GetComponent<RectTransform>();
        TitleScreenRectTransform = TitleScreen.GetComponent<RectTransform>();     

        SideBarSize = rect.sizeDelta;
        TitleScreenSize = TitleScreenRectTransform.sizeDelta;
        ScreenWidth = rect.sizeDelta.x + TitleScreenRectTransform.sizeDelta.x;

        // the final width should be twice as wide as the initial width
        StartingWidth = SideBarSize.x;
        FinalWidth = StartingWidth * 2f;     

        DefaultColor = SideBarImage.color;
       
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // Toggle();
        // LeanTween.size(rect, new Vector2(600f, rect.sizeDelta.y), 0.5f).setEaseOutExpo();
    }
    
    
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        LeanTween.size(rect, new Vector2(360f, rect.sizeDelta.y), 0.5f).setEaseOutExpo();
    }

    void Update()
    {
      

        if (IsHovered)
        {

            SideBarImage.color = Color.Lerp(DefaultColor, HoveredColor, Mathf.SmoothStep(0, 1, (WaitingTime / Delay)));

            if (ChangedValue)
            {
                if (WaitingTime < Delay)
                {
                    WaitingTime += Time.deltaTime;
                    return;
                }
                ChangedValue = false;

            }
            if (ElapsedTime < Duration)
            {
                ElapsedTime += Time.deltaTime;
                _percent = ElapsedTime / Duration;
                rect.sizeDelta = new Vector2(Mathf.Lerp(StartingWidth, FinalWidth, Mathf.SmoothStep(0, 1, _percent)), SideBarSize.y);
                TitleScreenRectTransform.sizeDelta = new Vector2(ScreenWidth - rect.sizeDelta.x, TitleScreenSize.y);
            }
        }
        else
        {
            SideBarImage.color = Color.Lerp(HoveredColor, DefaultColor, Mathf.SmoothStep(0, 1, Math.Abs(1 - _percent)));

            if (ChangedValue)
            {
                // captures the current width of the sidebar as soon as the pointer exits
                // this will serve as the starting width for the collapse animation
                ExitedWidth = rect.sizeDelta.x;
                ChangedValue = false;

            }
            if (ElapsedTime > 0)
            {
                ElapsedTime -= Time.deltaTime;
                _percent = ElapsedTime / Duration;
                rect.sizeDelta = new Vector2(Mathf.Lerp(ExitedWidth, StartingWidth, Mathf.SmoothStep(0, 1, Math.Abs(1 - _percent))), SideBarSize.y);
                TitleScreenRectTransform.sizeDelta = new Vector2(ScreenWidth - rect.sizeDelta.x, TitleScreenSize.y);
            }
        }

    }

    void Toggle()
    {
        ChangedValue = true;
        if (IsHovered)
        {
            WaitingTime = 0;
            _percent = 0f;
            IsHovered = false;
        }
        else
        {
            IsHovered = true;
        }
    }
}
