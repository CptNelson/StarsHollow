using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadConsole.Components;
using StarsHollow.Components;
using StarsHollow.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StarsHollow.UserInterface;

namespace StarsHollow.World
{
    public class WorldMap
    {
        private int _mapWidth, _mapHeight;
        private TileBase[] _worldMapTiles;
        private Map _overworldMap;
        private Entity _turnTimer;
        public Entity Player;
        public Map OverworldMap { get => _overworldMap; }
        public Map CurrentMap { get; set; }

        public WorldMap()
        {
        }

        public void CreateWorld(int width, int height)
        {
            _mapWidth = width;
            _mapHeight = height;

            _worldMapTiles = new TileBase[_mapWidth * _mapHeight];
            _overworldMap = new Map(_mapWidth, _mapHeight);
            // map generator return both Map and GoRogue's ArrayMap. 
            //Tuple<Map, ArrayMap<double>> maps = MapGenerator.GenerateWorld(_mapWidth, _mapHeight);
            Tuple<Map, ArrayMap<double>> maps = MapGenerator.GenerateLocalMap(_mapWidth, _mapHeight);
            _overworldMap = maps.Item1;
            _overworldMap.goMap = maps.Item2;
            CreateHelperEntities();
           // AddWorldMapEntities();
            AddPlayer();
            CreateGuard();

        }

        private void CreateHelperEntities()
        {
            // First create the helper entities and then add them to a game loop.
            _turnTimer = EntityFactory("timer", "helpers.json");
            _turnTimer.GetComponents();
            _turnTimer.Actionable = true;
            _overworldMap.Add(_turnTimer);
        }

        private Entity EntityFactory(string _name, string json)
        {
            Entity ent = new Entity();
            ent.Name = _name;
            JObject cmpList = JObject.Parse(Tools.LoadJson(json));
            //   ent.NonBlocking = (bool)cmpList[_name]["nonBlocking"];
            ent.AddComponentsFromFile(_name, json);
            // TODO: load whole entity  from file.

            //SadConsole.Serializer.Save<Entity>(ent, @"../../../res/json/test2.json", false);//JsonConvert.SerializeObject(ent);
            //Entity jsn2 = SadConsole.Serializer.Load<Entity>(@"test2.json", false);
            //Console.WriteLine(jsn2._components.Count);
            return ent;
        }

        private void AddPlayer()
        {
            if (Player != null) return;
            Player = EntityFactory("player", "player.json");
            Player.Animation.CurrentFrame[0].Glyph = '@';
            Player.Components.Add(new EntityViewSyncComponent());
            Player.Position = _overworldMap.GetRandomEmptyPosition();
            //  while (!_overworldMap.IsSouthOfRiver(Player.Position))
            //    Player.Position = _overworldMap.GetRandomEmptyPosition();
            Player.IsVisible = true;
            Console.Write("Pos: " + Player.Position);
            Player.Actionable = true;
            _overworldMap.Add(Player);
            Console.Write("errrrrr");
            //CurrentMap.Add(Player);

        }
        private void CreateGuard(int amount = 2)
        {
            for (int i = 0; i < amount; i++)
            {
                Entity guard = EntityFactory("guard", "level1.json");
                guard.Animation.CurrentFrame[0].Glyph = '@';
                guard.Animation.CurrentFrame[0].Foreground = ColorScheme.Four;
                guard.Components.Add(new EntityViewSyncComponent());
             //   int monsterPosition = rndNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                guard.Position = _overworldMap.GetRandomEmptyPosition();
                guard.IsVisible = false;
                guard.Actionable = true;
              //  guard.Time = Game.World.turnTimer.Time;
                _overworldMap.Add(guard);
            }
        }


        private void AddWorldMapEntities()
        {
        }
    }
}
