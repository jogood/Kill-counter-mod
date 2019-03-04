using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;
using System.IO;

namespace DuckGame.KillCounterMod
{
    class KillCount
    // Keep track of data about number of kills
    {

        public string profileTeam;
        public int initialKills = 0; // Total kills ever at the beginning of the match
        public int endKills = 0; // Total kills ever at the end of the match
        public int killScore = 0; // Number of kills
    }


    class KillCounter : IAutoUpdate
    {

        public int nMatch = 0; // To Remove 
        //private bool _read = false;
        //public bool sendScore = true;
        public StateMachine stateMachine;
        private int _beginEventIndex = 0;
        private int _endEventIndex = 0;
        private bool _scoreAdded = true;
        private int _localKills = 0; // Used for online purpose only: Represent the number of kills for the local user.
        private float _wait = 2f;


        public KillCounter()
        {
            AutoUpdatables.Add(this);
            this.stateMachine = new StateMachine();
        }
        public KillCounter(StateMachine s)
        {

            AutoUpdatables.Add(this);
            this.stateMachine = s;
        }

        public void Update()
        {
            //don't do it in the editor!
            //(Level.current is TeamSelect2 || Level.current is GameLevel || Level.current is RockScoreboard)
            //if (Level.current is Editor || Level.current is TeamSelect2 || Level.current is TitleScreen || Level.current is RockScoreboard) return;
            //if (Level.current is RockScoreboard && sendScore) Teams.active[0].score = 111;
            if (!(Level.current is GameLevel))
            {

                return;
            }

            if (this.stateMachine.state == GameState.newMatchStarting)
            {

                this._scoreAdded = false;
                this._wait = 2f;
                if (DuckNetwork.active)
                { 
                     this._localKills = DuckNetwork.profiles[DuckNetwork.localDuckIndex].stats.kills;
                }
                return;
            }

            if (!DuckNetwork.active && !this._scoreAdded && (this.stateMachine.state == GameState.matchEndedWithoutWinners || this.stateMachine.state == GameState.matchEndedWithWinners)) // Local
            {
                if (Event.events.Count() > 0) this._endEventIndex = Event.events.Count() - 1;
                else this._endEventIndex = 0;
                if (Event.events[this._endEventIndex].dealer != null || Event.events[this._endEventIndex].victim != null) return;


                List<String> victims = new List<String>(4);
                Dictionary<String, int> addScore = new Dictionary<string, int>();

                for (this._beginEventIndex = this._endEventIndex - 1; this._beginEventIndex > 0; this._beginEventIndex--)
                {
                    try
                    {
                        if (Event.events[this._beginEventIndex].dealer.team.name != Event.events[this._beginEventIndex].victim.team.name && !(victims.Contains(Event.events[this._beginEventIndex].victim.name)))
                        {

                            victims.Add(Event.events[this._beginEventIndex].victim.name);
                            Event.events[this._beginEventIndex].dealer.team.score += 1;
                            if (!(addScore.ContainsKey(Event.events[this._beginEventIndex].dealer.team.name))) addScore.Add(Event.events[this._beginEventIndex].dealer.team.name, 1);
                            else addScore[Event.events[this._beginEventIndex].dealer.team.name] += 1;
                        }
                        else if (Event.events[this._beginEventIndex].victim != null)
                        {
                            victims.Add(Event.events[this._beginEventIndex].victim.name);
                        }
                    }
                    catch { }
                    if (Event.events[this._beginEventIndex].dealer == null && Event.events[this._beginEventIndex].victim == null) break; // Begin of the match in Events
                }

                // PlusScore (+1)
                foreach (Team t in Teams.active)
                {

                    foreach (Profile profile in t.activeProfiles)
                    {
                        int s = 0;
                        foreach (Profile winner in GameMode.lastWinners)
                        {
                            if (profile == winner)
                            {
                                s = 1;
                            }
                        }
                        try
                        {
                            PlusScore plus = new PlusScore(0.0f, 0.0f, profile, false, (addScore[profile.team.name] + s).ToString());
                            plus.anchor = (Anchor)((Thing)profile.duck);
                            plus.anchor.offset = new Vec2(0.0f, -16f);
                            plus.depth = (Depth)0.95f;
                            Level.Add((Thing)plus);
                        }
                        catch
                        {
                            PlusScore plus = new PlusScore(0.0f, 0.0f, profile, false, (s).ToString());
                            plus.anchor = (Anchor)((Thing)profile.duck);
                            plus.anchor.offset = new Vec2(0.0f, -16f);
                            plus.depth = (Depth)0.95f;
                            Level.Add((Thing)plus);
                        }

                    }
                }
                this._scoreAdded = true;
            }
            if (this.stateMachine.state == GameState.matchEndedWithoutWinners && this._wait > 0)
            {
                this._wait -= 0.1f;
                return;
            }
                if (DuckNetwork.active && !this._scoreAdded && (this.stateMachine.state == GameState.matchEndedWithoutWinners || this.stateMachine.state == GameState.matchEndedWithWinners))
            {

                // Calcul number of kills
                bool isWinner = false;
                int s = 0;
                if (this.stateMachine.state == GameState.matchEndedWithWinners)
                {
                    foreach (Profile winner in GameMode.lastWinners)
                    {
                        if (winner == DuckNetwork.profiles[DuckNetwork.localDuckIndex])
                        {
                            isWinner = true;
                            s = 1;
                        }
                    }
                }
                
                Send.Message(new NMSendKills((byte)DuckNetwork.localDuckIndex, DuckNetwork.profiles[DuckNetwork.localDuckIndex].stats.kills - this._localKills, isWinner));
                DuckNetwork.profiles[DuckNetwork.localDuckIndex].team.score += DuckNetwork.profiles[DuckNetwork.localDuckIndex].stats.kills - this._localKills;
                DevConsole.Log(DCSection.Mod, "Team: " + DuckNetwork.profiles[DuckNetwork.localDuckIndex].team.name + "  Score: |WHITE|" + DuckNetwork.profiles[DuckNetwork.localDuckIndex].team.score, -1);
                        try
                        {
                            PlusScore plus = new PlusScore(0.0f, 0.0f, DuckNetwork.profiles[DuckNetwork.localDuckIndex], false, (DuckNetwork.profiles[DuckNetwork.localDuckIndex].stats.kills - this._localKills + s).ToString());
                            plus.anchor = (Anchor)((Thing)DuckNetwork.profiles[DuckNetwork.localDuckIndex].duck);
                            plus.anchor.offset = new Vec2(0.0f, -16f);
                            plus.depth = (Depth)0.95f;
                            Level.Add((Thing)plus);
                        }
                        catch { }
                this._scoreAdded = true;

            }
        }
        
    }
}
