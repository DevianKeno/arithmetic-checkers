using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Aritche.UI
{
    public class FriendEntryUI : MonoBehaviour
    {
        public string DisplayName
        {
            get
            {
                return displayNameTMP.text;
            }
            set
            {
                displayNameTMP.text = value;
            }
        }
        
        [SerializeField] TextMeshProUGUI displayNameTMP;
    }
}