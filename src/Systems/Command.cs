using Microsoft.Xna.Framework;
using StarsHollow.Components;
using StarsHollow.Utils;
using StarsHollow.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarsHollow.Commands
{
    static class Command
    {
        public static void Move(Entity ent, Point dir)
        {
            if (Game.UI._world.CurrentMap.IsTileWalkable(ent.Position + dir) &&
                !Game.UI._world.CurrentMap.IsThereEntityAt(ent.Position + dir))
            {
                    ent.GetComponent<CmpAction>().SetAction(new Actions.MoveBy(ent, dir));
                    Game.UI.AddMessage("asd");
            }

            // ent.GetComponent<CmpAction>().SetAction(new Actions.MoveBy(ent, dir));
        }
    }
}