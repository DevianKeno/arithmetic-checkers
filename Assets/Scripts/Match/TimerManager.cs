using System;
using UnityEngine;

namespace Damath
{
    public class TimerManager : MonoBehaviour
    {
        public Ruleset Ruleset { get; private set; }
        public Timer GlobalTimer;
        public Timer BlueTimer;
        public Timer OrangeTimer;

        [SerializeField] GameObject BlueChip;
        [SerializeField] GameObject OrangeChip;
        [SerializeField] GameObject timerPrefab;

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
            if (!(bool) Ruleset["EnableTimer"]) return;

            if ((bool) Ruleset["EnableGlobalTimer"])
            {
                GlobalTimer.SetFormat(TimerFormat.MM_SS);
                GlobalTimer.SetTime((float) Ruleset["GlobalTimerSeconds"]);
                GlobalTimer.Begin();
            }
            
            if ((bool) Ruleset["EnableTurnTimer"])
            {
                BlueTimer.SetFormat(TimerFormat.SS);
                BlueTimer.SetTime((float) Ruleset["TurnTimerSeconds"]);
                
                OrangeTimer.SetFormat(TimerFormat.SS);
                OrangeTimer.SetTime((float) Ruleset["TurnTimerSeconds"]);

                if ((Side) Ruleset["FirstTurn"] == Side.Blue)
                {
                    BlueTimer.Begin();
                    OrangeChip.SetActive(false);
                } else if ((Side) Ruleset["FirstTurn"] == Side.Orange)
                {
                    OrangeTimer.Begin();
                    BlueChip.SetActive(false);
                }
            }
            
        }

        public void SwapTurnTimer(Side turnOf)
        {
            if (turnOf == Side.Blue)
            {
                OrangeTimer.Stop();
                OrangeChip.SetActive(false);

                BlueChip.SetActive(true);
                BlueTimer.Reset(true);
            } else
            {
                BlueTimer.Stop();
                BlueChip.SetActive(false);

                OrangeChip.SetActive(true);
                OrangeTimer.Reset(true);
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