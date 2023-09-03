using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Damath
{
    public class TimerManager : MonoBehaviour
    {
        public Ruleset Rules { get; private set; }
        public List<Timer> activeTimers = new List<Timer>();
        public List<Timer> inactiveTimers = new List<Timer>();
        public Timer globalTimer;
        public Timer blueTimer;
        public Timer orangeTimer;
        public TextMeshProUGUI globalTimerTMP;
        public TextMeshProUGUI blueTimerTMP;
        public TextMeshProUGUI orangeTimerTMP;
        [SerializeField] GameObject timerPrefab;

        void Awake()
        {
            
        }

        void OnEnable()
        {
            Game.Events.OnMatchBegin += Init;
            Game.Events.OnRulesetReturn += ReceiveRuleset;
        }

        void OnDisable()
        {
            Game.Events.OnMatchBegin -= Init;
            Game.Events.OnRulesetReturn -= ReceiveRuleset;
        }

        void Update()
        {
            if (globalTimer != null)
            {
                globalTimerTMP.text = globalTimer.ToMM_SS();
            }
            
            if (blueTimer != null)
            {
                blueTimerTMP.text = blueTimer.ToSS();
            }
            
            if (orangeTimer != null)
            {
                orangeTimerTMP.text = orangeTimer.ToSS();
            }
        }
        
        public void Init(MatchController match)
        {
            if (Rules.EnableTimer) 
            {
                if (Rules.EnableGlobalTimer)
                {
                    InitGlobal(Rules.globalTimerSeconds);
                }
                if (Rules.EnableTurnTimer)
                {
                    InitTurn(Rules.turnTimerSeconds);
                }
                StartAll();
            }
        }

        public void ReceiveRuleset(Ruleset rules)
        {
            Rules = rules;
        }

        /// <summary>
        /// 
        /// </summary>
        void InitTurn(float timeInSeconds)
        {
            blueTimer = CreateTimer(timeInSeconds, "Blue Turn Timer");
            orangeTimer = CreateTimer(timeInSeconds, "Orange Turn Timer");
        }

        void InitGlobal(float timeInSeconds)
        {
            globalTimer = CreateTimer(timeInSeconds, "Global Timer");
        }

        public Timer CreateTimer(float startingTimeInSeconds, string name="New Timer", TextMeshProUGUI textComponent=null)
        {
            var newTimer = Instantiate(timerPrefab);
            newTimer.name = name;
            newTimer.transform.SetParent(transform);
            Timer c_timer = newTimer.GetComponent<Timer>();

            if (textComponent != null)
            {
                c_timer.SetText(textComponent);
            }

            c_timer.Init(startingTimeInSeconds);
            inactiveTimers.Add(c_timer);
            return c_timer;
        }

        public Timer GetTimer(string name)
        {
            foreach (Timer t in activeTimers)
            {
                if (t.name == name) return t;
            }
            return null;
        }

        public void StartTimer(string name)
        {
            foreach (Timer t in inactiveTimers)
            {
                if (t.name != name) continue;

                _StartTimer(t);
                break;
            }
        }

        public void StopTimer(string name)
        {
            foreach (Timer t in inactiveTimers)
            {
                if (t.name != name) continue;

                _StopTimer(t);
                break;
            }
        }

        public void StartAll()
        {
            foreach (Timer t in inactiveTimers)
            {
                if (t.name != name) continue;

                _StartTimer(t);
                break;
            }
        }
        
        public void StopAll()
        {
            foreach (Timer t in inactiveTimers)
            {
                if (t.name != name) continue;

                _StopTimer(t);
                break;
            }
        }

        void _StartTimer(Timer t)
        {
            t.Begin();
            activeTimers.Add(t);
            inactiveTimers.Remove(t);
        }
        
        void _StopTimer(Timer t)
        {
            t.Stop();
            activeTimers.Remove(t);
            inactiveTimers.Add(t);
        }

        public void SetActive(Timer timer, bool value)
        {

        }
    }
}