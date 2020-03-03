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
    
    Actions are Subjects, so they will raise Events.
    When action is executed, it will subscribe to correct System
    and then notify it.
    Actions are IEntities so we can iterate them in the Gameloop
    
    /=============================================================*/

    public class Action : Subject, IEntity
    {
        public bool IsActionable { get; set; }
        public uint EntityTime { get; set; }
        public uint ID { get; protected set; }
        public Entity ActionActor { get; protected set; }

        public uint TimeCost { get; protected set; }


        public Action(Entity actor)
        {
            ActionActor = actor;
            IsActionable = true;
        }

        public virtual bool Execute()
        {
            // base always checks if Actor/Entity is unable to act when action is trying to execute.
            if (ActionActor.GetComponent<CmpAction>().UnableToAct && !ActionActor.GetComponent<CmpHP>().Alive)
                return false;
            return true;
        }

        public bool SubscribeToSkills(Action action)
        {
            Game.UI.world.SystemSkills.Subscribe(action);
            action.NotifyObservers();
            return true;
        }

        public bool SubscribeToMover(Action action)
        {
            Game.UI.world.SystemMover.Subscribe(action);
            action.NotifyObservers();
            return true;
        }
    }

    public class WaitAction : Action
    {
        public WaitAction(Entity actor) : base(actor)
        {
            TimeCost = 100;
        }
    }

    public class Crouch : Action
    {
        public Crouch(Entity actor) : base(actor)
        {
            TimeCost = 20;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;

            ActionActor.IsCrouching = !ActionActor.IsCrouching;

            if (ActionActor.IsCrouching)
                ActionActor.MoveCostMod += 25;
            else
                ActionActor.MoveCostMod -= 25;

            return true;
        }
    }

    public class Shoot : Action
    {
        public Point targetPosition;

        public Shoot(Entity actor, Point pos) : base(actor)
        {
            TimeCost = 100;
            ActionActor = actor;
            targetPosition = pos;
        }

        public override bool Execute()
        {
            Console.WriteLine("shooting action");
            if (!base.Execute())
                return false;
            return SubscribeToSkills(this);
        }
    }

    public class MeleeAttack : Action
    {
        public Point Dir;

        public MeleeAttack(Entity actor, Point dir) : base(actor)
        {
            TimeCost = 100;
            ActionActor = actor;
            Dir = dir;
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            return SubscribeToSkills(this);
        }
    }

    public class MoveBy : Action
    {
        public Point position;

        public MoveBy(Entity actor, Point dir) : base(actor)
        {
            ActionActor = actor;
            TimeCost = 100 * Game.UI.world.LocalMap.GetTileAt(actor.Sprite.Position + dir).MoveCostMod + actor.MoveCostMod;
            position = actor.Sprite.Position + dir;
            Console.WriteLine(actor.MoveCostMod);
        }

        public override bool Execute()
        {
            if (!base.Execute())
                return false;
            return SubscribeToMover(this);
        }
    }
}