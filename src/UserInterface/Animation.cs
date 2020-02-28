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

        public bool isActionable { get; set; }
        public uint ID { get; set; }
        public uint entityTime { get; set; }

        protected Animation()
        {
            isActionable = true;
            entityTime = 1;
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
            _proj = new Entity { Name = "proj" };
            _proj.Animation.CurrentFrame[0].Foreground = Color.White;
            _proj.Animation.CurrentFrame[0].Background = Color.Transparent;
            _proj.Animation.CurrentFrame[0].Glyph = '*';
            Game.UI.world.CurrentMap.Add(_proj);
        }
        public override void Execute()
        {
            _timer = new Timer(25);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            int roll = GoRogue.DiceNotation.Dice.Roll("1d4");
            switch (roll)
            {
                case 1:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Second;
                    break;
                case 2:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Three;
                    break;
                case 3:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Four;
                    break;
                case 4:
                    _proj.Animation.CurrentFrame[0].Foreground = ColorScheme.Five;
                    break;

            }

            _proj.Animation.IsDirty = true;

            //Console.WriteLine("counter: " + _counter + " Line: " + _line.Count);
            _proj.Position = new Point(_line[_counter].X, _line[_counter].Y);

            Game.UI.MainWindow.DisplayFOV();
            _counter++;
            if (_counter >= _line.Count)
            {
                System.Threading.Thread.Sleep(25); // TODO: Find a way to use Timer for this.
                _counter = 0;
                _line = null;
                Game.UI.world.CurrentMap.Remove(_proj);
                Game.UI.MainWindow.GameState = States.Main;
                _timer.Stop();
                _timer.Dispose();
            }
        }
    }
}