using StarsHollow.World;

namespace StarsHollow.Engine
{
    public class DamageEvent : Subject
    {
        public Entity _target;
        public int _damage;
        public DamageEvent(Entity target, int damage)
        {
            _target = target;
            _damage = damage;
        }
    }
}