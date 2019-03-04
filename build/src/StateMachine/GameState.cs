using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;
using System.IO;

namespace DuckGame.KillCounterMod
{

    public enum GameState
    {
        newMatchStarting,
        inMatch,
        matchEndedWithWinners,
        matchEndedWithoutWinners,
        skipped, // Unused
        notInMatch

    }
}
