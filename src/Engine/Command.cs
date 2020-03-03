using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    /*
        Commands are called from player input and AI. This way the Entity player is controlling
        is easy to swap. 
    */
    static class Command
    {
        public static void Move(Entity actor, Point dir)
        {
            Map map = Game.UI.world.CurrentMap;

            if (map.IsTileWalkable(actor.Sprite.Position + dir) &&
                !map.IsThereEntityAt(actor.Sprite.Position + dir))
            {
                actor.GetComponent<CmpAction>().SetAction(new MoveBy(actor, dir));
            }

            else if (map.IsThereEntityAt(actor.Sprite.Position + dir))
            {
                List<Entity> targets = map.GetEntitiesAt(actor.Sprite.Position + dir);

                // get all entities at position, and check if anyone is !NonBlocking

                Entity target = null;

                // TODO: make sure that the entity that is top on the tile gets chosen as target

                foreach (Entity ent in targets)
                {
                    if (!ent.NonBlocking)
                        target = ent;
                }

                if (target == null)
                    actor.GetComponent<CmpAction>().SetAction(new MoveBy(actor, dir));
                else
                    actor.GetComponent<CmpAction>().SetAction(new MeleeAttack(actor, dir));
            }
        }

        public static void Shoot(Entity actor, Point location)
        {
            actor.GetComponent<CmpAction>().SetAction(new Shoot(actor, location));
        }

        public static void Crouch(Entity actor)
        {
            actor.GetComponent<CmpAction>().SetAction(new Crouch(actor));
        }
    }
}