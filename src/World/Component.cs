using System.Collections.Generic;
using StarsHollow.Engine;

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

        protected string Name { get; set;  }
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
        
        public int Turn { get => _turn;  }
        public CmpTimer()
        {
            Name = "timer";
        }
        public override void UpdateComponent()
        {
            _turn += 1;

            //System.Console.WriteLine("turn: " + _turn.ToString());
            Entity.Time += 100;

            if (Entity.Time % 100 == 0)
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
        private bool _alive = true;

        public CmpHP(params object[] args)
        {
            _hp = System.Convert.ToInt32(args[0]); 
        }
        public int Hp { get => _hp; set => _hp = value; }
        public bool Alive { get => _alive; set => _alive = value; }

        public override void UpdateComponent()
        {
        } 
    }

    public class CmpAttributes : Component
    {
        
        private int _strength;
        private int _agility;
        private int _reflex;
        private int _vitality;

        private int _charisma;
        private int _hunch;
        private int _smarts;
        
        public int Strength => _strength;
        public int Agility => _agility;
        public int Reflex => _reflex;
        public int Vitality => _vitality;
        public int Charisma => _charisma;
        public int Hunch => _hunch;
        public int Smarts => _smarts;


        public CmpAttributes(params object[] args)
        {
         _strength = System.Convert.ToInt32(args[0]); 
         _agility = System.Convert.ToInt32(args[1]); 
         _reflex = System.Convert.ToInt32(args[2]);
         _vitality = System.Convert.ToInt32(args[3]);
         
         _charisma = System.Convert.ToInt32(args[4]); 
         _hunch = System.Convert.ToInt32(args[5]); 
         _smarts = System.Convert.ToInt32(args[6]); 
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
        
        private Action _currentAction = new WaitAction();
        private Action _nextAction;
        private bool _unableToAct = false;
        public Action NextAction { get => _nextAction; set => _nextAction = value; }
     //   public Action CurrentAction { get => _currentAction; set => _currentAction = value; }
        public bool UnableToAct { get => _unableToAct; set => _unableToAct = value; }

        // called by user input or AI component
        public void SetAction(Action action)
        {
            _currentAction = action;
            _currentAction.Actor = Entity;
            _currentAction.Time = Entity.Time;
            Entity.Time += _currentAction.Cost + 1;
            Game.UI.MainWindow.MainLoop.EventsList.Add(_currentAction);
            // _currentAction.Time = Entity.Time;
            //     _currentAction.Execute();
            //       Game.World.Gameloop.EventsList.Add(_currentAction);
        }

        public override void UpdateComponent() { }
    }
    public class CmpItem : Component
    {
        public decimal Weight;
        public bool OnMap;
        public string Description;
        public Entity Holder;

        public CmpItem(decimal weight = 1, bool onMap = true, string descp = " ")
        {
            OnMap = onMap;
            Weight = weight;
            Description = descp;
        }
        public override void UpdateComponent()
        {

        }
    }
    public class CmpEdibleItem : CmpItem
    {

    }
    public class CmpWearableItem : CmpItem
    {
        public int ItemCapacity;
        public CmpWearableItem(int itemCap = 1, string descp = " ")
        {
            Description = descp;
            ItemCapacity = itemCap;
            if (Holder != null) ;
              //  holder.GetComponent<CmpBody>().itemCapacity += _itemCapacity;
        }
    }

    // Data for melee skills.
    public class CmpMelee : Component
    {
        private int _attackStrength;
        private int _attackSkill;
        private int _defenceSkill;
        public CmpMelee(params object[] args)
        {
            _attackStrength = System.Convert.ToInt32(args[0]); 
            _attackSkill = System.Convert.ToInt32(args[1]); 
            _defenceSkill = System.Convert.ToInt32(args[2]); 
        }

        public int AttackStrength { get => _attackStrength; set => _attackStrength = value; }
        public int AttackSkill { get => _attackSkill; set => _attackSkill = value; }
        public int DefenceSkill { get => _defenceSkill; set => _defenceSkill = value; }

        public override void UpdateComponent() { }
    }

    public class CmpRanged : Component
    {
        private int _attackStrength;
        private int _attackSkill;
        public CmpRanged(int attackStrength = 0, int attackSkill = 10)
        {
            _attackStrength = attackStrength;
            _attackSkill = attackSkill;
        }
        public override void UpdateComponent() {}
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
            Entity.GetComponent<CmpAction>().SetAction(new WaitAction());

        }


        public override void UpdateComponent() {}
    }
}
