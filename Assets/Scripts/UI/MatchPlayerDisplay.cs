using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Damath.Network;

namespace Damath.UI
{
    public class MatchPlayerDisplay : MonoBehaviour
    {
        [SerializeField] Image avatar;
        [SerializeField] TextMeshProUGUI usernameTMP;
        [SerializeField] LoadingIconAnimation loadingIcon;

        public void SetUserInfoDisplayed(UserInfo info)
        {
            usernameTMP.text = info.username;
            loadingIcon.gameObject.SetActive(false);
        }
        
        public void SetWaiting()
        {
            usernameTMP.text = "Waiting...";
            loadingIcon.gameObject.SetActive(true);
        }
    }
}