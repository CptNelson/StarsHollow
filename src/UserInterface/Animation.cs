using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GoRogue;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.UserInterface
{
    public class IAnimation { }
    public class Animation : IAnimation, IEntity
    {
        public bool IsActionable { get; set; }
        public uint ID { get; set; }
        public uint EntityTime { get; set; }

        protected Animation()
        {
            IsActionable = true;
            EntityTime = 1;
        }
        public virtual void Execute() { }
    }

    public class ProjectileAnimation : Animation
    {
        private static List<Coord> line;
        private int length;
        private static int counter;
        private static Entity projectile;
        private static Timer timer;
        private int speed;
        public ProjectileAnimation(Point start, Point end, int speed = 25)
        {
            counter = 0;
            this.speed = speed;
            line = Lines.Get(start, end, Lines.Algorithm.DDA).ToList();
            //TODO: get proj entity from json
            projectile = new Entity();
            projectile.Sprite.Name = "projectile";
            projectile.Sprite.Animation.CurrentFrame[0].Foreground = Color.White;
            projectile.Sprite.Animation.CurrentFrame[0].Background = Color.Transparent;
            projectile.Sprite.Animation.CurrentFrame[0].Glyph = '*';
            Game.UI.world.CurrentMap.Add(projectile.Sprite);
        }
        public override void Execute()
        {
            timer = new Timer(speed);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //TODO: This should also come from the projectile json
            int roll = GoRogue.DiceNotation.Dice.Roll("1d4");
            switch (roll)
            {
                case 1:
                    projectile.Sprite.Animation.CurrentFrame[0].Foreground = ColorScheme.Second;
                    break;
                case 2:
                    projectile.Sprite.Animation.CurrentFrame[0].Foreground = ColorScheme.Three;
                    break;
                case 3:
                    projectile.Sprite.Animation.CurrentFrame[0].Foreground = ColorScheme.Four;
                    break;
                case 4:
                    projectile.Sprite.Animation.CurrentFrame[0].Foreground = ColorScheme.Five;
                    break;
            }

            projectile.Sprite.Animation.IsDirty = true;

            projectile.Sprite.Position = new Point(line[counter].X, line[counter].Y);

            Game.UI.MainWindow.DisplayFOV();

            counter++;
            if (counter >= line.Count)
            {
                System.Threading.Thread.Sleep(25); // TODO: Find a way to use Timer for this.
                counter = 0;
                line = null;
                Game.UI.world.CurrentMap.Remove(projectile.Sprite);
                Game.UI.MainWindow.GameState = States.Main;
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}