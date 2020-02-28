using System;
using Microsoft.Xna.Framework;
using StarsHollow.World;

namespace StarsHollow.Engine
{

    /*============================================================\
    Actions are executed after it's timeCost has passed
    Eg. Entity chooses to do action at Time:120
    the action costs 100, so it will be executed at Time:220
    Entity's new Time will be 221.

    Actions will check if Entity who called the action is unable 
    to act when the action is executed, and cancel the action if so.
    
    Actions are Subjects, so they will raise Events
    Actions are IEntities so we can iterate them in the Gameloop
    
    /=============================================================*/

    public class Action : Subject, IEntity
    {
        public bool isActionable { get; set; }
        public Entity actionActor;

        public uint ID { get; set; }

        public uint timeCost { get; set; }

        public uint entityTime { get; set; }

        public Action()
        {
            isActionable = true;
        }

        public virtual bool Execute()
        {
            if (actionActor.GetComponent<CmpAction>().UnableToAct && !actionActor.GetComponent<CmpHP>().Alive)
                return false;
            return true;
        }

        public static bool SubscribeToSkills(Action action)
        {
            Game.UI.world.SystemSkills.Subscribe(action);
            action.NotifyObservers();
            return true;
        }

    }

    public class WaitAction : Action
    {
        public WaitAction()
        {
            timeCost = 100;
            ID = Map.IDGenerator.UseID();
        }
        //SubscribeToSkills(this);
    }

    public class TestAction : Action
    {
        public TestAction(Entity actor)
        {
            actionActor = actor;
            timeCost = 50;
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
        public Point targetPosition;

        public Shoot(Entity actor, Point pos)
        {
            timeCost = 100;
            actionActor = actor;
            targetPosition = pos;
        }

        public override bool Execute()
        {
            Console.WriteLine("shooting action");
            if (!base.Execute())
                return false;
            Game.UI.world.SystemSkills.Subscribe(this);
            NotifyObservers();
            return true;
        }
    }

    public class MeleeAttack : Action
    {
        public Point Dir;

        public MeleeAttack(Entity actor, Point dir)
        {
            timeCost = 100;
            actionActor = actor;
            Dir = dir;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            Game.UI.world.SystemSkills.Subscribe(this);
            NotifyObservers();
            return true;
        }
    }

    public class MoveBy : Action
    {
        public Point dir;

        public MoveBy(Entity actor, Point dir)
        {
            actionActor = actor;
            timeCost = 100 * Game.UI.world.OverworldMap.GetTileAt(actor.Position + dir).MoveCostMod;
            this.dir = dir;
            //System.Console.WriteLine("action!");
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            Game.UI.world.SystemMover.Subscribe(this);
            NotifyObservers();
            return true;
        }
    }
}