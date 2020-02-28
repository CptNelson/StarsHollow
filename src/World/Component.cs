using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;
using StarsHollow.Engine;
using Action = StarsHollow.Engine.Action;

namespace StarsHollow.World
{
    public interface IComponent
    {
        Entity Entity { get; set; }
        // string Name { get; set; }
    }

    public abstract class Component : IComponent
    {
        public List<IComponent> Components = new List<IComponent>();
        public Entity Entity { get; set; }

        protected string Name { get; set; }
        //Components are updated every turn. Override this if update is needed.
        public abstract void UpdateComponent();

        public void GetComponentByName()
        {

        }
    }
    public class CmpTimer : Component
    {
        private int _turn = 0;

        private int _minute = 0;
        private int _hour = 6;
        private int _day = 1;

        public int Minute => _minute;
        public int Hour => _hour;
        public int Day => _day;

        public int Turn { get => _turn; }
        public CmpTimer()
        {
            Name = "timer";
        }
        public override void UpdateComponent()
        {
            _turn += 1;

            //System.Console.WriteLine("turn: " + _turn.ToString());
            Entity.EntityTime += 100;

            if (Entity.EntityTime % 100 == 0)
                _minute++;
            if (_minute > 59)
            {
                _hour++;
                _minute = 0;
            }

            if (_hour > 23)
            {
                _hour = 0;
                _day++;
            }
            //Tools.StatusWindowUpdate(Game.World.player);
        }
    }
    public class CmpHP : Component
    {
        private int _hp = 10;
        private int _currentHp = 10;
        private bool _alive = true;

        public CmpHP(params object[] args)
        {
            _hp = Convert.ToInt32(args[0]);// + ; 
        }
        public int Hp { get => _hp; set => _hp = value; }
        public int CurrentHp { get => _currentHp; set => _currentHp = value; }
        public bool Alive { get => _alive; set => _alive = value; }

        public override void UpdateComponent()
        {
        }
    }

    public class CmpAttributes : Component
    {

        private int _strength;
        private int _agility;
        private int _vitality;

        private int _looks;
        private int _guts;
        private int _smarts;

        public int Strength => _strength;
        public int Agility => _agility;
        public int Vitality => _vitality;
        public int Looks => _looks;
        public int Guts => _guts;
        public int Smarts => _smarts;


        public CmpAttributes(params object[] args)
        {
            _strength = System.Convert.ToInt32(args[0]);
            _agility = System.Convert.ToInt32(args[1]);
            _vitality = System.Convert.ToInt32(args[2]);

            _looks = System.Convert.ToInt32(args[3]);
            _guts = System.Convert.ToInt32(args[4]);
            _smarts = System.Convert.ToInt32(args[5]);
        }

        public override void UpdateComponent()
        {
        }
    }

    public class CmpBody : Component
    {
        public List<Entity> ItemList;
        public readonly int ItemCapacity = 3;
        public Entity Holding;

        public List<Entity> RightHand;
        public List<Entity> LeftHand;

        public CmpBody(params object[] args)
        {
            ItemCapacity = System.Convert.ToInt32(args[0]);
            ItemList = new List<Entity>();
            RightHand = new List<Entity>(1);
            LeftHand = new List<Entity>(1);
        }

        public Entity GetItemAtRightHand()
        {
            return RightHand.First();
        }
        public Entity GetItemAtLeftHand()
        {
            return LeftHand.First();
        }

        public bool IsHoldingItem()
        {
            return RightHand.Count > 0 || LeftHand.Count > 0;
        }

        public Entity GetHeldItem()
        {
            if (GetItemAtRightHand() != null)
                return GetItemAtRightHand();
            if (GetItemAtLeftHand() != null)
                return GetItemAtLeftHand();
            return null;
        }

        public void AddItem(Entity item)
        {
            // This should be action/event.
            if (ItemList.Count <= ItemCapacity)
            {
                Game.UI.MainWindow.Message("You pick up the " + item.Name + ".");
                ItemList.Add(item);
                item.Position = new Point(-666, -666);
            }
            else
                Game.UI.MainWindow.Message("You have no space for the " + item + ".");
        }

        public void AddItems(List<Entity> items)
        {
            foreach (Entity item in items)
            {
                AddItem(item);
            }
        }

        public void RemoveItem(Entity item)
        {
            // This should be action/event.
            Game.UI.MainWindow.Message("You drop the " + item.Name + ".");
            ItemList.Remove(item);
            item.Position = Entity.Position;
        }

        public override void UpdateComponent()
        {
        }
    }


    public class CmpEnter : Component
    {
        // 0 = cave 1 = forest
        public int Type;
        public CmpEnter(params object[] args)
        {
            //_type = type;
        }
        public override void UpdateComponent() { }

        public void Enter()
        {

        }
    }
    public class CmpInput : Component
    {
        public override void UpdateComponent() { }
    }
    public class CmpAction : Component
    {

        private Action _currentAction;
        private Action _nextAction;
        private bool _unableToAct = false;
        public Action NextAction { get => _nextAction; set => _nextAction = value; }
        //   public Action CurrentAction { get => _currentAction; set => _currentAction = value; }
        public bool UnableToAct { get => _unableToAct; set => _unableToAct = value; }

        // called by user input or AI component
        public void SetAction(Action action)
        {
            _currentAction = action;
            _currentAction.EntityTime = Entity.EntityTime;
            Entity.EntityTime += _currentAction.TimeCost + 1;
            Game.UI.MainWindow.MainLoop.EventsList.Add(_currentAction);
        }

        public override void UpdateComponent() { }
    }
    public class CmpItem : Component
    {
        public decimal Weight;
        public bool OnMap;
        public string Description;
        public Entity Holder;

        public CmpItem(params object[] args)
        {
            OnMap = Convert.ToBoolean(args[0]);
            Weight = Convert.ToInt32(args[1]);
            Description = Convert.ToString(args[2]);
        }
        public override void UpdateComponent()
        {

        }
    }
    public class CmpEdibleItem : Component
    {

        public override void UpdateComponent()
        {

        }
    }
    public class CmpWearableItem : CmpItem
    {
        public int ItemCapacity;
        public CmpWearableItem(params object[] args)
        {
            if (Holder != null) ;
            //  holder.GetComponent<CmpBody>().itemCapacity += _itemCapacity;
        }
    }

    // Data for melee skills.
    public class CmpMelee : Component
    {
        public string damage;
        public int range;
        public int skillModifier;
        public CmpMelee(params object[] args)
        {
            Console.WriteLine("  a: " + args.Length);
            damage = Convert.ToString(args[0]);
            range = Convert.ToInt32(args[1]);
            skillModifier = Convert.ToInt32(args[2]);
        }

        public override void UpdateComponent() { }
    }

    public class CmpRanged : Component
    {
        public string damage;
        public int range;
        public int skillModifier;
        public CmpRanged(params object[] args)
        {
            damage = Convert.ToString(args[0]);
            range = Convert.ToInt32(args[1]);
            skillModifier = Convert.ToInt32(args[2]);
        }
        public override void UpdateComponent() { }
    }

    public class CmpAI : Component
    {
        public int Aggression { get; }

        public CmpAI(params object[] args)
        {
            Aggression = System.Convert.ToInt32(args[0]);
        }

        public void GetGoal()
        {
            Entity.GetComponent<CmpAction>().SetAction(new WaitAction(Entity));

        }


        public override void UpdateComponent() { }
    }


    // ====== EFFECTS ===============================

    public class CmpEffectStun : Component
    {
        private string _durationRoll;

        public string DurationRoll
        {
            get => _durationRoll;
            set => _durationRoll = value;
        }

        private bool _stunned;
        private int _duration;

        public int Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public CmpEffectStun(params object[] args)
        {
            _durationRoll = Convert.ToString(args[0]);
            _stunned = Convert.ToBoolean(args[1]); // if this component is in in Item, then this should be false.// when the item is used to attack and the target receives this effect, then it will be true.
            _duration = Convert.ToInt32(args[2]);
        }

        public override void UpdateComponent()
        {
            if (!_stunned) return;
            Entity.IsActionable = false;
            _duration -= 1; // TODO: make it so that different entities can have faster recovery.

            // check if entity can resist effect 
            if (Entity.GetComponent<CmpAttributes>().Guts + Entity.GetComponent<CmpAttributes>().Vitality >=
                Dice.Roll("1d100"))
            {
                _duration -= 1;
            }

            // when duration is 0 or less, change status and remove the component
            if (_duration >= 1) return;
            _stunned = false;
            Entity.IsActionable = true;
            Entity._components.Remove(this);
            Game.UI.MainWindow.Message(Entity.Name + " is no longer stunned.");
        }
    }


    public class CmpEffectSleep : Component
    {

        public CmpEffectSleep(params object[] args) { }

        public override void UpdateComponent() { }
    }


    public class CmpEffectEnraged : Component
    {

        public CmpEffectEnraged(params object[] args) { }

        public override void UpdateComponent() { }
    }


}
