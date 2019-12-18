﻿using System;
using System.Collections.Generic;
using System.Linq;
using StarsHollow.Components;
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
                // Console.WriteLine(map);
                var entitiesInMap = map.Entities.Items;
                foreach (Entity ent in entitiesInMap)
                {
                    _levelEntityList.Add(ent);
                    if (ent.Actionable)
                    {
                          Console.WriteLine(ent.Name);
                        _eventsList.Add(ent);
                    }
                }

                //Game.UI.gameState = States.main;
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


                // if the currentEvent is player, exit the loop and wait for input
                // after input Gameloop is continued.
                
                
                if (currentEntity is Animation)
                {
                    //System.Console.WriteLine("anim");
                    var animation = (Animation) currentEntity;
                    animation.Execute();
                    Game.UI.MainWindow.GameState = States.Animation;
                    LevelEntityList.Remove(animation);
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
                    // player's turn
                    if (ent.HasComponent<CmpInput>())
                    {
                        System.Console.WriteLine("Player turn");
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
                            // currentEntity.GetComponent<CmpAction>().
                            // currentEntity.GetComponent<CmpAI>().GetGoal();
                        }
                    }
                }
            }
        }

        public delegate void OnTurnChange(States state);

        public static event OnTurnChange onTurnChange;
    }
}