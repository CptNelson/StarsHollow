using Microsoft.Xna.Framework;
using StarsHollow.Systems;
using StarsHollow.UserInterface;
using StarsHollow.World;

namespace StarsHollow
{
    class Game
    {
        private const int WindowWidth = 160;
        private const int WindowHeight = 45;

        public static UI UI;
        private static WorldMap World;
        private static MainLoop MainLoop;

        static void Main(string[] args)
        {

            // Setup the engine and create the main window.
            SadConsole.Game.Create(WindowWidth, WindowHeight);
            //SadConsole.Game.Create(Width, Height);
            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;
            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;
            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }
        private static void Update(GameTime time)
        {
        }
        private static void Init()
        {
            World = new WorldMap();
            MainLoop = new MainLoop();
            // pass the world to UI so the MainWindow can create and show world.
            UI = new UI(WindowWidth, WindowHeight, World, MainLoop);
        }
    }
}
