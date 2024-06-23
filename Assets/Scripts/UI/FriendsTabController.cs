using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath.UI
{
    public class FriendsTabController : MonoBehaviour
    {
        [SerializeField] RectTransform sidebar;
        
        void Start()
        {
            CollapseTab();
        }

        public void ExpandTab()
        {
            sidebar.anchoredPosition = new(-400, 0);
        }

        public void CollapseTab()
        {
            sidebar.anchoredPosition = new(0, 0);
        }
    }
}