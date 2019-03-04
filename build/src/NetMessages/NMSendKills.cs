using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.KillCounterMod
{
    public class NMSendKills : NMDuckNetworkEvent
    {
        public byte index;
        public int kills;
        public bool isWinner;

        public NMSendKills()
        {
        }

        public NMSendKills(byte idx, int nKills, bool isWinner = false)
        {
            this.index = idx;
            this.kills = nKills;
            this.isWinner = isWinner;
        }

        public override void Activate()
        {
                if ((int)this.index >= 0 && (int)this.index < 4)
                {
                    DuckNetwork.profiles[(int)this.index].team.score += this.kills;
                    try
                    {
                    int s = 0;

                    if (this.isWinner) s = 1;

                    PlusScore plus = new PlusScore(0.0f, 0.0f, DuckNetwork.profiles[(int)this.index], false, (kills + s).ToString());
                        plus.anchor = (Anchor)((Thing)DuckNetwork.profiles[(int)this.index].duck);
                        plus.anchor.offset = new Vec2(0.0f, -16f);
                        plus.depth = (Depth)0.95f;
                        Level.Add((Thing)plus);
                    DevConsole.Log(DCSection.Mod, "Team: " + DuckNetwork.profiles[(int)this.index].team.name + "  Score: |WHITE|" + DuckNetwork.profiles[(int)this.index].team.score, -1);
                }
                catch { }
                   
                }
                base.Activate();
        }
    }
}
