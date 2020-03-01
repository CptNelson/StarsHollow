using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GoRogue;
using GoRogue.MapViews;
using System.Runtime.Serialization;

namespace StarsHollow.World
{
    [DataContract]
    public class Map
    {
        [DataMember]
        public readonly int Width;
        [DataMember]
        public readonly int Height;
        [DataMember]
        public TileBase[] Tiles { get; set; }
        [DataMember]
        public FOV Fov { get; set; }
        // Keeps track of all the Entities on the map
        public GoRogue.MultiSpatialMap<Entity> Entities { get; set; }
        // Creates unique ID's for all entities
        public static GoRogue.IDGenerator IDGenerator = new GoRogue.IDGenerator();
        //Build a new map with a specified width and height
        public ArrayMap<double> GoMap;

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new TileBase[width * height];
            Entities = new GoRogue.MultiSpatialMap<Entity>();
        }

        // =============MAP METHODS====================================

        public bool IsTileWalkable(Point location)
        {
            // first make sure that actor isn't trying to move
            // off the limits of the map
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            // then return whether the tile is walkable
            return !Tiles[location.Y * Width + location.X].IsBlockingMove;
        }
        public TileBase GetTileAt(Point location)
        {
            int index = location.Y * Width + location.X;
            if (index >= 0 && index < Tiles.Length)
            {
                return Tiles[location.Y * Width + location.X];
            }
            else
                return new TileNull();
        }
        public T GetFirstEntityAt<T>(Point location) where T : Entity
        {
            return Entities.GetItems(location).OfType<T>().FirstOrDefault();
        }
        public List<Entity> GetEntitiesAt(Point location)
        {
            List<Entity> _entities = new List<Entity>();
            _entities = Entities.GetItems(location).ToList<Entity>();
            return _entities;
        }
        public bool IsThereEntityAt(Point location)
        {
            Entity ent = GetFirstEntityAt<Entity>(location);
            if (ent != null)
                return true;
            else return false;
        }
        public bool CheckBounds(Point location)
        {
            if (location.X < Width && location.Y < Height && location.X >= 0 && location.Y >= 0)
                return true;
            else return false;
        }

        public Point GetRandomEmptyPosition()
        {
            Random rndNum = new Random();
            Point position = new Point(rndNum.Next(0, Width), rndNum.Next(0, Height));

            while (Tiles[position.Y * Width + position.X].IsBlockingMove || IsThereEntityAt(position))
            {
                position.X = rndNum.Next(0, Width);
                position.Y = rndNum.Next(0, Height);
            }
            return position;
        }
        public Point GetRandomWalkablePosition()
        {
            Random rndNum = new Random();
            Point position = new Point(rndNum.Next(0, Width), rndNum.Next(0, Height));
            while (Tiles[position.Y * Width + position.X].IsBlockingMove)
            {
                position.X = rndNum.Next(0, Width);
                position.Y = rndNum.Next(0, Height);
            }
            return position;
        }

        // Removes an Entity from the MultiSpatialMap
        public void Remove(Entity entity)
        {
            // remove from SpatialMap
            Entities.Remove(entity);
            // Link up the entity's Moved event to a new handler
            entity.Moved -= OnEntityMoved;
        }

        // Adds an Entity to the MultiSpatialMap
        public void Add(Entity entity)
        {
            // add entity to the SpatialMap
            Entities.Add(entity, entity.Position);
            // Link up the entity's Moved event to a new handler
            entity.Moved += OnEntityMoved;
        }

        // When the Entity's .Moved value changes, it triggers this event handler
        // which updates the Entity's current position in the SpatialMap
        private void OnEntityMoved(object sender, Entity.EntityMovedEventArgs args)
        {
            Entities.Move(args.Entity as Entity, args.Entity.Position);
        }
    }
    public class OverWorldMap : Map
    {
        public Map[,] localMaps;
        public OverWorldMap(int width, int height) : base(width, height)
        {
            localMaps = new Map[width, height];
        }
    }
}
