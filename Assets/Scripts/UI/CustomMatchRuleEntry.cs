using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Aritche.UI
{
    public class CustomMatchRuleEntry : MonoBehaviour
    {
        [SerializeField] int _selected;
        public int Selected => _selected;
        public bool NotifyOnTrue = true;
        public bool NotifyOnFalse = false;
        public event Action<Selectable, int> OnInteractElement;

        List<Selectable> buttonGroup = new();

        [SerializeField] Transform selectables;

        void Start()
        {
            _selected = -1;
            Initialize();
        }

        // void OnValidate()
        // {
        //     if (Application.isPlaying) return;
            
        //     selectables = transform.Find("Button Group");
        //     if (selectables != null)
        //     {
        //         Initialize();
        //         return;
        //     }

        //     selectables = transform.GetChild(transform.childCount - 1);
        //     if (selectables != null)
        //     {
        //         return;
        //     }
        // }

        void Initialize()
        {
            buttonGroup = new();

            int i = 0;
            foreach (Transform child in selectables)
            {
                if (child.TryGetComponent<UnityEngine.UI.Toggle>(out var btn))
                {
                    int btnIndex = i;
                    if (btn.isOn) _selected = btnIndex;

                    btn.onValueChanged.AddListener((value) =>
                    {
                        if (value && NotifyOnTrue || !value && NotifyOnFalse)
                        {
                            _selected = btnIndex;
                            OnInteractElement?.Invoke(btn, btnIndex);
                            Debug.Log($"Set rule entry \"{name}\" selected index to {btnIndex}");
                        }
                    });
                    buttonGroup.Add(btn);
                    i++;
                }
            }
        }

        public void SelectButton(int index)
        {
            if (index < 0 || index > buttonGroup.Count) return;
            
            try
            {
                (buttonGroup[index] as ToggleButton).isOn = true;
            }
            catch (Exception)
            {
                Debug.LogError($"Tried selecting button from rule entry \"{name}\" but failed to do so.");
            }
        }
    }
}