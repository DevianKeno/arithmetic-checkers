using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

namespace Damath
{
    public class SidebarController : MonoBehaviour
    {

        public Image SideBarImage;
        public RectTransform SidebarRectTransform;
        public RectTransform TitleScreenRectTransform;
        public float Duration = 0.1f;

        // stores the variables needed for the animation
        private Boolean _isClicked;
        private float _initialWidth = 250f;
        private float _extendedWidth = 450f;
        [SerializeField]
        private float _elapsedTime = 0f;
        private float _percent = 0f;
        private float _screenWidth;

        public void ExtendSidebar()
        {            
            _isClicked = true;

        }

        public void CollapseSidebar()
        {
            _isClicked = false;
        }

        private void Start()
        {
            _screenWidth = SidebarRectTransform.sizeDelta.x + TitleScreenRectTransform.sizeDelta.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isClicked)
            {
                if (SidebarRectTransform.sizeDelta.x < _extendedWidth)
                {   
                    _elapsedTime += Time.deltaTime;
                    _percent = _elapsedTime / Duration;
                    SidebarRectTransform.sizeDelta = new Vector2(Mathf.Lerp(_initialWidth, _extendedWidth, Mathf.SmoothStep(0, 1, _percent)), SidebarRectTransform.sizeDelta.y);
                }
            } 
            else
            {
                if (SidebarRectTransform.sizeDelta.x > _initialWidth)
                {
                    _elapsedTime -= Time.deltaTime;
                    _percent = _elapsedTime / Duration;
                    SidebarRectTransform.sizeDelta = new Vector2(Mathf.Lerp(_extendedWidth, _initialWidth, Mathf.SmoothStep(0, 1, Math.Abs(1 - _percent))), SidebarRectTransform.sizeDelta.y);
                }
            }

            TitleScreenRectTransform.sizeDelta = new Vector2(_screenWidth - SidebarRectTransform.sizeDelta.x, TitleScreenRectTransform.sizeDelta.y);
            TitleScreenRectTransform.anchoredPosition = new Vector2(SidebarRectTransform.sizeDelta.x, TitleScreenRectTransform.anchoredPosition.y);
        }
            
    }

}

