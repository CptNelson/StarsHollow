using GoRogue;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarsHollow.World
{
    public class Map
    {
        public TileBase[] _tiles;
        public FOV Fov;
        public readonly int _width;
        public readonly int _height;
        // Keeps track of all the Entities on the map
        public GoRogue.MultiSpatialMap<Entity> Entities;
        // Creates unique ID's for all entities
        public static GoRogue.IDGenerator IDGenerator = new GoRogue.IDGenerator();
        //Build a new map with a specified width and height
        public ArrayMap<double> goMap;
        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            _tiles = new TileBase[width * height];
            Entities = new GoRogue.MultiSpatialMap<Entity>();
        }

        // =============MAP METHODS====================================
       
        public bool IsTileWalkable(Point location)
        {
            // first make sure that actor isn't trying to move
            // off the limits of the map
            if (location.X < 0 || location.Y < 0 || location.X >= _width || location.Y >= _height)
                return false;
            // then return whether the tile is walkable
            return !_tiles[location.Y * _width + location.X].IsBlockingMove;
        }
        public TileBase GetTileAt(Point location)
        {
            int index = location.Y * _width + location.X;
            if (index >= 0 && index < _tiles.Length)
            {
                return _tiles[location.Y * _width + location.X];
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
        public bool IsSouthOfRiver(Point location)
        {
            for (int y = _height-1; y > location.Y; y--)
            {
                if (_tiles[y * _width + location.X].Name == "river")
                {
                    return false;
                }
            }
            return true;
        }
        public bool CheckBounds(Point location)
        {
            if (location.X < _width && location.Y < _height && location.X >= 0 && location.Y >= 0)
                return true;
            else return false;
       }

        public Point GetRandomEmptyPosition()
        {
            Random rndNum = new Random();
            Point position = new Point(rndNum.Next(0, _width), rndNum.Next(0, _height));

            while (_tiles[position.Y * _width + position.X].IsBlockingMove || IsThereEntityAt(position))
            {
                position.X = rndNum.Next(0, _width);
                position.Y = rndNum.Next(0, _height);
            }
            return position;
        }
        public Point GetRandomWalkablePosition()
        {
            Random rndNum = new Random();
            Point position = new Point(rndNum.Next(0, _width), rndNum.Next(0, _height));
            while (_tiles[position.Y * _width + position.X].IsBlockingMove)
            {
                position.X = rndNum.Next(0, _width);
                position.Y = rndNum.Next(0, _height);
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
        public OverWorldMap(int width, int height):base(width, height)
        {
            localMaps = new Map[width, height];
        }
    }
}
