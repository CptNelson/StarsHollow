using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarsHollow.World
{
    public class TileBase : Cell
    {
        // Movement and Line of Sight Flags
        private bool isBlockingMove;
        private bool isBlockingLOS;
        private bool isExplored;
        private string name;
        private string description;
        private uint moveCostMod;
        private int elavation;

        public FOV fovMap;

        public bool IsBlockingMove { get => isBlockingMove; set => isBlockingMove = value; }
        public bool IsExplored { get => isExplored; set => isExplored = value; }
        public bool IsBlockingLOS { get => isBlockingLOS; set => isBlockingLOS = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public uint MoveCostMod { get => moveCostMod; set => moveCostMod = value; }

        public TileBase(Color foreground, Color background, int glyph, bool blockingMove = false,
                         bool BlockingLOS = false, String name = "", String description = "") : base(foreground, background, glyph)
        {
            IsBlockingMove = blockingMove;
            IsBlockingLOS = BlockingLOS;
            IsVisible = false;
            isExplored = false;
            Name = name;
            Description = description;
            MoveCostMod = 1;
            elavation = 0;
        }
    }

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
        public TileWall(bool blocksMovement = true, bool blocksLOS = true) : base(Color.AntiqueWhite, Color.Transparent, '#', blocksMovement, blocksLOS)
        {
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
