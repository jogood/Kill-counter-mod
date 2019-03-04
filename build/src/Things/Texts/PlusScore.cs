using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.KillCounterMod
{
    public class PlusScore : Thing
    {
        private float _wait = 1f;
        private BitmapFont _font;
        private Profile _profile;
        public String text;
        private bool _temp;

        public PlusScore(float xpos, float ypos, Profile p, bool temp = false, String textScore = "0")
          : base(xpos, ypos, (Sprite)null)
        {
            foreach (Thing thing in Level.current.things[typeof(PlusOne)])
            {
                try
                {
                    Level.Remove(thing);
                }
                catch { }

            }
            this._font = new BitmapFont("biosFont", 8, -1);
            this._profile = p;
            this._temp = temp;
            this.text = textScore;
            this.layer = Layer.Blocks;
            this.depth = (Depth)0.9f;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            if (!this._temp)
                this._wait -= 0.01f;
            if ((double)this._wait >= 0.0)
                return;
            Level.Remove((Thing)this);
        }

        public override void Draw()
        {
            if (this._profile == null || this._profile.persona == null || this.anchor == (Thing)null)
                return;
            this.position = this.anchor.position;
            string text = "+" + this.text;
            float xpos = this.x - this._font.GetWidth(text, false, (InputProfile)null) / 2f;
            this._font.Draw(text, xpos - 1f, this.y - 1f, Color.Black, (Depth)0.8f, (InputProfile)null, false);
            this._font.Draw(text, xpos + 1f, this.y - 1f, Color.Black, (Depth)0.8f, (InputProfile)null, false);
            this._font.Draw(text, xpos - 1f, this.y + 1f, Color.Black, (Depth)0.8f, (InputProfile)null, false);
            this._font.Draw(text, xpos + 1f, this.y + 1f, Color.Black, (Depth)0.8f, (InputProfile)null, false);
            Color c = new Color((byte)this._profile.persona.color.x, (byte)this._profile.persona.color.y, (byte)this._profile.persona.color.z);
            this._font.Draw(text, xpos, this.y, c, (Depth)0.9f, (InputProfile)null, false);
        }
    }

	
}
