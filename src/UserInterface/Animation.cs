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
          //  _proj = new Entity("proj", Color.White, Color.Transparent, '*', 1, 1);
            //Game.World.CurrentMap.Add(_proj);
        }
        public override void Execute()
        {
            _timer = new System.Timers.Timer(50);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //System.Console.WriteLine(counter);
            _proj.Position = new Point(_line[_counter].X, _line[_counter].Y);
            _counter++;
            //Game.uiManager.gameState = States.player;
            if (_counter >= _line.Count)
            {
                _counter = 0;
                _line = null;
              //  Game.World.CurrentMap.Remove(_proj);
               // Game.uiManager.gameState = States.main;
                _timer.Stop();
                _timer.Dispose();
            }
        }
    }
}