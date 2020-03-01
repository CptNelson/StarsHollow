using System;
using Microsoft.Xna.Framework;
using SimplexNoise;
using GoRogue;
using GoRogue.MapViews;
using StarsHollow.Utils;
using System.IO;
using Newtonsoft.Json;

namespace StarsHollow.World
{
    public static class MapGenerator
    {
        private static Map map; // Temporarily store the map currently worked on
        private static ArrayMap<double> goMap;
        private static Map mapLocal;
        public static ArrayMap<double> goMapLocal { get; set; }

        public static Tuple<Map, ArrayMap<double>> GenerateLocalMap(int mapWidth, int mapHeight)
        {
            mapLocal = new Map(mapWidth, mapHeight);
            goMapLocal = new ArrayMap<double>(mapWidth, mapHeight);
            var goMapLocalBool = new ArrayMap<bool>(mapWidth, mapHeight);

            GoRogue.MapGeneration.QuickGenerators.GenerateRandomRoomsMap(goMapLocalBool, 8, 6, 12, 8);
            //GoRogue.MapGeneration.QuickGenerators.GenerateRectangleMap(goMapLocalBool);
            ArrayMap<bool> tempGoMap = new ArrayMap<bool>(mapWidth, mapHeight);

            foreach (var pos in goMapLocal.Positions())
            {
                if (goMapLocalBool[pos]) // floor
                {
                    goMapLocal[pos] = 0;
                    mapLocal.Tiles[pos.ToIndex(mapWidth)] = new TileFloor();
                    tempGoMap[pos] = true;
                }

                else
                {
                    goMapLocal[pos] = 1;
                    mapLocal.Tiles[pos.ToIndex(mapWidth)] = new TileWall();
                    tempGoMap[pos] = false;
                }

                mapLocal.Tiles[pos.ToIndex(mapWidth)].FovMap = new FOV(tempGoMap);
            }

            return Tuple.Create(mapLocal, goMapLocal);
        }

        public static Tuple<Map, ArrayMap<double>> GenerateLoadedMap(int mapWidth, int mapHeight, double[,] loadedMap)
        {
            mapLocal = new Map(mapWidth, mapHeight);
            goMapLocal = new ArrayMap<double>(mapWidth, mapHeight);
            ArrayMap<bool> tempGoMap = new ArrayMap<bool>(mapWidth, mapHeight);
            foreach (var pos in goMapLocal.Positions())
            {

                if (loadedMap[pos.X, pos.Y] == 0) // floor
                {
                    goMapLocal[pos] = 0;
                    mapLocal.Tiles[pos.ToIndex(mapWidth)] = new TileFloor();
                    tempGoMap[pos] = true;
                }
                else
                {
                    goMapLocal[pos] = 1;
                    mapLocal.Tiles[pos.ToIndex(mapWidth)] = new TileWall();
                    tempGoMap[pos] = false;
                }
                mapLocal.Tiles[pos.ToIndex(mapWidth)].FovMap = new FOV(tempGoMap);
            }

            return Tuple.Create(mapLocal, goMapLocal);
        }

        public static Tuple<Map, ArrayMap<double>> GenerateWorld(int mapWidth, int mapHeight)
        {
            map = new Map(mapWidth, mapHeight);
            goMap = new ArrayMap<double>(mapWidth, mapHeight);
            GenerateTheValley();
            //GenerateLocalMaps();

            void GenerateTheValley()
            {
                float[,] elevationSum = new float[mapWidth, mapHeight];
                float[,] moistureSum = new float[mapWidth, mapHeight];
                float[,] particleMap = new float[mapWidth, mapHeight];

                float[,] elevationFinal = new float[mapWidth, mapHeight];
                GenerateRollingParticleMask(9000, 50);
                GenerateElevationMap(0.85f, 0.023f);
                GenerateMoistureMap();
                // Go through all the positions and use the values to change them.
                foreach (var pos in goMap.Positions())
                {
                    float oldElevationValue = (float)Math.Round(elevationSum[pos.X, pos.Y] / 10, 0) / 10;
                    float moistureValue = (float)Math.Round(moistureSum[pos.X, pos.Y] / 10, 0) / 10;
                    float oldParticleValue = (float)Math.Round(particleMap[pos.X, pos.Y] * 10, 0) / 10;

                    float particleValue = (1.0f - oldParticleValue) * 1.0f;
                    float elevationValue = (oldElevationValue + particleValue * 0.65f);// * 1.3f;
                                                                                       // log(oldParticleValue);
                                                                                       //    _map._tiles[pos.ToIndex(mapWidth)] = new TileLand();

                    //_map._tiles[pos.ToIndex(mapWidth)].Background =  Color.White * particleValue;

                    //  float value = elevationSum[pos.X, pos.Y]/100;
                    if (elevationValue < 0.3f)
                        elevationValue += 0.0f;
                    else if (elevationValue > 0.7)
                        elevationValue -= -0.0f;
                    if (elevationValue > 0.5f)
                        elevationValue -= -0.0f;
                    elevationFinal[pos.X, pos.Y] = elevationValue;
                    GenerateRiver();

                    goMap[pos] = elevationValue;

                    Biome(pos, elevationValue, moistureValue);
                    //if (_map._tiles[pos.ToIndex(mapWidth)].Name == "trees")
                    // _goMap[pos] += 0.1f;
                    // Console.WriteLine("tile: " + elevationSum[pos.X, pos.Y] + " elev: " + elevationValue);

                }
                foreach (var pos in goMap.Positions())
                {
                    CreateFOV(elevationFinal, pos);
                }


                void GenerateElevationMap(float exp = 0.93f, float scl = 0.035f)
                {

                    // Create the Simplex noise heightmap
                    // add different frequencies to make it interesting
                    Noise.Seed = Tools.RandomNumber.GetRandomInt(1, 666666); // Optional                
                    float scale = scl;
                    float exponent = exp;
                    float[,] elevation = Noise.Calc2D(mapWidth, mapHeight, scale);
                    float[,] elevation2 = Noise.Calc2D(mapWidth, mapHeight, scale * 8);
                    float[,] elevation3 = Noise.Calc2D(mapWidth, mapHeight, scale * 16);

                    // add the noises together
                    // and do the powers
                    for (int i = 0; i < mapWidth; i++)
                        for (int j = 0; j < mapHeight; j++)
                        {
                            particleMap[i, j] = particleMap[i, j] / 255;
                            float noiseSum = (elevation[i, j] + elevation2[i, j] * 0.5f + elevation3[i, j] * 0.25f) / 3;
                            elevationSum[i, j] = (float)Math.Pow(noiseSum, exponent); //(float)Math.Round(e * 6) / 6; 
                        }
                }

                void GenerateMoistureMap()
                {

                    // Create the Simplex noise heightmap
                    // add different frequencies to make it interesting
                    // Noise.Seed = 2323666; // Optional                
                    float scale = 0.035f;
                    float exponent = 0.93f;
                    float[,] moisture = Noise.Calc2D(mapWidth, mapHeight, scale);
                    float[,] moisture2 = Noise.Calc2D(mapWidth, mapHeight, scale * 4);
                    float[,] moisture3 = Noise.Calc2D(mapWidth, mapHeight, scale * 8);

                    // add the noises together
                    // and do the powers
                    for (int i = 0; i < mapWidth; i++)
                        for (int j = 0; j < mapHeight; j++)
                        {
                            float e = (moisture[i, j] + moisture2[i, j] * 0.5f + moisture3[i, j] * 0.25f) / 3;
                            moistureSum[i, j] = (float)Math.Pow(e, exponent); //(float)Math.Round(e * 6) / 6; 
                        }
                }

                void GenerateRollingParticleMask(int _particles = 2000, int _life = 50)
                {
                    for (int particles = 0; particles < _particles; particles++)
                    {
                        Point start = new Point(Tools.RandomNumber.GetRandomInt(0, mapWidth - 1), Tools.RandomNumber.GetRandomInt(7, mapHeight - 2));//new Point(Dice.Roll("1d" + (mapWidth - 1).ToString()), Dice.Roll("1d" + (mapHeight-1).ToString()));
                        Point currentPos = start;

                        for (int life = 0; life < _life; life++)
                        {
                            Point newPos = new Point(1, 1);
                            while (particleMap[newPos.X, newPos.Y] <
                                    particleMap[currentPos.X, currentPos.Y] && map.CheckBounds(newPos))
                            {
                                int i = 0;
                                Point dir = Tools.GetRandomDir();

                                if (map.CheckBounds(currentPos + dir))
                                {
                                    newPos = currentPos + dir;
                                    currentPos = newPos;
                                }

                                if (i < 16)
                                    break;
                            }
                            if (map.CheckBounds(currentPos))
                                // this to make right side more higher, so river can flow from there.
                                if (currentPos.X > 60 && currentPos.Y > 4 && currentPos.Y < mapHeight - 20)
                                    particleMap[currentPos.X, currentPos.Y] += 0.4f;
                                else
                                    particleMap[currentPos.X, currentPos.Y] += 1.0f;
                        }
                    }
                }

                void GenerateRiver()
                {

                    for (double x = 0.0; x < mapWidth; x++)
                    {

                        double y = 0.35 * Math.Sin(2.0 * Math.PI * x / (mapHeight / 2));
                        y *= 10;
                        int yI = Convert.ToInt32(y);
                        particleMap[Convert.ToInt32(x), mapHeight / 2 + yI] = 100.0f;
                    }
                }

                void Biome(Point _pos, float elev, float moist)
                {
                    if (elev > 0.9f)
                        if (moist < 0.2f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileLand();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.Gray * (elev + 0.1f);
                        }
                        else if (moist < 0.4f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileLand();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DimGray * elev;
                        }
                        else if (moist < 0.7f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileGrass();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                        else
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileTrees();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                    else if (elev > 0.8f)
                        if (moist < 0.4f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileLand();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                        else if (moist < 0.6f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileGrass();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                        else
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileTrees();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                    else if (elev > 0.5f)
                        if (moist < 0.2f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileLand();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                        else if (moist < 0.6f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileGrass();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                        else
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileTrees();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                    else if (elev > 0.2f)
                        if (moist < 0.4f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileGrass();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * (elev);
                        }
                        else
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileTrees();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                    else if (elev > 0.0f)
                        if (moist > 0.6f)
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileSwamp();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkBlue * elev;
                        }
                        else
                        {
                            map.Tiles[_pos.ToIndex(mapWidth)] = new TileSwamp();
                            map.Tiles[_pos.ToIndex(mapWidth)].Background = Color.DarkOliveGreen * elev;
                        }
                    else if (elev < 0.1f)
                        map.Tiles[_pos.ToIndex(mapWidth)] = new TileRiver();
                }

                // TEHE: floodfill-algoritmi joka etsii metsät, esmes yli 8 puuta vierekkäin sijaiten


                void log(float _val, float _val2 = 0)
                {
                    Console.WriteLine("f: " + _val + "      f2: " + _val2);
                }

            }


            void CreateFOV(float[,] elevMap, Point location)
            {
                ArrayMap<bool> tempGoMap = new ArrayMap<bool>(mapWidth, mapHeight);

                //Console.WriteLine(" loc: " + _goMap[location]);
                foreach (Point pos in goMap.Positions())
                {
                    // Console.WriteLine("loc: "+ elevMap[location.X, location.Y] +  " pos:" + elevMap[pos.X, pos.Y]);
                    if (elevMap[pos.X, pos.Y] <= elevMap[location.X, location.Y])
                    {
                        tempGoMap[pos] = true;
                    }
                    else tempGoMap[pos] = false;
                    //else Console.WriteLine("false");
                }

                map.Tiles[location.ToIndex(mapWidth)].FovMap = new FOV(tempGoMap);

                //_map._tiles[location.ToIndex(mapWidth)].fovMap = Fov;
                //IMapView<double> senseMapView = new LambdaTranslationMap<bool, double>(_world.CurrentMap.goMap, val => val ? 0.0 : 1.0);
            }

            //            void GenerateLocalMaps() { }
            return Tuple.Create(map, goMap);
        }
    }
}
