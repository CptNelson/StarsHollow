using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    public class DamageEvent : Subject
    {
        public Entity Target { get; private set; }
        public int Damage { get; private set; }
        public DamageEvent(Entity target, int damage)
        {
            Target = target;
            Damage = damage;
        }
    }

    public class DeathEvent : Subject
    {
        //public Entity Entity { get; private set; }
        public DeathEvent(Entity entity)
        {

            Game.UI.MainWindow.Message(entity.Sprite.Name + " died!");
            if (entity.HasComponent<CmpInput>())
            {
                Game.UI.MainWindow.Message("Game over!");
                Game.UI.MainWindow.MainLoop.Playing = false;
            }

            Entity corpse = Game.UI.world.EntityFactory("prefabs/helpers", "corpse");

            corpse.Sprite.Name = "The corpse of a " + entity.Sprite.Name;
            corpse.Sprite.Position = entity.Sprite.Position;
            Game.UI.world.CurrentMap.Add(corpse.Sprite);

            Game.UI.world.CurrentMap.Remove(entity.Sprite);
            Game.UI.MainWindow.MainLoop.EventsList.Remove(entity);

        }
    }

}