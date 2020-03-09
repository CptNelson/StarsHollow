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
            SaveMapEntities();

        }

        private void SaveMapEntities()
        {
            // First create save files for Player and TurnTimer
            SaveEntity(@"./res/json/saves/timer.json", new List<string> { JsonConvert.SerializeObject(TurnTimer, converter) });
            SaveEntity(@"./res/json/saves/player.json", new List<string> { JsonConvert.SerializeObject(Player, converter) });

            // Then for the other entities in the Map, excluding already saved entities.
            List<string> entList = new List<string>();
            foreach (Sprite ent in LocalMap.Entities.Items)
            {
                if (!String.Equals(ent.Name, "player") && !String.Equals(ent.Name, "timer"))
                {
                    entList.Add(JsonConvert.SerializeObject(ent.owner, converter));
                }
            }
            SaveEntity(@"./res/json/saves/entities.json", entList);
        }

        private void SaveEntity(string filepath, List<string> savedEntityList)
        {
            // Clear old save, add [] and write the entities.
            File.WriteAllText(filepath, String.Empty);
            File.WriteAllText(filepath, "[");
            foreach (var ent in savedEntityList)
            {
                File.AppendAllText(filepath, ent + "," + Environment.NewLine);
            }
            string fileContent = File.ReadAllText(filepath);
            // Remove the last to characters
            fileContent = fileContent.Remove(fileContent.Length - 2) + "]";
            // Write content back to the file
            File.WriteAllText(filepath, fileContent);
        }
        private void SaveCurrentMap()
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
            CreateGuard();
        }

        private void LoadEntities()
        {
            List<int> entityIDList = new List<int>();
        }

        private void CreateHelperEntities()
        {
            // First create the helper entities and then add them to a game loop.
            TurnTimer = EntityFactory("timer", "timer");
            TurnTimer.IsActionable = true;
            TurnTimer.Sprite.Position = new Point(-2, -2);
            LocalMap.Add(TurnTimer.Sprite);
        }
        private void CreatePlayer()
        {
            if (Player != null) return;
            Player = EntityFactory("player", "player");
        }

        private void CreateGuard(int amount = 2)
        {
            EntityFactory("entities", "guard");
        }

        public Entity EntityFactory(string file, string nameOfEntity)
        {
            string json;
            string name = nameOfEntity;
            if (IsNew)
                json = "prefabs/" + file;
            else
                json = "saves/" + file;

            Entity entity = null;

            // get the entity's data from json file and make a JObject represeting the entity
            // TODO: HOW TO GET MULTIPLE ENTITIES WITH SAME NAME
            JArray arrayJSON = JArray.Parse(Tools.LoadJson(json));
            //JObject entityJSON = GetJObjectByName(name, arrayJSON);

            foreach (var result in arrayJSON)
            {
                entity = new Entity();
                JObject entityJSON = (JObject)result;

                // TODO: create method for the graphical assingment.
                // FIXME: get colors from the json even if they are ColorScheme.Color. 
                entity.Sprite.Animation.CurrentFrame[0].Glyph = Convert.ToInt32(entityJSON["Glyph"]);

                var fg = entityJSON["FgColor"];
                entity.Sprite.Animation.CurrentFrame[0].Foreground = new Color((float)fg["R"], (float)fg["G"], (float)fg["B"], (float)fg["A"]);
                entity.Sprite.Animation.CurrentFrame[0].Background = Color.Transparent;

                JObject components = (JObject)entityJSON["EntComponents"];

                entity.AddComponentsFromFile(components);

                // If new game is started
                if (IsNew)
                {
                    if (entity.HasComponent<CmpHP>())
                    {
                        entity.GetComponent<CmpHP>().Hp += entity.GetComponent<CmpAttributes>().Guts / 2 + entity.GetComponent<CmpAttributes>().Vitality;
                        entity.GetComponent<CmpHP>().CurrentHp = entity.GetComponent<CmpHP>().Hp;
                    }

                    entity.Sprite.ID = Map.IDGenerator.UseID();
                    entity.Sprite.Position = LocalMap.GetRandomEmptyPosition();
                }
                else
                {
                    entity.Sprite.ID = Convert.ToUInt32(entityJSON["ID"]);
                    var pos = entityJSON["Position"];
                    entity.Sprite.Position = new Point((int)pos["X"], (int)pos["Y"]);
                }
                entity.Sprite.IsVisible = Convert.ToBoolean(entityJSON["IsVisible"]);
                entity.Sprite.owner = entity;
                entity.IsActionable = Convert.ToBoolean(entityJSON["IsActionable"]);
                entity.Sprite.Name = Convert.ToString(entityJSON["Name"]);

                // TODO: attribute and stats to their own method/class
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

                entity.Sprite.Components.Add(new EntityViewSyncComponent());

                LocalMap.Add(entity.Sprite);
            }
            return entity;
        }

        public Entity ItemFactory(JToken Jitem)
        {
            Entity item = new Entity();

            JObject itemJSON = (JObject)Jitem;

            // TODO: create method for the graphical assingment.
            // FIXME: get colors from the json even if they are ColorScheme.Color. 
            item.Sprite.Animation.CurrentFrame[0].Glyph = Convert.ToInt32(itemJSON["Glyph"]);
            var fg = itemJSON["FgColor"];
            item.Sprite.Animation.CurrentFrame[0].Foreground = new Color((float)fg["R"], (float)fg["G"], (float)fg["B"], (float)fg["A"]);
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
                var pos = itemJSON["Position"];
                item.Sprite.Position = new Point((int)pos["X"], (int)pos["Y"]);
            }

            item.IsActionable = Convert.ToBoolean(itemJSON["IsActionable"]);
            item.Sprite.owner = item;
            item.Sprite.Name = Convert.ToString(itemJSON["Name"]);
            item.Sprite.Components.Add(new EntityViewSyncComponent());

            //LocalMap.Add(item.Sprite);
            return item;
        }

        private JToken[] getAllID(JArray array)
        {
            return array.Children().Select(a => a["ID"]).ToArray();
        }

        public JObject GetJObjectByName(string name, JArray array)
        {
            string entityNameToFind = name;

            JObject entity;

            entity = array.Values<JObject>()
            .Where(m => m["Name"].Value<string>() == entityNameToFind)
            .FirstOrDefault();

            if (entity != null)
            {
                return entity;
            }
            else
            {
                Console.WriteLine("match not found");
                return null;
            }
        }
    }
    public static partial class JTokenExtensions
    {
        public static JObject[] FilterObjectsByValue<T>(this JObject root, string someProp, T someValue)
        {
            var comparer = new JTokenEqualityComparer();
            var someValueToken = JToken.FromObject(someValue);
            var objs = root.DescendantsAndSelf()
                .OfType<JObject>()
                .Where(t => comparer.Equals(t[someProp], someValueToken))
                .ToArray();

            return objs;
        }
    }
}