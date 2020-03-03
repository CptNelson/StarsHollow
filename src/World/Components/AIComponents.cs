using StarsHollow.Utils;
using StarsHollow.Engine;
using System;

namespace StarsHollow.World
{

    // TODO: Create a method for adding AIComponents.
    // Maybe these should be called modules to avoid confusion?

    public class AIComponent : Component
    {
        public AIComponent(Entity entity)
        {
            Entity = entity;
        }
        public virtual void ChooseNextAction()
        {

        }
    }

    public class GuardArea : AIComponent
    {
        public int StayInPlace { get; set; }
        public int Walk { get; set; }
        public int Attack { get; set; }

        public GuardArea(Entity entity, int stay = 20, int walk = 100, int attack = 0) : base(entity)
        {
            StayInPlace = stay;
            Walk = walk;
            Attack = attack;
        }
        // TODO: A system that increases and decreases these values.

        public override void ChooseNextAction()
        {
            switch (Tools.GetMaxVariable.GetMaxVariableInt(StayInPlace, Walk, Attack))
            {
                case "a":
                    Entity.GetComponent<CmpAction>().SetAction(new WaitAction(Entity));
                    Console.WriteLine("stay");
                    StayInPlace -= 5;
                    Walk += 5;
                    break;
                case "b":
                    Entity.GetComponent<CmpAction>().SetAction(new WaitAction(Entity));
                    Console.WriteLine("walk");
                    Walk -= 1;
                    StayInPlace += 1;
                    break;
                case "c":
                    Entity.GetComponent<CmpAction>().SetAction(new WaitAction(Entity));
                    Console.WriteLine("attack");
                    Attack -= 5;
                    break;
            }
        }
    }
}