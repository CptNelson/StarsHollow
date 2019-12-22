using System;
using System.Collections.Generic;
using System.Linq;
using StarsHollow.UserInterface;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    public class MainLoop
    {
        private List<IEntity> _levelEntityList;
        private List<IEntity> _overworldEntityList;
        private List<IEntity> _eventsList;

        public bool playing;

        public List<IEntity> LevelEntityList
        {
            get => _levelEntityList;
            set => _levelEntityList = value;
        }

        public List<IEntity> OverworldEntityList
        {
            get => _overworldEntityList;
            set => _overworldEntityList = value;
        }

        public List<IEntity> EventsList
        {
            get => _eventsList;
            set => _eventsList = value;
        }

        public MainLoop()
        {
        }

        public void Init(Map map)
        {
            // Game.UI.CenterOnActor(Game.World.player);
            _levelEntityList = new List<IEntity>();
            _eventsList = new List<IEntity>();
            _overworldEntityList = new List<IEntity>();
            AddEntitiesToLevelList();

            void AddEntitiesToLevelList()
            {
                // Loop all entities in Map's entitylist, and add them to entityLists 
                // if they are flagged Actionable
                var entitiesInMap = map.Entities.Items;
                foreach (Entity ent in entitiesInMap)
                {
                    Console.WriteLine("1");
                    _levelEntityList.Add(ent);
                    if (ent.Actionable)
                    {
                        Console.WriteLine("2");
                          Console.WriteLine(ent.Name);
                        _eventsList.Add(ent);
                    }
                }
            }
        }

        private void UpdateEntities()
        {
            foreach (Entity ent in _levelEntityList)
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
                //_levelEntityList.Sort((x, y) => x.Time.CompareTo(y.Time));

                _eventsList.Sort((x, y) => x.Time.CompareTo(y.Time));


                IEntity currentEntity = _eventsList.First();

                foreach (IEntity ent in _eventsList)
                {
                    Console.WriteLine("entity: " + ent);
                }
                
                
                Console.WriteLine(currentEntity.Actionable);
                
                if (currentEntity is Animation)
                {
                    Console.WriteLine("anim");
                    var animation = (Animation) currentEntity;
                    _eventsList.Remove(animation);
                    animation.Execute();
                    Game.UI.MainWindow.GameState = States.Animation;
                    yield return true;
                }

                else if (currentEntity is Entity)
                {
                    var ent = (Entity) currentEntity;

                /*foreach (var component in ent.GetComponents())
                {
                    var comp = (Component) component;
                    Console.WriteLine(comp.Name);
                }*/
                
                    // if the currentEvent is player, exit the loop and wait for input
                    // after input Gameloop is continued.
                    // player's turn
                    if (ent.HasComponent<CmpInput>())
                    {
                       // System.Console.WriteLine("Player turn");
                        onTurnChange(States.Input);
                        //  Game.UI.gameState = States.player;
                        yield return true;
                    }

                    if (!currentEntity.Actionable)
                    {
                        // this is to pass time on non-action entities. Change it later
                        currentEntity.Time += 100;
                    }
                    else
                    {
                        // if currentEvent is timer, advance the turn.
                        if (ent.HasComponent<CmpTimer>())
                        {
                            //     System.Console.WriteLine("Timer turn");
                            //currentEntity.GetComponent<CmpTimer>().UpdateComponent();
                            UpdateEntities();
                        }

                        if (ent.HasComponent<CmpAI>())
                        {
                              System.Console.WriteLine("AI turn");
                              ent.GetComponent<CmpAI>().GetGoal();
                       //       Console.WriteLine(ent.Time);
                           //   ent.GetComponent<CmpAction>().NextAction
                            // currentEntity.GetComponent<CmpAction>().
                            // currentEntity.GetComponent<CmpAI>().GetGoal();
                        }
                    }
                }
                // if current event is Action
                // execute the action and remove it from the loop.
                else if (currentEntity is Action)
                {

                    var action = (Action)currentEntity;
                    //System.Console.WriteLine("event: " + action);

                    action.Execute();
                    EventsList.Remove(action);
                }
            }
        }

        public delegate void OnTurnChange(States state);

        public static event OnTurnChange onTurnChange;
    }
}