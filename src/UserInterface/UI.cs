using GoRogue;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using StarsHollow.Utils;
using StarsHollow.World;
using System;
using System.Collections;
using System.Linq;
using StarsHollow.Engine;
using Console = System.Console;

namespace StarsHollow.UserInterface
{
    /* 
    UIManager takes care of representing all things graphical and input. It's children
    are the MainWindow, and the consoles inside it: Map, Log, Status.
    Also includes color settings, themes, fonts and so on.
    */
    class UI
    {
        public readonly WorldMap world;
        public MainWindow MainWindow;
        private readonly int width;
        private readonly int height;
        private Map currentMap;
        private MessageLogWindow messageLogWindow;


        public UI(int screenWidth, int screenHeight, WorldMap world, MainLoop mainLoop)
        {
            width = screenWidth;
            height = screenHeight;
            SetupLook();
            MainWindow = new MainWindow(width, height, world, mainLoop, messageLogWindow);
            this.world = world;
        }

        public void AddMessage(string message)
        {
            MainWindow.Message(message);
        }

        private void SetupLook()
        {
            SadConsole.Themes.WindowTheme windowTheme = new SadConsole.Themes.WindowTheme();
            windowTheme.BorderLineStyle = CellSurface.ConnectedLineThick;
            SadConsole.Themes.Library.Default.WindowTheme = windowTheme;
            SadConsole.Themes.Library.Default.Colors.TitleText = ColorScheme.Three;
            SadConsole.Themes.Library.Default.Colors.Lines = ColorScheme.Three;
            SadConsole.Themes.Library.Default.Colors.ControlHostBack = ColorScheme.First;
        }
    }

    // TODO: break this down to multiple classes, or make it a partial class in multiple files

    // MainWindow Creates and holds every other window inside it.
    internal class MainWindow : ContainerConsole
    {
        public readonly MainLoop MainLoop;
        public States GameState { get; set; }
        private readonly WorldMap world;
        private IEnumerator iterator;
        private int inputState = 0;
        private SadConsole.Entities.Entity target;
        private Window menuWindow;
        private Window mapWindow;
        private ScrollingConsole menuConsole;
        private ScrollingConsole mapConsole;
        private ScrollingConsole TargetConsole;
        private MessageLogWindow messageLogWindow;
        private StatusWindow statusConsole;
        private FOV fov;

        public void Message(string message)
        {
            messageLogWindow.Add(message);
        }

        public MainWindow(int width, int height, WorldMap world, MainLoop mainLoop, MessageLogWindow messageLogWindow)
        {
            GameState = States.StartMenu;
            this.world = world;
            this.messageLogWindow = messageLogWindow;

            IsVisible = true;
            IsFocused = true;

            Parent = Global.CurrentScreen;
            CreateWindowsAndConsoles(width, height);
            menuWindow.Show();

            MainLoop = mainLoop;
            MainLoop.onTurnChange += ChangeState;

            // FIXME: Should this be called somewhere else?
            StartGame();
        }
        /* =========================================================
            When Update is run, it checks what is the GameState,
            and chooses the right input system if it's player's turn,
            if not, it iterates over the EventList. 
           ========================================================= */
        public override void Update(TimeSpan timeElapsed)
        {
            switch (GameState)
            {
                case States.StartMenu:
                    StartMenuKeyboard();
                    break;
                case States.Input:
                    WorldMapKeyboard();
                    break;
                case States.Animation:
                    base.Update(timeElapsed);
                    //DisplayFOV();
                    break;
                case States.Main:
                    {
                        if (iterator == null)
                        {
                            iterator = MainLoop.Loop().GetEnumerator();
                        }

                        iterator.MoveNext();
                        break;
                    }
            }

            CheckMouse();
            base.Update(timeElapsed);
        }

        // ============INIT==========================================
        private void CreateWindowsAndConsoles(int width, int height)
        {
            // calculating sizes of the child windows and consoles
            double tempWidth = width / 1.5 / 1.618;
            double tempHeight = height * 1.5 / 1.618;
            // but hardcoded values used here for now.
            int mapWidth = 72; //Convert.ToInt32(_tempWidth);
            int mapHeight = 40; //Convert.ToInt32(_tempHeight);

            // Consoles
            menuConsole = new ScrollingConsole(width, height, Fonts.quarterSizeFont);
            mapConsole = new ScrollingConsole(mapWidth, mapHeight, Fonts.halfSizeFont);
            TargetConsole = new ScrollingConsole(mapWidth, mapHeight, Fonts.halfSizeFont);

            // Windows
            CreateMenuWindow();
            CreateMapWindow(mapWidth, mapHeight, "*Stars Hollow*");
            CreateMessageLogWindow();
            CreateStatusWindow(world);


            // ===Creators for windows===

            void CreateMenuWindow()
            {
                menuWindow = new Window(width, height);
                // load image from REXpaint file.
                ScrollingConsole rexConsole;

                using (var rexStream = System.IO.File.OpenRead(@"./res/xp/metsa.xp"))
                {
                    var rex = SadConsole.Readers.REXPaintImage.Load(rexStream);
                    rexConsole = rex.ToLayeredConsole();
                }

                rexConsole.Position = new Point(0, 0);
                rexConsole.Font = Fonts.quarterSizeFont;

                menuWindow.Children.Add(rexConsole);
                Children.Add(menuWindow);
            }

            void CreateMapWindow(int mapWidth, int mapHeight, string title)
            {
                mapWindow = new Window(mapWidth, mapHeight);
                mapConsole = new ScrollingConsole(mapWindow.Width, mapWindow.Height, Fonts.halfSizeFont,
                    new Microsoft.Xna.Framework.Rectangle(0, 0, Width, Height));
                TargetConsole = new ScrollingConsole(mapWindow.Width, mapWindow.Height, Fonts.halfSizeFont);

                //make console short enough to show the window title
                //and borders, and position it away from borders
                int mapConsoleWidth = mapWidth - 2;
                int mapConsoleHeight = mapHeight - 2;

                // Resize the Map Console's ViewPort to fit inside of the window's borders
                mapConsole.ViewPort = new Microsoft.Xna.Framework.Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);
                TargetConsole.ViewPort = new Microsoft.Xna.Framework.Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);

                //reposition the MapConsole so it doesnt overlap with the left/top window edges
                mapConsole.Position = new Point(1, 1);
                TargetConsole.Position = new Point(1, 1);

                // Centre the title text at the top of the window
                mapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth, (char)205);

                //add the map viewer to the window
                mapWindow.Children.Add(mapConsole);
                mapWindow.Children.Add(TargetConsole);

                // The MapWindow becomes a child console of the MainWindow
                Children.Add(mapWindow);

                mapWindow.Font = Fonts.halfSizeFont;
                mapConsole.Font = Fonts.halfSizeFont;
            }

            void CreateMessageLogWindow()
            {
                messageLogWindow = new MessageLogWindow(mapWidth, height - mapHeight + 15, "*LOG*")
                {
                    Font = Fonts.halfSizeFont
                };
                Children.Add(messageLogWindow);
                messageLogWindow.Position = new Point(0, mapHeight);
                messageLogWindow.Show();
            }

            void CreateStatusWindow(WorldMap world)
            {
                statusConsole = new StatusWindow(width - mapWidth * 2 + 5, height + 15, "*Status*", world);
                statusConsole.Font = Fonts.halfSizeFont;
                Children.Add(statusConsole);

                statusConsole.Show();
                statusConsole.Position = new Point(mapWidth, 0);
            }
        }


        // ============WINDOW & CONSOLE MANAGEMENT==========================================

        public void DisplayFOV()
        {
            world.CurrentMap.Tiles[world.Player.Position.ToIndex(world.CurrentMap.Width)].FovMap
                .Calculate(world.Player.Position, 55, Radius.SQUARE);
            foreach (Point pos in world.CurrentMap.GoMap.Positions())
            {
                if (world.CurrentMap.Tiles[pos.ToIndex(world.CurrentMap.Width)].IsExplored)
                {
                    world.CurrentMap.Tiles[pos.ToIndex(world.CurrentMap.Width)].Foreground.A = 220;
                }
            }

            // set all currently visible tiles to their normal color
            // and entities Visible
            foreach (var pos in world.CurrentMap.Tiles[world.Player.Position.ToIndex(world.CurrentMap.Width)]
                .FovMap.CurrentFOV)
            {
                if (!world.CurrentMap.Tiles[pos.ToIndex(world.CurrentMap.Width)].IsExplored)
                {
                    world.CurrentMap.Tiles[pos.ToIndex(world.CurrentMap.Width)].IsExplored = true;
                    world.CurrentMap.Tiles[pos.ToIndex(world.CurrentMap.Width)].IsVisible = true;
                }

                // System.Console.WriteLine(pos + "   p: " + _world.player.Position);
                world.CurrentMap.Tiles[pos.ToIndex(world.CurrentMap.Width)].Foreground.A = 255;

                if (world.CurrentMap.Entities.Contains(pos))
                    world.CurrentMap.GetFirstEntityAt<Entity>(pos).Animation.IsVisible = true;
            }

            mapConsole.IsDirty = true;
        }

        private void ChangeState(States state)
        {
            GameState = state;
        }

        private void StartGame()
        {
            menuWindow.Hide();
            mapWindow.Show();

            world.CreateWorld(mapWindow.Width, mapWindow.Height);
            world.CurrentMap = world.LocalMap;
            LoadMapToConsole(world.LocalMap);
            Console.WriteLine(world.TurnTimer);
            statusConsole.WriteInformation();
            GameState = States.Main;
            DisplayFOV();
            CreateTarget();
            MainLoop.Init(world.LocalMap);
            MainLoop.Loop();
            //SyncMapEntities(_world.OverworldMap);
        }

        private void LoadMapToConsole(Map map)
        {
            // Now Sync all of the map's entities
            mapConsole.SetSurface(map.Tiles, mapWindow.Width, mapWindow.Height);
            SyncMapEntities(map);
        }

        private void CreateTarget()
        {
            target = new SadConsole.Entities.Entity(1, 1);
            target.Animation.CurrentFrame[0].Glyph = 7;
            target.Animation.CurrentFrame[0].Foreground = Color.Red;
            target.IsVisible = false;
            target.Font = Fonts.halfSizeFont;
            TargetConsole.Children.Add(target);
        }

        private void SyncMapEntities(Map map)
        {
            // remove all Entities from the console first
            mapConsole.Children.Clear();
            // Now pull all of the entity sprites into the MapConsole in bulk
            foreach (Entity entity in map.Entities.Items)
            {
                mapConsole.Children.Add(entity);
            }

            // Subscribe to the Entities ItemAdded listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemAdded += OnMapEntityAdded;

            // Subscribe to the Entities ItemRemoved listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemRemoved += OnMapEntityRemoved;
        }

        // Remove an Entity from the MapConsole every time the Map's Entity collection changes 
        private void OnMapEntityRemoved(object sender, ItemEventArgs<Entity> args)
        {
            mapConsole.Children.Remove(args.Item);
        }

        // Add an Entity to the MapConsole every time the Map's Entity collection changes
        private void OnMapEntityAdded(object sender, ItemEventArgs<Entity> args)
        {
            mapConsole.Children.Add(args.Item);
        }

        // ============INPUT===================================

        private void StartMenuKeyboard()
        {
            if (Global.KeyboardState.IsKeyPressed(Keys.Enter))
            {
                StartGame();
            }
        }

        private void WorldMapKeyboard()
        {
            if (Global.KeyboardState.IsKeyReleased(Keys.Enter))
            {
                GameState = States.Main;
            }

            if (Keyboard.GetState().GetPressedKeys().Length > 0)
            {
                if (inputState == 0)
                {
                    if (Global.KeyboardState.IsKeyPressed(Keys.L))
                    {
                        inputState = 1;
                        return;
                    }

                    if (Global.KeyboardState.IsKeyPressed(Keys.S))
                    {
                        inputState = 2;
                        return;
                    }

                    if (Global.KeyboardState.IsKeyPressed(Keys.Q))
                    {
                        SaveAndQuit();
                    }

                    if (Global.KeyboardState.IsKeyPressed(Keys.Up))
                        Command.Move(world.Player, Tools.Dirs.N);
                    if (Global.KeyboardState.IsKeyPressed(Keys.Down))
                        Command.Move(world.Player, Tools.Dirs.S);
                    if (Global.KeyboardState.IsKeyPressed(Keys.Right))
                        Command.Move(world.Player, Tools.Dirs.E);
                    if (Global.KeyboardState.IsKeyPressed(Keys.Left))
                        Command.Move(world.Player, Tools.Dirs.W);

                    GameState = States.Main;
                    DisplayFOV();

                }

                //  _gameState = States.Main;

                // inputState 1: looking, inputState 2: shooting
                if (inputState == 1 || inputState == 2)
                {
                    if (!target.IsVisible)
                        target.Position = world.Player.Position;
                    target.IsVisible = true;
                    if (Global.KeyboardState.IsKeyPressed(Keys.Escape))
                    {
                        Console.WriteLine("exit");
                        ExitTargetting();
                    }


                    if (Global.KeyboardState.IsKeyPressed(Keys.Up))
                        MoveTarget(new Point(0, -1));
                    if (Global.KeyboardState.IsKeyPressed(Keys.Down))
                        MoveTarget(new Point(0, 1));
                    if (Global.KeyboardState.IsKeyPressed(Keys.Left))
                        MoveTarget(new Point(-1, 0));
                    if (Global.KeyboardState.IsKeyPressed(Keys.Right))
                        MoveTarget(new Point(1, 0));
                }

                if (inputState == 2)
                {
                    if (Global.KeyboardState.IsKeyPressed(Keys.S))
                    {
                        TargetConsole.Clear();
                        Game.UI.world.Player.GetComponent<CmpAction>()
                            .SetAction(new Shoot(Game.UI.world.Player, target.Position));
                        ExitTargetting(true);
                    }
                }


                void ExitTargetting(bool endTurn = true)
                {
                    target.IsVisible = false;
                    TargetConsole.Clear();
                    inputState = 0;
                    if (endTurn)
                        GameState = States.Main;
                }

                void MoveTarget(Point dir)
                {
                    target.Position += dir;
                    if (Game.UI.world.CurrentMap.IsThereEntityAt(target.Position))
                        Message(Game.UI.world.CurrentMap.GetFirstEntityAt<Entity>(target.Position).Name);
                    if (inputState == 2)
                    {
                        TargetConsole.Clear();
                        var line = Lines.Get(Game.UI.world.Player.Position, target.Position).ToList();
                        line.RemoveAt(0);
                        line.RemoveAt(line.Count - 1);
                        foreach (Point pos in line)
                        {
                            TargetConsole.Print(pos.X, pos.Y, ".", Color.Red);
                        }
                    }
                }
            }
        }

        private void CheckMouse()
        {
            if (Global.MouseState.LeftClicked)
                System.Console.WriteLine(world.LocalMap
                    .GetTileAt(Global.MouseState.ScreenPosition.PixelLocationToConsole(mapWindow.Width,
                        mapWindow.Height)).Name);
        }

        private void SaveAndQuit()
        {
            world.SaveCurrentGame();
            Console.WriteLine("saved");
        }
    }

    //=================HELPERS=================================================================
    public static class Fonts
    {
        public static FontMaster font1 = Global.LoadFont(@"../../res/fonts/bisasam.font");
        public static FontMaster font2 = Global.LoadFont(@"../../res/fonts/lord.font");
        public static FontMaster font3 = Global.LoadFont(@"../../res/fonts/Anikki-square.font");
        public static Font halfSizeFont = font1.GetFont(Font.FontSizes.Half);
        public static Font normalSizeFont = font2.GetFont(Font.FontSizes.One);
        public static Font quarterSizeFont = font3.GetFont(Font.FontSizes.Quarter);
        public static Font squareHalfFont = font3.GetFont(Font.FontSizes.Half);
        public static Font normalSizeAnikkiFont = font3.GetFont(Font.FontSizes.One);
    }

    public static class ColorScheme
    {
        public static Color First = Color.Black;
        public static Color Second = new Color(138, 247, 228);
        public static Color Three = new Color(157, 114, 255);
        public static Color Four = new Color(255, 179, 253);
        public static Color Five = new Color(1, 255, 195);
    }


    public enum States
    {
        StartMenu,
        Input,
        Main,
        Animation
    }
}