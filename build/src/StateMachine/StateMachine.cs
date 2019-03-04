using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;
using System.IO;

namespace DuckGame.KillCounterMod
{ // Find the state of the game

    public class StateMachine : IAutoUpdate
    {
        public GameState state = GameState.notInMatch;
        //public String lastLevel;
        public bool allDead = false;
        private int _changes = 0;
        //public PublicGameMode access;



        public StateMachine()
        {
            AutoUpdatables.Add(this);
        }

        public void Update()
        {
            if (!(Level.current is GameLevel))
            {
                if (this.state != GameState.notInMatch)
                {
                    this.state = GameState.notInMatch;
                    File.AppendAllText("stateLog.txt", ++this._changes + " : not in match\n");
                }
                return;
            }
            if (!GameMode.started && !GameMode.firstDead)
            {
                if (this.state != GameState.newMatchStarting)
                {
                    this.state = GameState.newMatchStarting;
                    File.AppendAllText("stateLog.txt", ++this._changes + " : new match\n");
                }
                //lastLevel = Level.current.ToString();

                return;
            }
            if (GameMode.firstDead && GameMode.lastWinners.Count > 0)
            {
                if (this.state != GameState.matchEndedWithWinners)
                {
                    this.state = GameState.matchEndedWithWinners;
                    File.AppendAllText("stateLog.txt", ++this._changes + " : end win\n");
                }
                return;

            }
            bool isAllDead = true;
            if (!DuckNetwork.active) // Offline
            {
                foreach (Team team in Teams.all)
                {
                    foreach (Profile activeProfile in team.activeProfiles)
                    {
                        if (activeProfile.duck != null && !activeProfile.duck.dead)
                        {
                            isAllDead = false;
                        }
                    }
                }
            }
            else // Online
            {
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (profile.duck != null && !profile.duck.dead)
                    {
                        isAllDead = false;
                    }
                }
            }
            this.allDead = isAllDead;
            if (!this.allDead)
            {
                if (this.state != GameState.inMatch)
                {
                    this.state = GameState.inMatch;
                    File.AppendAllText("stateLog.txt", ++this._changes + " : in match\n");
                }
                return;
            }
            else
            {
                if (this.state != GameState.matchEndedWithoutWinners)
                {
                    this.state = GameState.matchEndedWithoutWinners;
                    File.AppendAllText("stateLog.txt", ++this._changes + " : end looser no win\n");
                }
                return;
                
            }
            
        }

    }
}
