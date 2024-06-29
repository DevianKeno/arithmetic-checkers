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
                globalTimer.SetTime(Ruleset.GetValue<float>("GlobalTimeSeconds"));
                globalTimer.SetFormat(TimerFormat.MM_SS);
                globalTimer.Begin();
            }
            
            if (Ruleset.GetValue<bool>("EnableTurnTimer"))
            {
                var turnTime = Ruleset.GetValue<float>("TurnTimeSeconds");
                
                blueTimer.SetTime(turnTime);
                blueTimer.SetFormat(TimerFormat.SS);
                
                orangeTimer.SetTime(turnTime);
                orangeTimer.SetFormat(TimerFormat.SS);

                if (Ruleset.GetValue<Side>("FirstTurn") == Side.Blue)
                {
                    blueTimer.Begin();
                    blueChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);

                    orangeChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                    orangeChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);

                } else if (Ruleset.GetValue<Side>("FirstTurn") == Side.Orange)
                {
                    orangeTimer.Begin();
                    orangeChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);

                    blueChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                    blueChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                }
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

                blueTimer.Reset(true);
                blueChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
                blueChipText.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
            } else
            {
                blueTimer.Stop();
                blueChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                blueChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                blueChipText.text = "";

                orangeTimer.Reset(true);
                orangeChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
                orangeChipText.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
            }
        }
        
        public void PauseAll()
        {
            globalTimer.Pause();
            blueTimer.Pause();
            orangeTimer.Pause();
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