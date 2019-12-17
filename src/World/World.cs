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

namespace StarsHollow.World
{
    public class WorldMap
    {
        private int mapWidth, mapHeight;
        private TileBase[] _worldMapTiles;
        private Map overworldMap;
        private Map currentMap;
        private Entity turnTimer;
        public Entity player;
        public Map OverworldMap { get => overworldMap; }
        public Map CurrentMap { get => currentMap; set => currentMap = value; }
        public WorldMap()
        {
        }

        public void CreateWorld(int width, int height)
        {
            mapWidth = width;
            mapHeight = height;

            _worldMapTiles = new TileBase[mapWidth * mapHeight];
            overworldMap = new Map(mapWidth, mapHeight);
            // map generator return both Map and GoRogue's ArrayMap. 
            Tuple<Map, ArrayMap<double>> maps = MapGenerator.GenerateWorld(mapWidth, mapHeight);
            overworldMap = maps.Item1;
            overworldMap.goMap = maps.Item2;
            CreateHelperEntities();
            AddWorldMapEntities();
            AddPlayer();

        }

        private void CreateHelperEntities()
        {
            // First create the helper entities and then add them to a game loop.
            turnTimer = EntityFactory("timer", "helpers.json");
            turnTimer.GetComponents();
            turnTimer.Actionable = true;
            overworldMap.Add(turnTimer);
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
            if (player == null)
            {
                player = EntityFactory("player", "player.json");
                player.Animation.CurrentFrame[0].Glyph = '@';
                player.Components.Add(new EntityViewSyncComponent());
                player.Position = overworldMap.GetRandomEmptyPosition();
                while (!overworldMap.IsSouthOfRiver(player.Position))
                    player.Position = overworldMap.GetRandomEmptyPosition();
                player.IsVisible = true;
                Console.Write("Pos: " + player.Position);
                player.Actionable = true;
                overworldMap.Add(player);
            }

        }

        private void AddWorldMapEntities()
        {
            GenerateFarms();


            void GenerateFarms()
            {
                for (int i = 0; i < 5; i++)
                {
                    //Entity farm = new Entity("farm", Color.RosyBrown, Color.Yellow, 'O', 1, 1, "farm", false);
                    // Entity farm = new Entity();

                    Entity farm = EntityFactory("farm", "overworld.json");
                    farm.Animation.CurrentFrame[0].Glyph = 15;
                    farm.Animation.CurrentFrame[0].Foreground = Color.RosyBrown;
                    //   farm.AddComponents(new List<IComponent> { new CmpHP(50) });
                    farm.Components.Add(new EntityViewSyncComponent());
                    farm.Position = overworldMap.GetRandomEmptyPosition();
                    while (!overworldMap.IsSouthOfRiver(farm.Position))
                        farm.Position = overworldMap.GetRandomEmptyPosition();
                    farm.IsVisible = false;
                    farm.Actionable = false;
                    farm.GetComponents();
                    overworldMap.Add(farm);
                }
            }
        }
    }
}
