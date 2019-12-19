using System;
using System.Threading;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    static class Command
    {
        public static void Move(Entity ent, Point dir)
        {
            if (Game.UI.world.CurrentMap.IsTileWalkable(ent.Position + dir) &&
                !Game.UI.world.CurrentMap.IsThereEntityAt(ent.Position + dir))
            {
                    ent.GetComponent<CmpAction>().SetAction(new MoveBy(ent, dir));
            }

            else if (Game.UI.world.CurrentMap.IsThereEntityAt(ent.Position + dir))
            {
                Entity target = Game.UI.world.CurrentMap.GetFirstEntityAt<Entity>(ent.Position + dir);      
                
                if (target.NonBlocking)
                    ent.GetComponent<CmpAction>().SetAction(new MoveBy(ent, dir));
                else
                {
                    if (target.HasComponent<CmpAI>() && target.GetComponent<CmpAI>().Aggression >= 2)
                    {
                        ent.GetComponent<CmpAction>().SetAction(new MeleeAttack(ent, dir));
                    } 
                }
            }

            // ent.GetComponent<CmpAction>().SetAction(new Actions.MoveBy(ent, dir));
        }

        public static void Shoot(Entity ent, Point location)
        {
            ent.GetComponent<CmpAction>().SetAction(new Shoot(ent, location));
            Console.WriteLine("shooting command");
        }
    }
}