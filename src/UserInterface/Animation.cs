using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GoRogue;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.UserInterface
{
    public class IAnimation
    {
        
    }
    public class Animation : IAnimation, IEntity
    {

        public bool Actionable { get; set; }
        public uint ID { get; set; }
        public uint Time { get; set; }

        protected Animation()
        {
            Actionable = true;
            Time = 1;
        }
        public virtual void Execute()
        {

        }
    }

    public class ProjectileAnimation : Animation
    {
        private static List<Coord> _line;
        private int _len;
        private static int _counter;
        private static Entity _proj;
        private static Timer _timer;
        public ProjectileAnimation(Point start, Point end)
        {
            _counter = 0;
            _line = Lines.Get(start, end, Lines.Algorithm.DDA).ToList();
            _proj = new Entity {Name = "proj"};
            _proj.Animation.CurrentFrame[0].Foreground = Color.White;
            _proj.Animation.CurrentFrame[0].Background = Color.Transparent;
            Game.UI.world.CurrentMap.Add(_proj);
        }
        public override void Execute()
        {
            _timer = new Timer(50);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _proj.Position = new Point(_line[_counter].X, _line[_counter].Y);
             
            switch (GoRogue.DiceNotation.Dice.Roll("1d8"))
            {
                case 1:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Second;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Three;
                    break;
                case 7:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Four;
                    break;
                case 8:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Five;
                    break;
                default:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Three;
                    break;

            }
            
            _counter++;
          //  Game.UI.MainWindow.GameState = States.Input;
            if (_counter >= _line.Count)
            {
                _counter = 0;
                _line = null;
                Game.UI.world.CurrentMap.Remove(_proj);
                Game.UI.MainWindow.GameState = States.Main;
                Console.WriteLine("main state");
                _timer.Stop();
                _timer.Dispose();
            }
        }
    }
}