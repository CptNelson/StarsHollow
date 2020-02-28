using System.Collections.Generic;
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
        public Entity Entity { get; private set; }
        public DeathEvent(Entity entity)
        {
            Entity = entity;
            Game.UI.MainWindow.Message(Entity.Name + " died!");
            if (Entity.HasComponent<CmpInput>())
            {
                Game.UI.MainWindow.Message("Game over!");
                Game.UI.MainWindow.MainLoop.Playing = false;
            }
            Game.UI.world.CurrentMap.Remove(Entity);
            Game.UI.MainWindow.MainLoop.EventsList.Remove(entity);

            Entity corpse = Game.UI.world.EntityFactory("corpse", "helpers.json");
            corpse.Name = "The corpse of a " + Entity.Name;
            corpse.NonBlocking = true;
            corpse.Position = Entity.Position;
            Game.UI.world.CurrentMap.Add(corpse);
        }
    }

}