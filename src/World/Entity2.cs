using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using GoRogue;
using StarsHollow.UserInterface;
using StarsHollow.World;

namespace StarsHollow.World
{
    // The most basic, IEntity. Time is entity's current time in timeline.
    // Actionable entities are looped in the main game loop.
    public interface IEntity : IHasID
    {
        bool IsActionable { get; set; }
        uint EntityTime { get; set; }
    }
    public class Entity : IEntity
    {
        // All the components entity has.
        public List<IComponent> EntComponents { get; set; }
        public SadConsole.Entities.Entity Sprite { get; set; }
        // Every Entity has unique ID
        public uint ID { get; set; }
        public uint EntityTime { get; set; }
        // name for checking what kind of entity it is.(TODO: make this a enum)
        public string TypeName { get; set; }
        // check if entity blocks movement
        public bool NonBlocking { get; set; }
        public bool IsActionable { get; set; }

        public Entity(int width = 1, int height = 1)
        {
            Sprite = new SadConsole.Entities.Entity(width, height);
            Sprite.Font = Fonts.halfSizeFont;
            Sprite.Animation.CurrentFrame[0].Foreground = Color.White;
            Sprite.Animation.CurrentFrame[0].Background = Color.Transparent;
            Sprite.Animation.CurrentFrame[0].Glyph = 'X';
            // Animation is set to invisible in the beginning. FOV calculations will change this.
            Sprite.Animation.IsVisible = false;
            Sprite.Name = "name";
            Sprite.Position = new Point(-1, -1);

            TypeName = "type";
            EntityTime = 0;
            IsActionable = false;
            EntComponents = new List<IComponent>();
            ID = Map.IDGenerator.UseID();
            NonBlocking = false;
        }

        // adds chosen component to the list of components, and makes the entity owner of the component.
        public Entity AddComponent(IComponent newComponent)
        {
            if (newComponent == null)
            {
                Console.WriteLine("Component that you intented to add is null, method will return void");
                return this;
            }
            newComponent.Entity = this;
            EntComponents.Add(newComponent);
            return this;
        }


        // add multiple components at once.
        public Entity AddComponents(List<IComponent> components)
        {
            foreach (IComponent i in components)
            {
                AddComponent(i);
            }
            if (components.Count == 0)
            {
                Console.WriteLine("Component that you intented to add is null, method will return void");
                return this;
            }
            return this;
        }

        public Entity AddComponentsFromFile(JObject components)
        {
            // Get the name of the Component
            foreach (KeyValuePair<string, JToken> tag in components)
            {
                var property = tag.Key;
                var args = new object[tag.Value.Count()];
                for (int i = 0; i < tag.Value.Count(); i++)
                {
                    args[i] = tag.Value.ElementAt(i).First;
                }
                Type cmpType = Type.GetType("StarsHollow.World." + property);
                var newComponent = (Component)Activator.CreateInstance(cmpType, args);

                if (newComponent == null)
                {
                    Console.WriteLine("Component that you intended to add is null, method will return void");
                    return this;
                }
                newComponent.Entity = this;
                EntComponents.Add(newComponent);
            }
            return this;
        }
        // get component by referencing its type/class
        public List<IComponent> GetComponents()
        {
            if (EntComponents.Count > 0)
            {
                // foreach (Component cmp in _components)
                // Console.WriteLine(cmp);
                return EntComponents;
            }
            else
                return null;
        }

        public new T GetComponent<T>() where T : class, IComponent
        {
            foreach (IComponent cmp in EntComponents)
                if (cmp is T)
                    return (T)cmp;

            return null;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            foreach (IComponent cmp in EntComponents)
                if (cmp is T) return true;

            return false;
        }
        public event EventHandler<EntityMovedEventArgs> Moved;
        public class EntityMovedEventArgs
        {
            public readonly Entity Entity;
            public readonly Point FromPosition;

            public EntityMovedEventArgs(Entity entity, Point oldPosition) { }
        }
    }

}


