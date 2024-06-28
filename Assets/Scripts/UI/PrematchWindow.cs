using System;
using UnityEngine;
using TMPro;

namespace Aritche.UI
{
    public class PrematchWindow : Window
    {
        public event Action OnStart;
        
        [Header("UI Elements")]
        public TMP_InputField blueNameField;
        public TMP_InputField orangeNameField;
        [SerializeField] Button startBtn;

        void Awake()
        {
            startBtn.onClick.AddListener(StartMatch);
        }

        void Start()
        {
            
        }

        void StartMatch()
        {
            OnStart?.Invoke();
            Game.Main.StartMatch();
            Destroy();
        }
    }

}