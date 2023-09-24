using UnityEngine;

namespace Damath
{
    public class TimerManager : MonoBehaviour
    {
        public Ruleset Rules { get; private set; }
        public Timer GlobalTimer;
        public Timer BlueTimer;
        public Timer OrangeTimer;
        [SerializeField] GameObject BlueChip;
        [SerializeField] GameObject OrangeChip;
        [SerializeField] GameObject timerPrefab;

        void Awake()
        {
            Game.Events.OnRulesetDistribute += ReceiveRuleset;
            Game.Events.OnMatchBegin += Init;
            Game.Events.OnChangeTurn += SwapTurnTimer;
        }

        void OnDisable()
        {
            Game.Events.OnRulesetDistribute -= ReceiveRuleset;
            Game.Events.OnMatchBegin -= Init;
            Game.Events.OnChangeTurn -= SwapTurnTimer;
        }

        void ReceiveRuleset(Ruleset value)
        {
            if (Settings.EnableDebugMode)
            {
                Game.Console.Log($"[DEBUG]: Received ruleset");
            }
            Rules = value;
        }
        
        public void Init()
        {
            if ((bool) Rules["EnableTimer"]) 
            {
                if ((bool) Rules["EnableTurnTimer"])
                {
                    GlobalTimer.SetFormat(Format.MM_SS);
                    GlobalTimer.SetTime((float) Rules["GlobalTimerSeconds"]);
                    GlobalTimer.Begin();
                }
                if ((bool) Rules["EnableTurnTimer"])
                {
                    BlueTimer.SetFormat(Format.SS);
                    BlueTimer.SetTime((float) Rules["TurnTimerSeconds"]);
                    
                    OrangeTimer.SetFormat(Format.SS);
                    OrangeTimer.SetTime((float) Rules["TurnTimerSeconds"]);

                    if ((Side) Rules["FirstTurn"] == Side.Bot)
                    {
                        BlueTimer.Begin();
                        OrangeChip.SetActive(false);
                    } else if ((Side) Rules["FirstTurn"] == Side.Top)
                    {
                        OrangeTimer.Begin();
                        BlueChip.SetActive(false);
                    }
                }
            }
        }

        public void Init(MatchController match)
        {
            Init();
        }

        public void SwapTurnTimer(Side turnOf)
        {
            if (turnOf == Side.Bot)
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