using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using GoRogue;
using StarsHollow.UserInterface;

namespace StarsHollow.World
{
    // The most basic, IEntity. Time is entity's current time in timeline.
    // Actionable entities are looped in the main game loop.
    public interface IEntity : IHasID
    {
        bool IsActionable { get; set; }
        uint EntityTime { get; set; }
    }
    public class Sprite : SadConsole.Entities.Entity, IHasID
    {
        public Sprite(Color fg, Color bg, int glyph, Point pos, string name, bool isVisible, int width = 1, int height = 1) : base(width, height) { }
        public uint ID { get; set; }
        public Entity owner { get; set; }
    }
    public class Entity : IEntity
    {
        // All the components entity has.
        public List<Component> EntComponents { get; set; }

        public Sprite Sprite { get; set; }
        // Every Entity has unique ID
        public uint ID { get; set; }
        public uint EntityTime { get; set; }
        // name for checking what kind of entity it is.(TODO: make this a enum)
        public string TypeName { get; set; }
        // check if entity blocks movement
        public bool NonBlocking { get; set; }
        public bool IsActionable { get; set; }
        public bool IsCrouching { get; set; }
        public uint MoveCostMod { get; set; }

        public Entity()
        {
            Sprite = new Sprite(Color.White, Color.Transparent, 1, new Point(-1, -1), "name", false, 1, 1);
            Sprite.Font = Fonts.halfSizeFont;
            Sprite.Animation.CurrentFrame[0].Glyph = 'X';
            // Animation is set to invisible in the beginning. FOV calculations will change this.
            Sprite.Animation.IsVisible = false;
            Sprite.Position = new Point(-1, -1);
            Sprite.owner = this;
            Sprite.ID = Map.IDGenerator.UseID();



            TypeName = "type";
            EntityTime = 0;
            IsActionable = false;
            EntComponents = new List<Component>();
            NonBlocking = false;
            IsCrouching = false;
            MoveCostMod = 0;
        }

        // adds chosen component to the list of components, and makes the entity owner of the component.
        public Entity AddComponent(Component newComponent)
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
        public Entity AddComponents(List<Component> components)
        {
            foreach (Component i in components)
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
            foreach (KeyValuePair<string, JToken> tag in components)
            {

                //Console.WriteLine("A: " + tag + " B: " + tag.Key);
                var property = tag.Key;
                var args = new object[tag.Value.Count()];
                Console.WriteLine("prop: " + property);
                for (int i = 0; i < tag.Value.Count(); i++)
                {
                    args[i] = tag.Value.ElementAt(i).First;
                    Console.WriteLine("args: " + args[i]);
                }
                //Type cmpType = Type.GetType("StarsHollow.World." + property);
                Type cmpType = Type.GetType(property);
                //Console.WriteLine(cmpType);
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
        public List<Component> GetComponents()
        {
            if (EntComponents.Count > 0)
            {
                return EntComponents;
            }
            else
                return null;
        }
        public bool ListComponents()
        {
            foreach (Component cmp in EntComponents)
            {
                Console.WriteLine(cmp.GetType().ToString());
            }
            return true;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component cmp in EntComponents)
                if (cmp is T)
                    return (T)cmp;
            return null;
        }

        public bool HasComponent<T>() where T : Component
        {
            foreach (Component cmp in EntComponents)
                if (cmp is T) return true;

            return false;
        }
    }
}


