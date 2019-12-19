using System.Collections.Generic;
using SadConsole;
using System;
using Microsoft.Xna.Framework;
using StarsHollow.World;
using StarsHollow.Engine;
using StarsHollow.Utils;
using Console = System.Console;

namespace StarsHollow.UserInterface
{

    public class StatusWindow : Window
    {
        //public SystemStatusWindow statusWindowSystem = new SystemStatusWindow();
        // the messageConsole displays the active messages
        private SadConsole.Console _statusConsole;

        private WorldMap _map;
        // account for the thickness of the window border to prevent UI element spillover
        private int _windowBorderThickness = 4;

        //public int _hpStatus;

        public string _timeOfDay = "Noon";
        public int _daysGone = 0;
        

        // Create a new window with the title centered
        // the window is draggable by default
        public StatusWindow(int width, int height, string title, WorldMap map) : base(width, height)
        {
            // Ensure that the window background is the correct colour
            //Theme.WindowTheme.FillStyle.Background = Color.Black;
            CanDrag = true;
            Title = title.Align(HorizontalAlignment.Center, Width, (char)205);

            _statusConsole = new SadConsole.Console(width - _windowBorderThickness,height);
            _statusConsole.Position = new Point(1, 1);
            //_statusConsole.ViewPort = new Rectangle(0, 0, width - 1, height - _windowBorderThickness);
            _statusConsole.Font = Fonts.normalSizeAnikkiFont;
            
         //   WriteInformation();
            // enable mouse input
            UseMouse = true;
            _map = map;
            // Add the child consoles to the window
            Children.Add(_statusConsole);
        }

        //Remember to draw the window!
        public override void Draw(TimeSpan drawTime)
        {
            base.Draw(drawTime);
        }

        public void WriteInformation()
        {
            _statusConsole.Clear();

            //_statusConsole.Print(5, 3, _hpStatus.ToString(), ColorScheme.Second);
            Console.WriteLine(_map.TurnTimer);
            _statusConsole.Print(1, 5, (_map.TurnTimer.GetComponent<CmpTimer>().Hour + " : " + _map.TurnTimer.GetComponent<CmpTimer>().Minute), ColorScheme.Second);
            _statusConsole.Print(1, 6, ("Turn " + _map.TurnTimer.GetComponent<CmpTimer>().Turn), ColorScheme.Second);
            _statusConsole.Print(1, 7, _timeOfDay, ColorScheme.Second);
            _statusConsole.Print(1, 9, ("Midsummer"), ColorScheme.Five);
            _statusConsole.Print(1, 10, ("* Sunny *"), Color.Yellow);
            
            _statusConsole.Print(1, 12, "Strength: ", ColorScheme.Three);
            _statusConsole.Print(11, 12, _map.Player.GetComponent<CmpAttributes>().Strength.ToString(), ColorScheme.Second);
            _statusConsole.Print(1, 13, "Agility: ", ColorScheme.Three);
            _statusConsole.Print(11, 13, _map.Player.GetComponent<CmpAttributes>().Agility.ToString(), ColorScheme.Second);
            _statusConsole.Print(1, 14, "Vitality: ", ColorScheme.Three);
            _statusConsole.Print(11, 14, _map.Player.GetComponent<CmpAttributes>().Vitality.ToString(), ColorScheme.Second);
            _statusConsole.Print(1, 15, "Charisma ", ColorScheme.Three);
            _statusConsole.Print(11, 15, _map.Player.GetComponent<CmpAttributes>().Charisma.ToString(), ColorScheme.Second);
            _statusConsole.Print(1, 16, "Guts: ", ColorScheme.Three);
            _statusConsole.Print(11, 16, _map.Player.GetComponent<CmpAttributes>().Hunch.ToString(), ColorScheme.Second);
            _statusConsole.Print(1, 17, "Smarts: ", ColorScheme.Three);
            _statusConsole.Print(11, 17, _map.Player.GetComponent<CmpAttributes>().Smarts.ToString(), ColorScheme.Second);
            
            _statusConsole.Print(1, 19, "HP: ", ColorScheme.Three);
            _statusConsole.Print(5, 19, _map.Player.GetComponent<CmpHP>().CurrentHp + "/" + _map.Player.GetComponent<CmpHP>().Hp, ColorScheme.Second);
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