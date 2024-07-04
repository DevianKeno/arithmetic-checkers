using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Aritche
{
    public enum TimerFormat { MM_SS, SS }
    public enum TimeCheck { Less, Greater }

    public class Timer : MonoBehaviour
    {
        public bool IsEnabled = false;
        [SerializeField] TimerFormat format = TimerFormat.MM_SS;
        public TimerFormat Format
        {
            get { return format; }
            set { format = value; }
        }
        [SerializeField] TimeCheck timeCheck = TimeCheck.Less;
        public TimeCheck TimeCheck
        {
            get { return timeCheck; }
            set { timeCheck = value; }
        }
        public float StartTimeInSeconds = 0f;
        [SerializeField] float _currentTime = 0f;
        public float Time
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                textTMP.text = FormatTime();
            }
        }
        public float CheckTimeOn = 0f;
        bool _isRunning = false;
        public bool IsRunning => _isRunning;
        public Color Color
        {
            get { return textTMP.color; }
            set { textTMP.color = value; }
        }
        Color _originalColor;

        public event Action OnReachZero;

        [SerializeField] TextMeshProUGUI textTMP;
        [SerializeField] Image pauseIcon;

        void Awake()
        {
            if (textTMP == null && TryGetComponent<TextMeshProUGUI>(out var tmp))
            {
                textTMP = tmp;
            }
        }

        void Start()
        {
            _originalColor = Color;
        }

        void Update()
        {
            if (!_isRunning) return;
            
            _currentTime -= 1 * UnityEngine.Time.deltaTime;

            textTMP.text = FormatTime();
            CheckTime();
        }

        void CheckTime()
        {
            if (timeCheck == TimeCheck.Less)
            {
                if (_currentTime < CheckTimeOn)
                {
                    Finish();
                }
            }
            else if (timeCheck == TimeCheck.Greater)
            {
                if (_currentTime > CheckTimeOn)
                {
                    Finish();
                }
            }
        }

        string FormatTime()
        {
            if (format == TimerFormat.MM_SS)
            {
                return ToMM_SS();
            }
            else if (format == TimerFormat.SS)
            {
                return ToSS();
            }
            return ToSS();
        }

        string ToMM_SS()
        {
            TimeSpan time = TimeSpan.FromSeconds(_currentTime);
            return time.ToString("mm':'ss");
        }

        string ToSS()
        {
            TimeSpan time = TimeSpan.FromSeconds(_currentTime);
            return time.ToString("%s");
        }

        public void Begin()
        {
            _isRunning = true;
            if (pauseIcon != null)
            {
                pauseIcon.gameObject.SetActive(false);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Resume()
        {
            _isRunning = true;
            if (pauseIcon != null)
            {
                pauseIcon.gameObject.SetActive(false);
            }
        }

        public void Pause()
        {
            _isRunning = false;
            if (pauseIcon != null)
            {
                pauseIcon.gameObject.SetActive(true);
            }
        }

        public void ResetTimer(bool start = false)
        {
            _currentTime = StartTimeInSeconds;
            _isRunning = start;
        }

        public void Finish()
        {
            _isRunning = false;
            OnReachZero?.Invoke();
        }
        
        public void ResetColor()
        {
            Color = _originalColor;
        }
        
        public void SetStartTime(float valueInSeconds, bool reset = true)
        {
            StartTimeInSeconds = valueInSeconds;
            if (reset)
            {
                _currentTime = valueInSeconds;
            }
        }

        public void AddTime(float valueInSeconds, bool notify = false)
        {
            _currentTime += valueInSeconds;
            if (notify)
            {
                CheckTime();
            }
        }
        
        public void RemoveTime(float valueInSeconds, bool notify = false)
        {
            _currentTime -= valueInSeconds;
            if (notify)
            {
                CheckTime();
            }
        }
    }
}