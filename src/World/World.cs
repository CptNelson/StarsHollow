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
        private EntityConverterJson converter;
        private bool IsNew { get; set; }

        public WorldMap()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                //TypeNameHandling = TypeNameHandling.All
            };

            converter = new EntityConverterJson();
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

            Console.WriteLine(Player.GetComponent<CmpHP>().Alive);
            File.WriteAllText(@"./res/json/saves/player.json", JsonConvert.SerializeObject(Player, converter));
            File.WriteAllText(@"./res/json/saves/timer.json", JsonConvert.SerializeObject(TurnTimer, converter));

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
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            JsonSerializer serializer = new JsonSerializer();
            //File.WriteAllText(@"./res/json/saves/entities.json", JsonConvert.SerializeObject(CurrentMap.Entities, settings));
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

        public void CreateWorld(int width, int height, bool isNew = true)
        {
            IsNew = isNew;

            mapWidth = width;
            mapHeight = height;

            LocalMap = new Map(mapWidth, mapHeight);
            // map generator returns both Map and GoRogue's ArrayMap. 
            Tuple<Map, ArrayMap<double>> maps;

            // If new game is started
            if (IsNew)
            {
                maps = MapGenerator.GenerateLocalMap(mapWidth, mapHeight);
            }
            else
            {
                double[,] tempMap = LoadCurrentMap();
                double[,] tempFovMap = LoadCurrentFovMap();
                maps = MapGenerator.GenerateLoadedMap(mapWidth, mapHeight, tempMap, tempFovMap);
            }

            LocalMap = maps.Item1;
            LocalMap.GoMap = maps.Item2;

            InitSystems();

            CreateHelperEntities();
            CreatePlayer();
            // CreateGuard();
        }

        private void CreateHelperEntities()
        {
            // First create the helper entities and then add them to a game loop.
            TurnTimer = EntityFactory("helpers", "timer");
            //TurnTimer = EntityFactory2("saves/timer.json");
            //TurnTimer.GetComponents();
            TurnTimer.IsActionable = true;
            LocalMap.Add(TurnTimer.Sprite);
        }
        private void CreatePlayer()
        {
            if (Player != null) return;
            Player = EntityFactory("player", "player");
        }

        private void CreateGuard(int amount = 2)
        {
            for (int i = 0; i < amount; i++)
            {
                Entity guard = EntityFactory("level1", "guard");
                //guard.GetComponent<CmpAI>().AddAIComponent(new GuardArea(guard));
            }
        }

        public Entity EntityFactory(string file, string name)
        {
            string json;
            if (IsNew)
                json = "prefabs/" + file;
            else
                json = "saves/" + file;

            Console.WriteLine(json);

            //Entity entity = JsonConvert.DeserializeObject<Entity>(json, converter);
            Entity entity = new Entity();

            // get the entity's data from json file and make a JObject represeting the entity
            JObject fullJSON = JObject.Parse(Tools.LoadJson(json));
            JObject entityJSON = (JObject)fullJSON[name];

            // TODO: create method for the graphical assingment.
            // FIXME: get colors from the json even if they are ColorScheme.Color. 

            entity.Sprite.Animation.CurrentFrame[0].Glyph = Convert.ToInt32(entityJSON["Glyph"]);
            System.Drawing.Color fg = System.Drawing.Color.FromName(entityJSON["FgColor"].ToString());
            entity.Sprite.Animation.CurrentFrame[0].Foreground = new Color(fg.R, fg.G, fg.B, fg.A);
            entity.Sprite.Animation.CurrentFrame[0].Background = Color.Transparent;

            JObject components = (JObject)entityJSON["EntComponents"];

            entity.AddComponentsFromFile(components);

            // TODO: attribute and stats to their own method/class
            if (entity.HasComponent<CmpHP>())
            {

                entity.GetComponent<CmpHP>().Hp += entity.GetComponent<CmpAttributes>().Guts / 2 + entity.GetComponent<CmpAttributes>().Vitality;
                entity.GetComponent<CmpHP>().CurrentHp = entity.GetComponent<CmpHP>().Hp;
            }


            // If new game is started
            if (IsNew)
            {
                entity.Sprite.ID = Map.IDGenerator.UseID();
                entity.Sprite.Position = LocalMap.GetRandomEmptyPosition();
            }
            else
            {
                entity.Sprite.ID = Convert.ToUInt32(entityJSON["ID"]);
                Object pos = (Object)entityJSON["Position"];
                entity.Sprite.Position = LocalMap.GetRandomEmptyPosition();
            }

            entity.Sprite.owner = entity;
            entity.IsActionable = Convert.ToBoolean(entityJSON["IsActionable"]);
            entity.Sprite.Name = Convert.ToString(entityJSON["Name"]);
            entity.Sprite.Components.Add(new EntityViewSyncComponent());

            if (entity.HasComponent<CmpBody>())
            {
                List<Entity> items = new List<Entity>();
                foreach (var Jitem in entityJSON["EntComponents"]["StarsHollow.World.CmpBody"]["ItemList"])
                {
                    var item = ItemFactory(Jitem);
                    item.GetComponent<CmpItem>().Holder = entity;
                    item.Sprite.Position = entity.Sprite.Position;
                    item.Sprite.IsVisible = false;
                    entity.GetComponent<CmpBody>().ItemList.Add(item);
                }
                entity.GetComponent<CmpBody>().ShowItems();
            }

            LocalMap.Add(entity.Sprite);
            return entity;
        }

        public Entity ItemFactory(JToken Jitem)
        {

            Entity item = new Entity();

            // get the entity's data from json file and make a JObject represeting the entity
            //JObject fullJSON = JObject.Parse(Tools.LoadJson(json));
            //JObject entityJSON = (JObject)fullJSON[name];

            // TODO: create method for the graphical assingment.
            // FIXME: get colors from the json even if they are ColorScheme.Color. 
            Console.WriteLine("item fac: " + Jitem);
            JObject itemJSON;

            if (IsNew)
            {
                JObject fullJSON = JObject.Parse(Tools.LoadJson("prefabs/weapons"));
                itemJSON = (JObject)fullJSON[Jitem.ToString()];

                item.Sprite.Animation.CurrentFrame[0].Glyph = Convert.ToInt32(itemJSON["Glyph"]);
                System.Drawing.Color fg = System.Drawing.Color.FromName(itemJSON["FgColor"].ToString());
                item.Sprite.Animation.CurrentFrame[0].Foreground = new Color(fg.R, fg.G, fg.B, fg.A);
                item.Sprite.Animation.CurrentFrame[0].Background = Color.Transparent;

                JObject components = (JObject)itemJSON["EntComponents"];

                item.AddComponentsFromFile(components);


                if (IsNew)
                {
                    item.Sprite.ID = Map.IDGenerator.UseID();
                    item.Sprite.Position = LocalMap.GetRandomEmptyPosition();
                }
                else
                {
                    item.Sprite.ID = Convert.ToUInt32(itemJSON["ID"]);
                    Object pos = (Object)itemJSON["Position"];
                    item.Sprite.Position = LocalMap.GetRandomEmptyPosition();
                }

                item.IsActionable = Convert.ToBoolean(itemJSON["IsActionable"]);
                item.Sprite.owner = item;
                item.Sprite.Name = Convert.ToString(itemJSON["Name"]);
                item.Sprite.Components.Add(new EntityViewSyncComponent());
            }





            LocalMap.Add(item.Sprite);
            return item;
        }

    }
}