using UnityEngine;
using TMPro;
using Aritche.Players;

namespace Aritche.UI
{
    public class MessageBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageTmp;
        [SerializeField] private TMP_InputField inputTmp;

        void Awake()
        {
            inputTmp.onSubmit.AddListener(AddMessage);
            Game.Events.OnPlayerSendMessage += GetMessage;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        public void AddMessage(string message)
        {
            if (inputTmp.text == "") return;
            
            inputTmp.text = "";
            inputTmp.Select();
            messageTmp.text += $"\n{message}";
        }

        public void GetMessage(Player player, string message)
        {
            var str = $"<{player.name}> {message}";
            AddMessage(str);
        }
    }
}