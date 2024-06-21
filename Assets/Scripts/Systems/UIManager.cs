using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Damath.UI;

namespace Damath.Systems
{
    public class UIManager : MonoBehaviour
    {
        [Serializable]
        public struct PrefabId
        {
            public string Id;
            public GameObject Prefab;
        }

        [Serializable]
        public struct IconId
        {
            public string Id;
            public Sprite Sprite;
        }
        
        public Canvas Canvas { get; private set; }
        Dictionary<string, Sprite> _iconsDict = new();

        public Tooltip Tooltip;

        bool IsDimmed;

        Dictionary<string, IUIElement> windows = new();
        
        [Header("UI Prefabs")]
        public List<PrefabId> Prefabs;

        [SerializeField] Image dim;


        PlayerInput mainInput;

        void Awake()
        {
            mainInput = Game.Main.MainInput;
            Canvas = GetComponentInChildren<Canvas>();
        }

        internal void Initialize()
        {
            InitializeWindows();
            InitializeInputs();

            var icons = Resources.LoadAll<Sprite>("UI/Icons");
            foreach (var icon in icons)
            {
                _iconsDict[icon.name] = icon;
            }
        }

        void InitializeWindows()
        {
            IUIElement window;
            /// Console
            window = Create<ConsoleWindow>("console_window");
            window.SetVisible(false);
            windows["console_window"] = window;
        }

        void InitializeInputs()
        {
            
        }

        public void PlayTransition()
        {
            //
        }

        public void AddIcon(string id, Sprite sprite)
        {
            _iconsDict[id] = sprite;
        }

        public Sprite GetIcon(string iconId)
        {
            if (_iconsDict.ContainsKey(iconId))
            {
                return _iconsDict[iconId];
            }
            else
            {
                return null;
            }
        }
        
        public T Create<T>(string id, bool inSafeArea = true) where T : IUIElement
        {
            Transform parent = Canvas.transform;
            if (!inSafeArea) parent = Canvas.transform;
            foreach (PrefabId p in Prefabs)
            {
                if (p.Id == id)
                {
                    var go = Instantiate(p.Prefab, parent);
                    go.name = p.Prefab.name;

                    if (go.TryGetComponent(out T element))
                    {
                        return element;
                    }
                }
            }
            Game.Console.Log("Unable to create UI element, it does not exist");
            Debug.Log("Unable to create UI element, it does not exist");
            return default;
        }

        public void Dim(float time)
        {
            if (IsDimmed)
            {
                LeanTween.value(dim.gameObject, dim.color.a, 0f, time)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    dim.color = new (dim.color.r, dim.color.g, dim.color.b, i);
                });
            } else
            {
                LeanTween.value(dim.gameObject, 0f, 0.25f, time)
                .setEaseOutExpo()
                .setOnUpdate( (i) =>
                {
                    dim.color = new (dim.color.r, dim.color.g, dim.color.b, i);
                });
            }
            IsDimmed = !IsDimmed;
        }

        internal T GetWindowFromId<T>(string v) where T : class
        {
            if (windows.TryGetValue(v, out var window))
            {
                if (window is T typedWindow)
                {
                    return typedWindow;
                }
            }
            return null;
        }
    }
}
