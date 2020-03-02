using SadConsole;
using System;
using Microsoft.Xna.Framework;
using StarsHollow.World;
using Console = System.Console;

namespace StarsHollow.UserInterface
{
    // TODO: system for updating statuses.
    public class StatusWindow : Window
    {
        private SadConsole.Console statusConsole;

        private WorldMap map;
        private int windowBorderThickness = 4;

        public string timeOfDay = "Noon";
        public int daysGone = 0;

        public StatusWindow(int width, int height, string title, WorldMap map) : base(width, height)
        {
            CanDrag = true;
            Title = title.Align(HorizontalAlignment.Center, Width, (char)205);

            statusConsole = new SadConsole.Console(width - windowBorderThickness, height);
            statusConsole.Position = new Point(1, 1);
            statusConsole.Font = Fonts.normalSizeAnikkiFont;

            UseMouse = true;
            this.map = map;
            Children.Add(statusConsole);
        }

        public override void Draw(TimeSpan drawTime)
        {
            base.Draw(drawTime);
        }

        public void WriteInformation()
        {
            var attributes = map.Player.GetComponent<CmpAttributes>();
            var timer = map.TurnTimer.GetComponent<CmpTimer>();

            statusConsole.Clear();

            statusConsole.Print(1, 5, (timer.Hour + " : " + timer.Minute), ColorScheme.Second);
            statusConsole.Print(1, 6, ("Turn " + timer.Turn), ColorScheme.Second);
            statusConsole.Print(1, 7, timeOfDay, ColorScheme.Second);
            statusConsole.Print(1, 9, ("Midsummer"), ColorScheme.Five);
            statusConsole.Print(1, 10, ("* Sunny *"), Color.Yellow);

            statusConsole.Print(1, 12, "Strength: ", ColorScheme.Three);
            statusConsole.Print(11, 12, attributes.Strength.ToString(), ColorScheme.Second);
            statusConsole.Print(1, 13, "Agility: ", ColorScheme.Three);
            statusConsole.Print(11, 13, attributes.Agility.ToString(), ColorScheme.Second);
            statusConsole.Print(1, 14, "Vitality: ", ColorScheme.Three);
            statusConsole.Print(11, 14, attributes.Vitality.ToString(), ColorScheme.Second);
            statusConsole.Print(1, 15, "Looks ", ColorScheme.Three);
            statusConsole.Print(11, 15, attributes.Looks.ToString(), ColorScheme.Second);
            statusConsole.Print(1, 16, "Guts: ", ColorScheme.Three);
            statusConsole.Print(11, 16, attributes.Guts.ToString(), ColorScheme.Second);
            statusConsole.Print(1, 17, "Smarts: ", ColorScheme.Three);
            statusConsole.Print(11, 17, attributes.Smarts.ToString(), ColorScheme.Second);

            statusConsole.Print(1, 19, "HP: ", ColorScheme.Three);
            statusConsole.Print(5, 19, map.Player.GetComponent<CmpHP>().CurrentHp + "/" + map.Player.GetComponent<CmpHP>().Hp, ColorScheme.Second);
        }


    }

    /*
    public class SystemStatusWindow : Observer
    {
        // Change status window information

        public override void DoSomething(object sender, EventArgs e)
        {
            var _status = Game.UI.StatusConsole;
            var _event = (StatusEvent)sender;
            Entity player = _event._entity;
            int _hour = Game.UI._world.TurnTimer.GetComponent<CmpTimer>()._hour;
            _status._hpStatus = player.GetComponent<CmpHP>().Hp;

            if (_hour == 0)
            {
                _status._timeOfDay = "Night";
            }
            else if (_hour == 6)
            {
                _status._timeOfDay = "Morning";

            }
            else if (_hour == 12)
            {
                _status._timeOfDay = "Day";

            }
            else if (_hour == 18)
            {
                _status._timeOfDay = "Evening";
            }
            else if (_hour > 2)
            {
                _status._daysGone++; 
                _hour = 0;
            }


            Game.UI.StatusConsole.WriteInformation();
        }
    }
    */

}