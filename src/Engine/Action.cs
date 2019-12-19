using System;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.Engine
{
    // Actions are executed after it's Time cost has passed
    // Eg. Entity chooses to do action at Time:120
    // the action costs 100, so it will be executed at Time:220
    // Entity's new Time will be 221.

    public interface IAction
    {
    }

    // Action are Subjects, so they will raise Events
    public class Subject
    {
        public event EventHandler EventHandler; //We can also consider
        //using an auto-implemented property instead of a public field

        public void NotifyObservers()
        {
            if (EventHandler != null) //Ensures that if there are no handlers,
                //the event won't be raised
            {
                EventHandler(this, EventArgs.Empty); //We can also replace
                //EventArgs.Empty with our own message
            }
        }
    }

    // Actions are IEntities so we can iterate them in the Gameloop
    // TODO: a cleaner way to this.
    public class Action : Subject, IAction, IEntity
    {
        public bool Actionable { get; set; }
        public Entity Actor;

        public uint ID { get; set; }

        // Cost is how many Time units action will take
        public uint Cost { get; set; }

        public uint Time { get; set; }

        // If entity is UnableToAct, stop executing the action.
        public virtual bool Execute()
        {
            if (Actor.GetComponent<CmpAction>().UnableToAct && !Actor.GetComponent<CmpHP>().Alive)
                return false;
            return true;
        }

        public Action()
        {
            Actionable = true;
        }
    }

    public class WaitAction : Action
    {
        public WaitAction()
        {
            Cost = 100;
            ID = Map.IDGenerator.UseID();
        }
    }

    public class TestAction : Action
    {
        public TestAction(Entity actor)
        {
            Actor = actor;

            Cost = 50;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            Console.WriteLine("test Action");
            return true;
        }
    }

    public class Shoot : Action
    {
        private Point _position;

        public Shoot(Entity actor, Point pos)
        {
            Cost = 100;
            Actor = actor;
            _position = pos;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            //Game.World.systemSkills.Subscribe(this);
            NotifyObservers();
            return true;
        }
    }

    public class MeleeAttack : Action
    {
        private Point _dir;

        public MeleeAttack(Entity actor, Point dir)
        {
            Cost = 100;
            Actor = actor;
            _dir = dir;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            //Game.World.systemSkills.Subscribe(this);
            NotifyObservers();
            return true;
        }
    }

    public class DoDamage : Action
    {
        private int _damage;

        public DoDamage(Entity actor, int damage)
        {
            Cost = 1;
            Actor = actor;
            _damage = damage;
        }

        public override bool Execute()
        {
            //Game.World.systemDamage.Subscribe(this);
            //NotifyObservers();
            //  DamageEvent damageEvent = new DamageEvent(_actor, _damage);
            //  Game.World.systemDamage.Subscribe(damageEvent);
            //  damageEvent.NotifyObservers();
            return true;
        }
    }

    public class Drink : Action
    {
        private Entity _container;

        public Drink(Entity actor, Entity container = null)
        {
            Actor = actor;
            _container = container;
            Cost = 50;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;

            /*    if (_container.GetComponent<CmpLiquid>().CurrentAmount > 0)
                {
                    _container.GetComponent<CmpLiquid>().CurrentAmount--;
                    _actor.GetComponent<CmpBody>().ReduceThirst();
                }
                else
                {
                    Game.UI.AddMessage("It is empty.");
                } */
            return true;
        }
    }

    public class GatherFood : Action
    {
        public GatherFood(Entity actor)
        {
            Actor = actor;
            Cost = 1000;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;

            switch (GoRogue.DiceNotation.Dice.Roll("1d6"))
            {
                case 1:
                case 2:
                case 3:
                    Game.UI.AddMessage("You don't find anything to eat.");
                    break;
                case 4:
                    Game.UI.AddMessage("You gather some berries and eat them.");
                    //    _actor.GetComponent<CmpBody>().ReduceHunger(50.0m);
                    break;
                case 5:
                    Game.UI.AddMessage("You find wild vegetables to eat.");
                    //  _actor.GetComponent<CmpBody>().ReduceHunger(100.0m);
                    break;
                case 6:
                    Game.UI.AddMessage("You manage to catch a rabbit.");
                    //   _actor.GetComponent<CmpBody>().ReduceHunger(200.0m);
                    break;
            }
            //   Tools.StatusWindowUpdate(_actor);

            return true;
        }
    }

    public class MoveBy : Action
    {
        public Point _dir;

        public MoveBy(Entity actor, Point dir)
        {
            Actor = actor;
            Cost = 100 * Game.UI.world.OverworldMap.GetTileAt(actor.Position + dir).MoveCostMod;
            _dir = dir;
            //System.Console.WriteLine("action!");
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            //Game.UI._world.systemMover.Subscribe(this);
            //  NotifyObservers();
            Actor.Position += _dir;
            Actor.Time += Cost;

            return true;
        }
    }
}