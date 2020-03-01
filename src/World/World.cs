using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SadConsole.Components;
using StarsHollow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using StarsHollow.Engine;
using System.IO;
using Newtonsoft.Json;

namespace StarsHollow.World
{

    /* ============================================================================
        World hosts all the Entities, Tiles and Systems inside it.
    


       ============================================================================ */
    public class WorldMap
    {
        public SystemMover SystemMover;
        public SystemSkills SystemSkills;
        public SystemDamage SystemDamage;

        public Map LocalMap { get; private set; }
        public Map CurrentMap { get; set; }
        public Entity TurnTimer { get; set; }
        public Entity Player { get; set; }

        private int mapWidth, mapHeight;

        public WorldMap()
        {
        }

        public void InitSystems()
        {
            //TODO: maybe these should be inside a list?
            SystemMover = new SystemMover();
            SystemSkills = new SystemSkills();
            SystemDamage = new SystemDamage();
        }

        public void SaveCurrentGame()
        {
            SaveCurrentMap();
        }
        public void SaveCurrentMap()
        {
            double[,] tempMap = new double[LocalMap.Width, LocalMap.Height];
            double[,] tempFovMap = new double[LocalMap.Width, LocalMap.Height];

            foreach (var pos in LocalMap.GoMap.Positions())
            {


                if (LocalMap.GoMap[pos] == 0) // floor
                {
                    tempMap[pos.X, pos.Y] = 0;
                }
                else
                {
                    tempMap[pos.X, pos.Y] = 1;
                }
                if (!CurrentMap.Tiles[pos.ToIndex(CurrentMap.Width)].IsExplored)
                {
                    tempFovMap[pos.X, pos.Y] = 0;
                }
                else
                {
                    tempFovMap[pos.X, pos.Y] = 1;
                }
            }
            File.WriteAllText(@"./res/json/saves/map.json", JsonConvert.SerializeObject(tempMap, Formatting.Indented));
            File.WriteAllText(@"./res/json/saves/mapfov.json", JsonConvert.SerializeObject(tempFovMap, Formatting.Indented));
            File.WriteAllText(@"./res/json/saves/entities.json", JsonConvert.SerializeObject(CurrentMap.Entities, Formatting.Indented));
        }
        public double[,] LoadCurrentMap()
        {
            double[,] tempMap;
            using (StreamReader file = File.OpenText(@"./res/json/saves/map.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                tempMap = (double[,])serializer.Deserialize(file, typeof(double[,]));
            }
            return tempMap;
        }
        public double[,] LoadCurrentFovMap()
        {
            double[,] tempFovMap;
            using (StreamReader file = File.OpenText(@"./res/json/saves/mapfov.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                tempFovMap = (double[,])serializer.Deserialize(file, typeof(double[,]));
            }
            return tempFovMap;
        }

        public void CreateWorld(int width, int height)
        {
            mapWidth = width;
            mapHeight = height;

            LocalMap = new Map(mapWidth, mapHeight);
            // map generator returns both Map and GoRogue's ArrayMap. 
            //Tuple<Map, ArrayMap<double>> maps = MapGenerator.GenerateLocalMap(mapWidth, mapHeight);

            double[,] tempMap = LoadCurrentMap();
            double[,] tempFovMap = LoadCurrentFovMap();
            Tuple<Map, ArrayMap<double>> maps = MapGenerator.GenerateLoadedMap(mapWidth, mapHeight, tempMap, tempFovMap);

            LocalMap = maps.Item1;
            LocalMap.GoMap = maps.Item2;

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
            LocalMap.Add(TurnTimer);
        }

        public Entity EntityFactory(string name, string json)
        {
            Entity ent = new Entity();
            ent.Sprite.Name = name;

            // get the entity's data from json file and make a JObject represeting the entity
            JObject entityJSON = JObject.Parse(Tools.LoadJson(json));

            // TODO: create method for the graphical assingment.
            // get glyph info from the entityJSON and make a dictionary out of them
            IDictionary<string, JToken> looks = (JObject)(entityJSON[name]["look"]);
            Dictionary<string, string> looksDictionary = looks.ToDictionary(pair => pair.Key, pair =>
                (string)pair.Value);

            // FIXME: get colors from the json even if they are ColorScheme.Color. 
            ent.Sprite.Animation.CurrentFrame[0].Glyph = Convert.ToInt32(looksDictionary["glyph"]);
            System.Drawing.Color fg = System.Drawing.Color.FromName(looksDictionary["fg"]);

            ent.Sprite.Animation.CurrentFrame[0].Foreground = new Color(fg.R, fg.G, fg.B, fg.A);
            ent.Sprite.Animation.CurrentFrame[0].Background = Color.Transparent;


            // TODO: create method for component attachments
            JObject components = (JObject)entityJSON[name]["components"];

            ent.AddComponentsFromFile(components);

            // TODO: attribute and stats to their own method/class
            if (ent.HasComponent<CmpHP>())
            {
                ent.GetComponent<CmpHP>().Hp += ent.GetComponent<CmpAttributes>().Guts / 2 + ent.GetComponent<CmpAttributes>().Vitality;
                ent.GetComponent<CmpHP>().CurrentHp = ent.GetComponent<CmpHP>().Hp;
            }

            ent.Sprite.Components.Add(new EntityViewSyncComponent());

            return ent;
        }

        private void AddPlayer()
        {
            if (Player != null) return;
            Player = EntityFactory("player", "player.json");
            Player.GetComponent<CmpBody>().ItemList.Add(EntityFactory("stun gun", "weapons.json"));
            Player.GetComponent<CmpBody>().RightHand.Add(Player.GetComponent<CmpBody>().ItemList.First());
            Player.Sprite.Position = LocalMap.GetRandomEmptyPosition();
            Player.Sprite.IsVisible = true;
            Player.IsActionable = true;
            LocalMap.Add(Player);
        }

        private void CreateGuard(int amount = 2)
        {
            for (int i = 0; i < amount; i++)
            {
                Entity guard = EntityFactory("guard", "level1.json");
                guard.Sprite.Position = LocalMap.GetRandomEmptyPosition();
                guard.Sprite.IsVisible = false;
                guard.IsActionable = true;
                LocalMap.Add(guard);
            }
        }
    }
}