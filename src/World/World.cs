using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadConsole.Components;
using StarsHollow.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
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

        public Map OverworldMap
        {
            get => _overworldMap;
        }

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

            JObject entityJSON = JObject.Parse(Tools.LoadJson(json));

            IDictionary<string, JToken> looks = (JObject) (entityJSON[_name]["look"]);
            Dictionary<string, string> looksDictionary = looks.ToDictionary(pair => pair.Key, pair =>
                (string) pair.Value);

            ent.Animation.CurrentFrame[0].Glyph =
                Convert.ToInt32(
                    looksDictionary["glyph"]); //Convert.ToInt32((JObject)entityJSON[_name]["look"]["glyph"]);
            ent.Animation.CurrentFrame[0].Foreground = Color.White;
            ent.Animation.CurrentFrame[0].Background = Color.Transparent;

            JObject components = (JObject) entityJSON[_name]["components"];

            ent.AddComponentsFromFile(components);
            ent.Components.Add(new EntityViewSyncComponent());

            return ent;
        }


        private void AddPlayer()
        {
            if (Player != null) return;
            Player = EntityFactory("player", "player.json");
           // Player.Components.Add(new EntityViewSyncComponent());
            Player.Position = _overworldMap.GetRandomEmptyPosition();
            Player.IsVisible = true;
            Console.Write("Pos: " + Player.Position);
            Player.Actionable = true;
            _overworldMap.Add(Player);
            Console.Write("errrrrr");
        }

        private void CreateGuard(int amount = 2)
        {
            for (int i = 0; i < amount; i++)
            {
                Entity guard = EntityFactory("guard", "level1.json");
                guard.Position = _overworldMap.GetRandomEmptyPosition();
                guard.IsVisible = false;
                guard.Actionable = true;
                _overworldMap.Add(guard);
            }
        }


        private void AddWorldMapEntities()
        {
        }
    }
}