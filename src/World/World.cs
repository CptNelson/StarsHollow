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
using StarsHollow.Engine;
using StarsHollow.UserInterface;

namespace StarsHollow.World
{
    public class WorldMap
    {
        private int _mapWidth, _mapHeight;
        private TileBase[] _worldMapTiles;
        public SystemMover SystemMover;
        public SystemSkills SystemSkills;
        public SystemDamage SystemDamage;

        private Map _overworldMap;
        public Entity TurnTimer;
        public Entity Player;

        public Map OverworldMap
        {
            get => _overworldMap;
        }

        public Map CurrentMap { get; set; }

        public WorldMap()
        {
        }

        public void InitSystems()
        {
            SystemMover = new SystemMover();
            SystemSkills = new SystemSkills();
            SystemDamage = new SystemDamage();
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

            InitSystems();

            CreateHelperEntities();
            // AddWorldMapEntities();
            AddPlayer();
            CreateGuard();
        }

        private void CreateHelperEntities()
        {
            // First create the helper entities and then add them to a game loop.
            TurnTimer = EntityFactory("timer", "helpers.json");
            TurnTimer.GetComponents();
            TurnTimer.IsActionable = true;
            _overworldMap.Add(TurnTimer);
        }

        public Entity EntityFactory(string name, string json)
        {
            Entity ent = new Entity();
            ent.Name = name;

            JObject entityJSON = JObject.Parse(Tools.LoadJson(json));
            Console.WriteLine(entityJSON[name]["look"]);
            IDictionary<string, JToken> looks = (JObject)(entityJSON[name]["look"]);
            Dictionary<string, string> looksDictionary = looks.ToDictionary(pair => pair.Key, pair =>
                (string)pair.Value);

            ent.Animation.CurrentFrame[0].Glyph =
                Convert.ToInt32(
                    looksDictionary["glyph"]); //Convert.ToInt32((JObject)entityJSON[_name]["look"]["glyph"]);
            ent.Animation.CurrentFrame[0].Foreground = Color.White;
            ent.Animation.CurrentFrame[0].Background = Color.Transparent;

            JObject components = (JObject)entityJSON[name]["components"];

            Console.WriteLine(entityJSON);

            ent.AddComponentsFromFile(components);

            if (ent.HasComponent<CmpHP>())
            {
                ent.GetComponent<CmpHP>().Hp += ent.GetComponent<CmpAttributes>().Guts / 2 + ent.GetComponent<CmpAttributes>().Vitality;
                ent.GetComponent<CmpHP>().CurrentHp = ent.GetComponent<CmpHP>().Hp;
            }

            ent.Components.Add(new EntityViewSyncComponent());

            return ent;
        }


        private void AddPlayer()
        {
            if (Player != null) return;
            Player = EntityFactory("player", "player.json");
            Player.GetComponent<CmpBody>().ItemList.Add(EntityFactory("stun gun", "weapons.json"));
            Player.GetComponent<CmpBody>().RightHand.Add(Player.GetComponent<CmpBody>().ItemList.First());
            Player.Position = _overworldMap.GetRandomEmptyPosition();
            Player.IsVisible = true;
            Player.IsActionable = true;
            _overworldMap.Add(Player);
        }

        private void CreateGuard(int amount = 2)
        {
            for (int i = 0; i < amount; i++)
            {
                Entity guard = EntityFactory("guard", "level1.json");
                guard.Position = _overworldMap.GetRandomEmptyPosition();
                guard.IsVisible = false;
                guard.IsActionable = true;
                _overworldMap.Add(guard);
            }
        }


        private void AddWorldMapEntities()
        {
        }
    }
}