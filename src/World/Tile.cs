using System;
using Microsoft.Xna.Framework;
using GoRogue;
using SadConsole;
using StarsHollow.UserInterface;

namespace StarsHollow.World
{
    public class TileBase : Cell
    {
        public FOV FovMap { get; set; }
        public bool IsBlockingMove { get; set; }
        public bool IsBlockingLOS { get; set; }
        public bool IsExplored { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint MoveCostMod { get; set; }
        private int elavation;


        public TileBase(Color foreground, Color background, int glyph, bool blockingMove = false,
                         bool BlockingLOS = false, String name = "", String description = "") : base(foreground, background, glyph)
        {
            IsBlockingMove = blockingMove;
            IsBlockingLOS = BlockingLOS;
            IsVisible = false;
            IsExplored = false;
            Name = name;
            Description = description;
            MoveCostMod = 1;
            elavation = 0;
        }
    }

    // TODO: These should come fron JSON files.
    public class TileNull : TileBase
    {
        public TileNull(bool blocksMovement = true, bool blocksLOS = true) : base(Color.Transparent, Color.Transparent, 'x', blocksMovement, blocksLOS)
        {
            Name = "null";
        }
    }
    public class TileLand : TileBase
    {
        public TileLand(bool blocksMovement = false, bool blocksLOS = false) : base(Color.Transparent, Color.Brown, ' ', blocksMovement, blocksLOS)
        {
            Name = "land";
            MoveCostMod = 1;
        }
    }
    public class TileSwamp : TileBase
    {
        public TileSwamp(bool blocksMovement = false, bool blocksLOS = false) : base(Color.DarkGreen, Color.DarkOliveGreen, '"', blocksMovement, blocksLOS)
        {
            Name = "swamp";
            MoveCostMod = 2;
        }
    }
    public class TileWall : TileBase
    {
        public TileWall(bool blocksMovement = true, bool blocksLOS = true) : base(UserInterface.ColorScheme.Four, new Color(23, 23, 23), '.', blocksMovement, blocksLOS)
        {
            switch (GoRogue.DiceNotation.Dice.Roll("1d8"))
            {
                case 1:
                    Foreground = ColorScheme.Second;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    Foreground = ColorScheme.Three;
                    break;
                case 7:
                    Foreground = ColorScheme.Four;
                    break;
                case 8:
                    Foreground = ColorScheme.Five;
                    break;
                default:
                    Foreground = ColorScheme.Three;
                    break;

            }
            Name = "wall";
            MoveCostMod = 50;
        }
    }
    public class TileFloor : TileBase
    {
        public TileFloor(bool blocksMovement = false, bool blocksLOS = false) : base(Color.DarkGray, Color.Transparent, '.', blocksMovement, blocksLOS)
        {
            Name = "floor";
            MoveCostMod = 1;
        }
    }
    public class TileTrees : TileBase
    {
        public TileTrees(bool blocksMovement = false, bool blocksLOS = true) : base(Color.DarkGreen, Color.Transparent, 30, blocksMovement, blocksLOS)
        {
            Name = "trees";
            Description = "The long, green needles of the spruces sting as you walk between them.";
            MoveCostMod = 2;

            switch (GoRogue.DiceNotation.Dice.Roll("1d12"))
            {
                case 1:
                    Foreground = Color.SeaGreen;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    Foreground = Color.ForestGreen;
                    break;
                case 7:
                    Foreground = Color.DarkOliveGreen;
                    break;
                case 8:
                    Foreground = Color.DarkOrange;
                    break;
                default:
                    Foreground = Color.DarkGreen;
                    break;

            }
            switch (GoRogue.DiceNotation.Dice.Roll("1d10"))
            {
                case 1:
                    Glyph = 6;
                    break;
                default:
                    break;

            }
        }
    }
    public class TileGrass : TileBase
    {
        public TileGrass(bool blocksMovement = false, bool blocksLOS = false) : base(Color.Green, Color.Transparent, 'v', blocksMovement, blocksLOS)
        {
            Name = "grass";
            Description = "A patch of tall grass with some bushes here and there";
            switch (GoRogue.DiceNotation.Dice.Roll("1d7"))
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    Foreground = Color.ForestGreen;
                    break;
                case 7:
                    Foreground = Color.GreenYellow;
                    break;
            }
        }
    }
    public class TileRiver : TileBase
    {

        public TileRiver(bool blocksMovement = true, bool blocksLOS = false) : base(Color.LightSteelBlue, Color.DarkBlue, 247, blocksMovement)
        {
            Name = "river";
            Description = "Before you flows a slow river";
            MoveCostMod = 5;

            switch (GoRogue.DiceNotation.Dice.Roll("1d16"))
            {
                case 1:
                    Foreground = Color.CornflowerBlue;
                    break;
                case 2:
                case 3:
                    Foreground = Color.CadetBlue;
                    break;
                case 4:
                    Foreground = Color.NavajoWhite;
                    break;
                default:
                    break;

            }
        }
    }
}
