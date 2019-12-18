using System;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    static class Command
    {
        public static void Move(Entity ent, Point dir)
        {
            if (Game.UI._world.CurrentMap.IsTileWalkable(ent.Position + dir) &&
                !Game.UI._world.CurrentMap.IsThereEntityAt(ent.Position + dir))
            {
                    ent.GetComponent<CmpAction>().SetAction(new MoveBy(ent, dir));
            }

            // ent.GetComponent<CmpAction>().SetAction(new Actions.MoveBy(ent, dir));
        }
    }
}