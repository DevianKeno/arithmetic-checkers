using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Aritche.Rules;

namespace Aritche
{
    public class TimerManager : MonoBehaviour
    {
        public Ruleset Ruleset { get; private set; }
        public float FadeDuration = 0.25f;
        public bool IgnoreTimeScale = false;

        [SerializeField] Timer globalTimer;
        public Timer GlobalTimer => globalTimer;
        [SerializeField] Timer blueTimer;
        public Timer BlueTimer => blueTimer;
        [SerializeField] Timer orangeTimer;
        public Timer OrangeTimer => orangeTimer;

        [SerializeField] Image blueChipImage;
        TextMeshProUGUI blueChipText;
        [SerializeField] Image orangeChipImage;
        TextMeshProUGUI orangeChipText;
        [SerializeField] GameObject timerPrefab;

        void Awake()
        {
            blueChipText = blueChipImage.GetComponentInChildren<TextMeshProUGUI>();
            orangeChipText = orangeChipImage.GetComponentInChildren<TextMeshProUGUI>();
        }

        internal void SetRuleset(Ruleset ruleset)
        {
            Ruleset = ruleset;
        }

        internal void Initialize()
        {

        }

        internal void OnBoardTurnChangedCallback(object sender, Side e)
        {
            SwapTurnTimer(e);
        }
        
        public void BeginMatch()
        {
            if (!Ruleset.GetValue<bool>("EnableTimer"))
            {
                return;
            }

            if (Ruleset.GetValue<bool>("EnableGlobalTimer"))
            {
                globalTimer.SetStartTime(Ruleset.GetValue<float>("GlobalTimeSeconds"), reset: true);
                globalTimer.Format = TimerFormat.MM_SS;
                globalTimer.Begin();
            }
            
            if (Ruleset.GetValue<bool>("EnableTurnTimer"))
            {
                var turnTime = Ruleset.GetValue<float>("TurnTimeSeconds");
                
                blueTimer.Format = TimerFormat.SS;
                blueTimer.SetStartTime(turnTime, reset: true);
                
                orangeTimer.Format = TimerFormat.SS;
                orangeTimer.SetStartTime(turnTime, reset: true);

                var first = Ruleset.GetValue<Side>("FirstTurn");
                SwapTurnTimer(first);
            }
        }

        public void SwapTurnTimer(Side turnOf)
        {
            if (turnOf == Side.Blue)
            {
                orangeTimer.Stop();
                orangeChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                orangeChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                orangeChipText.text = "";

                blueTimer.ResetTimer(true);
                blueChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
                blueChipText.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
            }
            else
            {
                blueTimer.Stop();
                blueChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                blueChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                blueChipText.text = "";

                orangeTimer.ResetTimer(true);
                orangeChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
                orangeChipText.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
            }
        }
        
        public void PauseAll()
        {
            globalTimer.Pause();
            globalTimer.Color = Color.red;

            blueTimer.Pause();
            blueTimer.Color = Color.red;

            orangeTimer.Pause();
            orangeTimer.Color = Color.red;
        }

        public void ResumeAll()
        {
            globalTimer.Resume();
            blueTimer.Resume();
            orangeTimer.Resume();
        }

        // public Timer CreateTimer(float startingTimeInSeconds, string name="New Timer", TextMeshProUGUI textComponent=null)
        // {
        //     var newTimer = Instantiate(timerPrefab);
        //     newTimer.name = name;
        //     newTimer.transform.SetParent(transform);
        //     Timer c_timer = newTimer.GetComponent<Timer>();

        //     if (textComponent != null)
        //     {
        //         c_timer.SetText(textComponent);
        //     }

        //     c_timer.Init(startingTimeInSeconds);
        //     inactiveTimers.Add(c_timer);
        //     return c_timer;
        // }
    }
}