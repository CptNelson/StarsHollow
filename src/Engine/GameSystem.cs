using System;
using System.Linq;
using GoRogue;
using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;
using StarsHollow.World;
using StarsHollow.UserInterface;

namespace StarsHollow.Engine
{
    // All systems are Observers.

    public class SystemMover : Observer
    {
        //TODO: this needs to be either the the whole move-command, or somehow make them both 
        // complement each other.
        private MoveBy _action;
        public override void DoSomething(object sender, System.EventArgs e)
        {
            // Get the entity that is moving and the position it tries to move.
            // If tile is walkable and there is no other entity, move it there.
            // Refresh the Player fov afterwards.
            _action = (MoveBy)sender;
            Point pos = _action.position;

            // if map is local
            if (Game.UI.world.CurrentMap.IsTileWalkable(pos))
            {
                if (!Game.UI.world.CurrentMap.IsThereEntityAt(pos))
                {
                    //System.Console.WriteLine("no one here. mover: " + _action._actor.Name);
                    _action.ActionActor.Position = pos;
                    //  Game.UI.CenterOnActor(Game.UI.world.Player);
                }
                else if (Game.UI.world.CurrentMap.IsThereEntityAt(pos))
                {
                    Entity entity = Game.UI.world.CurrentMap.GetFirstEntityAt<Entity>(pos);
                    if (!entity.NonBlocking)
                    {
                        Console.WriteLine("mover: " + _action.ActionActor.Name + " blocker: " + entity.Name);
                        return;
                    }
                    else
                        _action.ActionActor.Position = pos;
                }
            }
        }
    }


    public class SystemDamage : Observer
    {
        // decrease damage for target's health, and raise DestroyEvent if target dies.
        // TODO: check for items, resistances etc damage modifiers before decreasing health.
        public override void DoSomething(object sender, EventArgs e)
        {
            var @event = (DamageEvent)sender;
            Entity target = @event.Target;
            CmpHP healthComponent = target.GetComponent<CmpHP>();
            int damage = @event.Damage;

            healthComponent.Hp -= damage;

            //  Utils.StatusWindowUpdate(target);


            Game.UI.MainWindow.Message(target.Name + " took " + damage + " of damage.");

            if (healthComponent.Hp < 0)
            {
                healthComponent.Alive = false;
                DeathEvent destroyEvent = new DeathEvent(target);
            }
        }
    }



    public class SystemSkills : Observer
    {
        // Checks what Action raised the event, and chooses the correct method for the skill check.
        // Skills are usually checked by comparing them to a 1d100 die roll, roll under the skill value is success.
        // If comparing against other entity, both roll the die, add their value and then compare. One with higher result wins.
        // Everytime skill check fails, it raises ExperienceEvent, which might rise the skill. 
        public override void DoSomething(object sender, System.EventArgs e)
        {
            base.sender = sender;
            args = e;
            if (sender is MeleeAttack)
                MeleeAttack();

            if (sender is Shoot)
                RangedAttack();
        }

        public void MeleeAttack()
        {
            // Compares attacker's Attack skill to target's Defence skill.
            // If target throws 95+, they get Attack of Opportunity.
            // If attacker throws 95+, they do Critical Hit.
            // Successfull hit raises DamageEvent.
            var _action = (MeleeAttack)sender;
            Entity attacker = _action.ActionActor;
            Entity target = Game.UI.world.CurrentMap.GetFirstEntityAt<Entity>(attacker.Position + _action.Dir);
            if (target != null)
            {
                CmpAttributes attributes = attacker.GetComponent<CmpAttributes>();
                CmpAttributes targetAttributes = target.GetComponent<CmpAttributes>();

                int attackerWeaponSkill = 0;
                int attackerWeaponDamage = 0;
                int targetWeaponSkill = 0;
                int targetWeaponDamage = 0;

                if (attacker.GetComponent<CmpBody>().IsHoldingItem())
                {
                    attackerWeaponSkill = attacker.GetComponent<CmpBody>().GetHeldItem().GetComponent<CmpMelee>().skillModifier;
                    attackerWeaponDamage = Dice.Roll(attacker.GetComponent<CmpBody>().GetItemAtRightHand().GetComponent<CmpMelee>().damage);
                }
                if (target.GetComponent<CmpBody>().IsHoldingItem())
                {
                    targetWeaponSkill = attacker.GetComponent<CmpBody>().GetHeldItem().GetComponent<CmpMelee>().skillModifier;
                }

                int attackSkill = attributes.Agility + attributes.Strength / 4 + attributes.Smarts / 4 + attackerWeaponSkill;
                int defenceSkill = targetAttributes.Agility + attributes.Strength / 4 + attributes.Guts / 4 + targetWeaponSkill;

                int attackRoll = Dice.Roll("1d100");
                int defenceRoll = Dice.Roll("1d100");
                int damageBonus = 0;


                if (attackSkill + attackRoll > defenceSkill + defenceRoll)
                {
                    if (attackRoll >= 95)
                        damageBonus = Dice.Roll("1d4");

                    // add damage bonus from weapon
                    damageBonus += attackerWeaponDamage;

                    int damage = Dice.Roll("1d" + attributes.Strength / 5) + damageBonus;

                    Game.UI.MainWindow.Message(attacker.Name + " hit " + target.Name + "!");

                    //ExperienceEvent experienceEvent = new ExperienceEvent(target, ref target.GetComponent<CmpMelee>()._defenceSkill, "defence skill");
                    DamageEvent damageEvent = new DamageEvent(target, damage);
                    Game.UI.world.SystemDamage.Subscribe(damageEvent);
                    damageEvent.NotifyObservers();
                }
                else
                {
                    if (defenceRoll >= 95)
                    {
                        // this needs to be changed
                        int damage = Dice.Roll("1d4");
                        Game.UI.MainWindow.Message(target.Name + " evaded " + attacker.Name + "'s attack and executed a counter-attack!");
                        DamageEvent damageEvent = new DamageEvent(attacker, damage);
                        Game.UI.world.SystemDamage.Subscribe(damageEvent);
                        damageEvent.NotifyObservers();
                    }
                    else
                    {
                        Game.UI.MainWindow.Message(attacker.Name + " tried to hit " + target.Name + " but failed.");
                        //ExperienceEvent experienceEvent = new ExperienceEvent(attacker, ref attacker.GetComponent<CmpMelee>()._attackSkill, "attack skill");
                    }
                }

            }
            // If there is no entity at target position
            else
                Game.UI.MainWindow.Message(attacker.Name + " hits empty air!");
        }

        public void RangedAttack()
        {
            Console.WriteLine("shooting");
            var _action = (Shoot)sender;
            Entity attacker = _action.ActionActor;

            var line = Lines.Get(attacker.Position, _action.targetPosition, Lines.Algorithm.DDA);
            //System.Console.WriteLine(_action.position+" P0:"+attacker.Position); 
            //line.Reverse();
            //line.RemoveAt(0);
            foreach (Point pos in line.Skip(1))
            {
                //System.Console.WriteLine(pos+ " P: " + attacker.Position); 
                Entity target = Game.UI.world.CurrentMap.GetFirstEntityAt<Entity>(pos);
                if (target != null)
                {
                    CmpAttributes attributes = attacker.GetComponent<CmpAttributes>();

                    int attackRoll = Dice.Roll("1d100");
                    int damageBonus = 0;

                    int attackerWeaponSkill = 0;
                    int attackerWeaponDamage = 0;

                    attackerWeaponSkill = attacker.GetComponent<CmpBody>().GetHeldItem().GetComponent<CmpRanged>()
                        .skillModifier;
                    attackerWeaponDamage = Dice.Roll(attacker.GetComponent<CmpBody>().GetItemAtRightHand()
                        .GetComponent<CmpRanged>().damage);

                    int attackSkill = attributes.Agility + attributes.Guts / 4 + attributes.Smarts / 4 +
                                      attackerWeaponSkill;

                    if (attackSkill <= attackRoll)
                    {
                        if (attackRoll <= 5)
                            damageBonus += Dice.Roll("1d4");
                        int damage = Dice.Roll("1d4") + damageBonus;

                        CheckForStatusEffects(attacker, target);

                        Game.UI.MainWindow.MainLoop.EventsList.Add(new ProjectileAnimation(attacker.Position,
                            target.Position));
                        Game.UI.MainWindow.Message(attacker.Name + " hit " + target.Name + "!");
                        DamageEvent damageEvent = new DamageEvent(target, damage);
                        Game.UI.world.SystemDamage.Subscribe(damageEvent);
                        damageEvent.NotifyObservers();
                        break;
                    }
                    else
                    {

                        Game.UI.MainWindow.MainLoop.EventsList.Add(new ProjectileAnimation(attacker.Position,
                            target.Position));
                        Game.UI.MainWindow.Message(attacker.Name + " missed " + target.Name + ".");
                        return;
                        //ExperienceEvent experienceEvent = new ExperienceEvent(attacker, ref attacker.GetComponent<CmpRanged>()._attackSkill, "attack skill");
                    }
                }

                if (!Game.UI.world.CurrentMap.IsTileWalkable(pos))
                {
                    Game.UI.MainWindow.MainLoop.EventsList.Add(new ProjectileAnimation(attacker.Position, pos));
                    Console.WriteLine("third");
                    Game.UI.MainWindow.Message("You hit a wall!");
                    break;
                }

                if (pos == _action.targetPosition)
                {
                    Game.UI.MainWindow.MainLoop.EventsList.Add(new ProjectileAnimation(attacker.Position, pos));
                    Console.WriteLine("fourth");
                }

            }
        }


        void CheckForStatusEffects(Entity attacker, Entity target)
        {
            if (attacker.GetComponent<CmpBody>().GetHeldItem().HasComponent<CmpEffectStun>())
            {
                int duration = Dice.Roll(attacker.GetComponent<CmpBody>().GetHeldItem().GetComponent<CmpEffectStun>()
                    .DurationRoll);
                Console.WriteLine("d: " + duration);

                if (target.GetComponent<CmpAttributes>().Guts / 2 + target.GetComponent<CmpAttributes>().Vitality / 2 >=
                    Dice.Roll("1d100"))
                {
                    Game.UI.MainWindow.Message(target.Name + " resisted stun.");
                    return;
                }


                // In case target is already stunned, don't add new one.
                if (target.HasComponent<CmpEffectStun>())
                {
                    target.GetComponent<CmpEffectStun>().Duration += duration;
                    Game.UI.MainWindow.Message(target.Name + " got stunned again!");
                    return;
                }

                target.AddComponent(new CmpEffectStun(new object[] { "", true, duration }));
                Game.UI.MainWindow.Message(target.Name + " is stunned!");
            }
        }


    }
}

