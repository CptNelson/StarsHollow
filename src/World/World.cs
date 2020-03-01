using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SadConsole.Components;
using StarsHollow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using StarsHollow.Engine;

namespace StarsHollow.World
{
    public class WorldMap
    {
        public SystemMover SystemMover;
        public SystemSkills SystemSkills;
        public SystemDamage SystemDamage;

        public Map OverworldMap {get; private set;}
        public Entity TurnTimer {get; set;}
        public Entity Player {get; set;}
        public Map CurrentMap { get; set; }

        private int mapWidth, mapHeight;
        private TileBase[] worldMapTiles;

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
            mapWidth = width;
            mapHeight = height;

            worldMapTiles = new TileBase[mapWidth * mapHeight];
            OverworldMap = new Map(mapWidth, mapHeight);
            // map generator return both Map and GoRogue's ArrayMap. 
            //Tuple<Map, ArrayMap<double>> maps = MapGenerator.GenerateWorld(_mapWidth, _mapHeight);
            Tuple<Map, ArrayMap<double>> maps = MapGenerator.GenerateLocalMap(mapWidth, mapHeight);
            OverworldMap = maps.Item1;
            OverworldMap.GoMap = maps.Item2;

            InitSystems();

            CreateHelperEntities();
            AddPlayer();
            CreateGuard();
        }

        private void CreateHelperEntities()
        {
            // First create the helper entities and then add them to a game loop.
            TurnTimer = EntityFactory("timer", "helpers.json");
            TurnTimer.GetComponents();
            TurnTimer.IsActionable = true;
            OverworldMap.Add(TurnTimer);
        }

        public Entity EntityFactory(string name, string json)
        {
            Entity ent = new Entity();
            ent.Name = name;

            // get the entity's data from json file and make a JObject represeting the entity
            JObject entityJSON = JObject.Parse(Tools.LoadJson(json));

            // TODO: create method for the graphical assingment.
            // get glyph info from the entityJSON and make a dictionary out of them
            IDictionary<string, JToken> looks = (JObject)(entityJSON[name]["look"]);
            Dictionary<string, string> looksDictionary = looks.ToDictionary(pair => pair.Key, pair =>
                (string)pair.Value);

            // TODO: get colors from the json 
            ent.Animation.CurrentFrame[0].Glyph =
                Convert.ToInt32(looksDictionary["glyph"]);             
            ent.Animation.CurrentFrame[0].Foreground = Color.White;
            ent.Animation.CurrentFrame[0].Background = Color.Transparent;

            
            // TODO: create method for component attachments
            JObject components = (JObject)entityJSON[name]["components"];

            ent.AddComponentsFromFile(components);

            // TODO: attribute and stats to their own method/class
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
            Player.Position = OverworldMap.GetRandomEmptyPosition();
            Player.IsVisible = true;
            Player.IsActionable = true;
            OverworldMap.Add(Player);
        }

        private void CreateGuard(int amount = 2)
        {
            for (int i = 0; i < amount; i++)
            {
                Entity guard = EntityFactory("guard", "level1.json");
                guard.Position = OverworldMap.GetRandomEmptyPosition();
                guard.IsVisible = false;
                guard.IsActionable = true;
                OverworldMap.Add(guard);
            }
        }
    }
}