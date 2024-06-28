using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Aritche.UI
{
    public class ConsoleWindow : Window
    {
        public string Command
        {
            get
            {
                return inputField.text;
            }
            set
            {
                inputField.text = value;
            }
        }
        string buffer;
        int currentIndex = 0;
        List<string> _previousInputs = new();
        
        [SerializeField] TextMeshProUGUI messages;
        [SerializeField] TMP_InputField inputField;

        InputActionMap actionMap;
        InputAction toggle;
        InputAction navUp;
        InputAction navDown;
        InputAction submit;

        internal void Initialize()
        {
            InitializeInputs();
        }

        void Start()
        {
            Hide();
        }

        void InitializeInputs()
        {            
            actionMap = Game.Main.GetActionMap("Console Window");
            toggle = actionMap.FindAction("Show/Hide");
            navUp = actionMap.FindAction("Navigate Entry Up");
            navDown = actionMap.FindAction("Navigate Entry Down");
            submit = actionMap.FindAction("Navigate Entry Down");

            toggle.performed += ToggleVisible;
            navUp.performed += NavigatePreviousEntry;
            navDown.performed += NavigateNextEntry;
            // submit.performed += SubmitInput;

            toggle.Enable();
            navUp.Enable();
            navDown.Enable();
            submit.Enable();

            inputField.onEndEdit.AddListener(SubmitInput);
        }       
        
        void ToggleVisible(InputAction.CallbackContext context)
        {
            ToggleVisibility();
        }

        void NavigatePreviousEntry(InputAction.CallbackContext context)
        {
            if (currentIndex == 0)
            {
                currentIndex = _previousInputs.Count;

                if (inputField.text != "")
                {
                    buffer = inputField.text;
                }                
            }

            if (currentIndex - 1 >= 0)
            {
                currentIndex--;
                inputField.text = _previousInputs[currentIndex];
            }
        }

        void NavigateNextEntry(InputAction.CallbackContext context)
        {
            if (currentIndex + 1 < _previousInputs.Count)
            {
                currentIndex++;
                inputField.text = _previousInputs[currentIndex];
            } else
            {
                currentIndex = 0;
                inputField.text = buffer;
            }            
        }

        void UpdateMessages(string message)
        {
            messages.text += message;
        }

        public void Refresh()
        {
            inputField.text = "";
            inputField.Select();
        }

        public void Write(string value)
        {
            messages.text += $"{value}";
        }

        public void WriteLine(string value)
        {
            messages.text += $"\n{value}";
        }

        public void SubmitInput(string input)
        {
            if (input == "") return;
            
            _previousInputs.Add(inputField.text);
            buffer = "";
            currentIndex = 0;
            inputField.text = "";
            inputField.Select();
            Game.Console.ExecuteCommand(input);
        }

        // public void ToggleWindow(bool isVisible)
        // {
        //     _isVisible = isVisible;
        //     OnToggle?.Invoke(isVisible);
        //     Game.UI.ToggleCursor(isVisible);
        //     gameObject.SetActive(isVisible);
        // }

        // public void ToggleWindow()
        // {
        //     ToggleWindow(!_isVisible);
        // }
    }
}
