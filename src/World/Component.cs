using StarsHollow.Actions;
using StarsHollow.World;

using System.Collections.Generic;
using System.Text;

namespace StarsHollow.Components
{
    public interface IComponent
    {
        Entity Entity { get; set; }
       // string Name { get; set; }
    }

    abstract public class Component : IComponent
    {
        public List<IComponent> components = new List<IComponent>();
        public Entity Entity { get; set; }
        public string Name { get; set;  }
        //Components are updated every turn. Override this if update is needed.
        abstract public void UpdateComponent();

        public void GetComponentByName()
        {

        }
    }
    public class CmpTimer : Component
    {       
        private int _turn = 0;
        public int _minute = 0;
        public int _hour = 6;
        public int _day = 1;

        public int Turn { get => _turn;  }
        public CmpTimer()
        {
            Name = "timer";
        }
        public override void UpdateComponent()
        {
            _turn += 1;

            System.Console.WriteLine("turn: " + _turn.ToString());
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
    public class CmpEnter : Component
    {
        // 0 = cave 1 = forest
        public int _type;
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
            // _currentAction.Time = Entity.Time;
       //     _currentAction.Execute();
     //       Game.World.Gameloop.EventsList.Add(_currentAction);
        }

        public override void UpdateComponent() { }
    }
    public class CmpItem : Component
    {
        public decimal weight;
        public bool _onMap;
        public string description;
        public Entity holder;

        public CmpItem(decimal wght = 1, bool onMap = true, string descp = " ")
        {
            _onMap = onMap;
            weight = wght;
            description = descp;
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
        public int _itemCapacity;
        public CmpWearableItem(int itemCap = 1, string descp = " ")
        {
            description = descp;
            _itemCapacity = itemCap;
            if (holder != null) ;
              //  holder.GetComponent<CmpBody>().itemCapacity += _itemCapacity;
        }
    }

    // Data for melee skills.
    public class CmpMelee : Component
    {
        private int _attackStrength;
        public int _attackSkill;
        public int _defenceSkill;
        public CmpMelee(int attackStrength = 1, int attackSkill = 10, int defenceSkill = 10)
        {
            _attackStrength = attackStrength;
            _attackSkill = attackSkill;
            _defenceSkill = defenceSkill;
        }

        public int AttackStrength { get => _attackStrength; set => _attackStrength = value; }
        public int AttackSkill { get => _attackSkill; set => _attackSkill = value; }
        public int DefenceSkill { get => _defenceSkill; set => _defenceSkill = value; }

        public override void UpdateComponent() { }
    }

    public class CmpRanged : Component
    {
        private int _attackStrength;
        public int _attackSkill;
        public CmpRanged(int attackStrength = 0, int attackSkill = 10)
        {
            _attackStrength = attackStrength;
            _attackSkill = attackSkill;
        }
        public override void UpdateComponent() {}
    }

    public class CmpAI : Component
    {

        public override void UpdateComponent() {}
    }
}
