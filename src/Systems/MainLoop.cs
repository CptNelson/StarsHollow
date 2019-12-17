using StarsHollow.Components;
using StarsHollow.UserInterface;
using StarsHollow.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarsHollow.Systems
{
    public class MainLoop
    {
        private List<Entity> levelEntityList;
        private List<Entity> overworldEntityList;
        public bool playing;

        public List<Entity> LevelEntityList { get => levelEntityList; set => levelEntityList = value; }
        public List<Entity> OverworldEntityList { get => overworldEntityList; set => overworldEntityList = value; }

        public MainLoop()
        {

        }

        public void Init(Map map)
        {
            // Game.UI.CenterOnActor(Game.World.player);
            levelEntityList = new List<Entity>();
            overworldEntityList = new List<Entity>();
            AddEntitiesToLevelList();

            void AddEntitiesToLevelList()
            {   // Loop all entities in Map's entitylist, and add them to entityLists 
                // if they are flagged Actionable
               // Console.WriteLine(map);
                var entitiesInMap = map.Entities.Items;
                foreach (Entity ent in entitiesInMap)
                {
              //      Console.WriteLine(ent.Name);
                    levelEntityList.Add(ent);
                }
                //Game.UI.gameState = States.main;
            }
        }
        private void UpdateEntities()
        {
            foreach (Entity ent in levelEntityList)
            {
                var cmps = ent.GetComponents();
                foreach (Component cmp in cmps)
                {
                    cmp.UpdateComponent();
                }
            }
        }

        public IEnumerable<bool> Loop()
        {
            playing = true;
            while (playing)
            {

                levelEntityList.Sort((x, y) => x.Time.CompareTo(y.Time));

                Entity currentEntity = levelEntityList.First();


                // if the currentEvent is player, exit the loop and wait for input
                // after input Gameloop is continued.

                foreach (Component comp in currentEntity.GetComponents())
                {
                }
                

           //     System.Console.WriteLine("entity : " + currentEntity.Name);
                // this is to pass time on non-action entities. Change it later
                if (!currentEntity.Actionable)
                    currentEntity.Time += 100;
                
                // player's turn
                if (currentEntity.HasComponent<CmpInput>())
                {
                //    System.Console.WriteLine("Player turn");
                    onTurnChange(States.Input);
                    //  Game.UI.gameState = States.player;
                    yield return true;
                }
                else
                {  
                    // if currentEvent is timer, advance the turn.
                    if (currentEntity.HasComponent<CmpTimer>())
                    {
                  //     System.Console.WriteLine("Timer turn");
                        //currentEntity.GetComponent<CmpTimer>().UpdateComponent();
                        UpdateEntities();
                    }
                    if (currentEntity.HasComponent<CmpAI>())
                    {
                     //  System.Console.WriteLine("AI turn");
                       // currentEntity.GetComponent<CmpAction>().
                       // currentEntity.GetComponent<CmpAI>().GetGoal();
                    }
                }
                
            }
        }
        public delegate void OnTurnChange(States state);
        public static event OnTurnChange onTurnChange;
    }
}
