using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("Kill Counter Mod")]

// The author of the mod
[assembly: AssemblyCompany("JoGooD")]

// The description of the mod
[assembly: AssemblyDescription("This mod will add kills count to score for each player (team). " +
    "Last standing will still gain a point. It will also close the XP window, so you don't have to wait after every match!")]

// The mod's version
[assembly: AssemblyVersion("2.2.1.1")]

namespace DuckGame.KillCounterMod
{
    public class KillCounterMod : Mod
    {

        static KillCounter killCounter;
        static StateMachine stateMachine;

        // The mod's priority; this property controls the load order of the mod.
        public override Priority priority
		{
			get { return base.priority; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
			base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
			base.OnPostInitialize();
            Network.Initialize();
            
            void thread()
            {
                while (!(Level.current is TeamSelect2)) // This prevent crash
                {
                    Thread.Sleep(1000);
                }
                DevConsole.Log(DCSection.Mod, "--KillCounter 2.2.1.1--", -1);
                File.WriteAllText("stateLog.txt", "\n");
                stateMachine = new StateMachine();
                killCounter = new KillCounter(stateMachine);
            }

            new Thread(thread).Start();
        }
	}
}
