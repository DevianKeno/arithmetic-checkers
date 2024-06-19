using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Damath
{
    public class TimerManager : MonoBehaviour
    {
        public Ruleset Ruleset { get; private set; }
        public float FadeDuration = 0.25f;
        public bool IgnoreTimeScale = false;

        [SerializeField] Timer GlobalTimer;
        [SerializeField] Timer BlueTimer;
        [SerializeField] Timer OrangeTimer;

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
                GlobalTimer.SetTime(Ruleset.GetValue<float>("GlobalTimerSeconds"));
                GlobalTimer.SetFormat(TimerFormat.MM_SS);
                GlobalTimer.Begin();
            }
            
            if (Ruleset.GetValue<bool>("EnableTurnTimer"))
            {
                var turnTime = Ruleset.GetValue<float>("TurnTimerSeconds");
                
                BlueTimer.SetTime(turnTime);
                BlueTimer.SetFormat(TimerFormat.SS);
                
                OrangeTimer.SetTime(turnTime);
                OrangeTimer.SetFormat(TimerFormat.SS);

                if (Ruleset.GetValue<Side>("FirstTurn") == Side.Blue)
                {
                    BlueTimer.Begin();
                    blueChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);

                    orangeChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                    orangeChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);

                } else if (Ruleset.GetValue<Side>("FirstTurn") == Side.Orange)
                {
                    OrangeTimer.Begin();
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
                OrangeTimer.Stop();
                orangeChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                orangeChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                orangeChipText.text = "";

                BlueTimer.Reset(true);
                blueChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
                blueChipText.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
            } else
            {
                BlueTimer.Stop();
                blueChipImage.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                blueChipText.CrossFadeAlpha(0.5f, FadeDuration, IgnoreTimeScale);
                blueChipText.text = "";

                OrangeTimer.Reset(true);
                orangeChipImage.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
                orangeChipText.CrossFadeAlpha(1f, FadeDuration, IgnoreTimeScale);
            }
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