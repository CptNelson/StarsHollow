using System;
using System.Collections.Generic;
using System.Linq;
using StarsHollow.UserInterface;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    public class MainLoop
    {
        public bool Playing { get; set; }

        public List<IEntity> LevelEntityList { get; private set; }

        public List<IEntity> OverworldEntityList { get; private set; }

        public List<IEntity> EventsList { get; private set; }

        public MainLoop()
        {
        }

        public void Init(Map map)
        {
            LevelEntityList = new List<IEntity>();
            EventsList = new List<IEntity>();
            OverworldEntityList = new List<IEntity>();

            AddEntitiesToLevelList();

            void AddEntitiesToLevelList()
            {
                // Loop all entities in Map's entitylist, and add them to entityLists 
                // if they are flagged Actionable
                var entitiesInMap = map.Entities.Items;
                foreach (Entity ent in entitiesInMap)
                {
                    LevelEntityList.Add(ent);
                    if (ent.IsActionable)
                    {
                        EventsList.Add(ent);
                    }
                }
            }
        }


        // FIXME: foreach doesn't work when adding new components on the run.

        // TODO: maybe some kind of queue of added elements that adds them when this is not running?
        private void UpdateEntitiesNotWorking()
        {
            foreach (Entity ent in LevelEntityList)
            {
                var cmps = ent.GetComponents();
                foreach (Component cmp in cmps)
                {
                    cmp.UpdateComponent();
                }
            }
        }

        private void UpdateEntities()
        {
            for (int ent = 0; ent < LevelEntityList.Count; ent++)
            {
                Entity entity = (Entity)LevelEntityList[ent];
                List<IComponent> components = entity.GetComponents();

                for (int cmp = 0; cmp < components.Count; cmp++)
                {
                    // FIXME: lol
                    Component component = (Component)components[cmp];
                    component.UpdateComponent();
                }
            }
        }

        public IEnumerable<bool> Loop()
        {
            Playing = true;
            while (Playing)
            {
                EventsList.Sort((x, y) => x.EntityTime.CompareTo(y.EntityTime));

                IEntity currentEntity = EventsList.First();

                if (currentEntity is Animation)
                {
                    var animation = (Animation)currentEntity;
                    EventsList.Remove(animation);
                    animation.Execute();
                    Game.UI.MainWindow.GameState = States.Animation;
                    yield return true;
                }

                else if (currentEntity is Entity)
                {
                    var ent = (Entity)currentEntity;

                    // if the currentEvent is player, exit the loop and wait for input
                    // after input Gameloop is continued.
                    if (ent.HasComponent<CmpInput>())
                    {
                        onTurnChange(States.Input);
                        yield return true;
                    }

                    if (!currentEntity.IsActionable)
                    {
                        // this is to pass time on non-actionable entities.
                        currentEntity.EntityTime += 100;
                    }
                    else
                    {
                        // if currentEvent is timer, advance the turn.
                        if (ent.HasComponent<CmpTimer>())
                        {
                            UpdateEntities();
                        }

                        if (ent.HasComponent<CmpAI>())
                        {
                            ent.GetComponent<CmpAI>().GetGoal();
                        }
                    }
                }
                // if current event is Action
                // execute the action and remove it from the loop.
                else if (currentEntity is Action)
                {
                    var action = (Action)currentEntity;
                    action.Execute();
                    EventsList.Remove(action);
                }
            }
        }

        public delegate void OnTurnChange(States state);

        public static event OnTurnChange onTurnChange;
    }
}