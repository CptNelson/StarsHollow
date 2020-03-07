using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StarsHollow.Engine;
using Action = StarsHollow.Engine.Action;

namespace StarsHollow.World
{
    public class Component// : IComponent
    {
        [JsonIgnore]
        public List<Component> Components;// = new List<Component>();

        [JsonIgnore]
        public Entity Entity { get; set; }
        protected string Name { get; set; }
        //Components are updated every turn. Override this if update is needed.
        public virtual void UpdateComponent() { }
    }

    public class CmpTimer : Component
    {
        public int Turn { get; private set; }

        public int Minute { get; private set; }
        public int Hour { get; private set; }
        public int Day { get; private set; }

        public CmpTimer(params object[] args)
        {
            Name = "timer";
        }

        public override void UpdateComponent()
        {
            AdvanceTime();
        }

        private void AdvanceTime()
        {
            Turn += 1;

            Entity.EntityTime += 100;

            if (Entity.EntityTime % 100 == 0)
                Minute++;
            if (Minute > 59)
            {
                Hour++;
                Minute = 0;
            }

            if (Hour > 23)
            {
                Hour = 0;
                Day++;
            }
        }
    }

    public class CmpHP : Component
    {
        public int Hp { get; set; }
        public int CurrentHp { get; set; }
        public bool Alive { get; set; }

        public CmpHP(params object[] args)
        {
            Hp = Convert.ToInt32(args[0]);
            CurrentHp = Hp;
        }

        public override void UpdateComponent()
        {
        }

        public void ChangeCurrentHp(int amount)
        {
            CurrentHp += amount;
        }

        public void ChangeMaxHp(int amount)
        {
            Hp += amount;
        }
    }

    public class CmpAttributes : Component
    {


        public int Strength { get; set; }

        public int Agility { get; set; }
        public int Vitality { get; set; }
        public int Looks { get; set; }
        public int Guts { get; set; }
        public int Smarts { get; set; }


        public CmpAttributes(params object[] args)
        {

            Strength = System.Convert.ToInt32(args[0]);
            Agility = System.Convert.ToInt32(args[1]);
            Vitality = System.Convert.ToInt32(args[2]);

            Looks = System.Convert.ToInt32(args[3]);
            Guts = System.Convert.ToInt32(args[4]);
            Smarts = System.Convert.ToInt32(args[5]);

        }

        public override void UpdateComponent()
        {
        }
    }

    public class CmpBody : Component
    {
        public int ItemCapacity = 3;
        public List<Entity> ItemList;
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
                Game.UI.MainWindow.Message("You pick up the " + item.Sprite.Name + ".");
                ItemList.Add(item);
                item.Sprite.Position = new Point(-666, -666);
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
            Game.UI.MainWindow.Message("You drop the " + item.Sprite.Name + ".");
            ItemList.Remove(item);
            item.Sprite.Position = Entity.Sprite.Position;
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
        public CmpInput(params object[] args) { }
        public override void UpdateComponent() { }
    }
    public class CmpAction : Component
    {
        public CmpAction(params object[] args)
        {

        }

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
            //if (Holder != null) 
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

        public AIComponent CurrentAIComponent { get; set; }

        public CmpAI(params object[] args)
        {

            Aggression = System.Convert.ToInt32(args[0]);
            //CurrentAIComponent = (AIComponent)args[1];
        }

        public void AddAIComponent(AIComponent newComponent)
        {
            if (newComponent == null)
            {
                Console.WriteLine("Component that you intented to add is null, method will return void");
                return;
            }

            Components.Add(newComponent);
            CurrentAIComponent = newComponent;
        }

        public void GetGoal()
        {


            CurrentAIComponent.ChooseNextAction();

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
            Entity.EntComponents.Remove(this);
            Game.UI.MainWindow.Message(Entity.Sprite.Name + " is no longer stunned.");
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
