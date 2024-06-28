using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Aritche
{
    public enum TimerFormat {MM_SS, SS}

    public class Timer : MonoBehaviour
    {
        public TimerFormat Format = TimerFormat.MM_SS;
        public float StartTimeInSeconds = 0f;
        public float CurrentTime = 0f;
        public bool IsEnabled = false;
        public bool IsRunning { get; private set; }

        public List<UnityAction> Callbacks = new();
        public TextMeshProUGUI TMPComponent = null;

        void Update()
        {
            if (!IsRunning) return;
            
            CurrentTime -= 1 * Time.deltaTime;

            if (TMPComponent != null)
            {
                TMPComponent.text = Format switch
                {
                    TimerFormat.MM_SS => ToMM_SS(),
                    TimerFormat.SS => ToSS(),
                    _ => ToSS()
                };
            }

            if (CurrentTime < 0)
            {
                Finish();
            }
        }

        public void Init(float value)
        {
            CurrentTime = value;
        }

        public void SetFormat(TimerFormat format)
        {
            Format = format;
        }

        public string ToMM_SS()
        {
            TimeSpan time = TimeSpan.FromSeconds(CurrentTime);

            return time.ToString("mm':'ss");
        }

        public string ToSS()
        {
            TimeSpan time = TimeSpan.FromSeconds(CurrentTime);

            return time.ToString("%s");
        }
        
        public void Begin()
        {
            if (IsRunning) return;
            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
        }

        public void Resume()
        {
            IsRunning = true;
        }

        public void Pause()
        {
            IsRunning = false;
        }

        public void Reset(bool start = false)
        {
            CurrentTime = StartTimeInSeconds;
            IsRunning = start;
        }
        
        
        public void Finish()
        {
            IsRunning = false;

            if (Callbacks.Count == 0) return;

            foreach (var c in Callbacks)
            {
                c();
            }
        }

        public float GetTime()
        {
            return CurrentTime;
        }

        public void SetTime(float valueInSeconds)
        {
            StartTimeInSeconds = valueInSeconds;
            CurrentTime = valueInSeconds;
        }

        public void ChangeTime(float valueInSeconds)
        {
            CurrentTime = valueInSeconds;
        }
        
        public void Invoke()
        {
            foreach (var c in Callbacks)
            {
                c();
            }
        }
        
        public void AddTime(float valueInSeconds)
        {
            CurrentTime += valueInSeconds;
        }
        
        public void RemoveTime(float valueInSeconds)
        {
            CurrentTime -= valueInSeconds;
        }

        public void AddCallback(UnityAction function)
        {
            Callbacks.Add(function);
        }

        public void SetText(TextMeshProUGUI value)
        {
            this.TMPComponent = value;
        }
    }
}