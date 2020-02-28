using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    public class DamageEvent : Subject
    {
        public Entity _target;
        public int _damage;
        public DamageEvent(Entity target, int damage)
        {
            _target = target;
            _damage = damage;
        }
    }

    public class DeathEvent : Subject
    {
        public Entity _entity;
        public DeathEvent(Entity entity)
        {
            _entity = entity;
            Game.UI.MainWindow.Message(_entity.Name + " died!");
            if (_entity.HasComponent<CmpInput>())
            {
                Game.UI.MainWindow.Message("Game over!");
                Game.UI.MainWindow.MainLoop.playing = false;
            }
            Game.UI.world.CurrentMap.Remove(_entity);
            Game.UI.MainWindow.MainLoop.EventsList.Remove(entity);

            Entity corpse = new Entity(1, 1) { Name = "The corpse of " + _entity.Name };
            corpse.Animation.CurrentFrame[0].Glyph = '%';
            corpse.Animation.CurrentFrame[0].Foreground = Color.IndianRed;
            corpse.Animation.CurrentFrame[0].Background = Color.Transparent;

            corpse.AddComponents(new List<IComponent> { new CmpEdibleItem() }); ;
            corpse.isActionable = false;
            corpse.NonBlocking = true;
            corpse.Position = _entity.Position;
            Game.UI.world.CurrentMap.Add(corpse);

        }
    }

}