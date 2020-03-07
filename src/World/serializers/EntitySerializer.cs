using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace StarsHollow.World
{
    //  #pragma warning disable 1591
    public class EntityConverterJson : JsonConverter<Entity>
    {
        public override void WriteJson(JsonWriter writer, Entity value, JsonSerializer serializer) => serializer.Serialize(writer, (EntitySerialized)value);

        public override Entity ReadJson(JsonReader reader, Type objectType,
            Entity existingValue, bool hasExistingValue, JsonSerializer serializer) => serializer.Deserialize<EntitySerialized>(reader);
    }

    [DataContract]
    public class EntitySerialized
    {
        // Sprite fields
        [DataMember] public Color FgColor;
        [DataMember] public Color BgColor;
        [DataMember] public int Glyph;
        [DataMember] public bool IsVisible;
        [DataMember] public Point Position;
        [DataMember] public string Name;

        [DataMember] public List<Component> EntComponents;
        [DataMember] public uint ID;
        [DataMember] public uint EntityTime;
        [DataMember] public bool NonBlocking;
        [DataMember] public bool IsActionable;
        [DataMember] public bool IsCrouching;
        [DataMember] public uint MoveCostMod;

        public static implicit operator EntitySerialized(Entity entity) => new EntitySerialized()
        {
            FgColor = entity.Sprite.Animation.CurrentFrame[0].Foreground,
            BgColor = entity.Sprite.Animation.CurrentFrame[0].Background,
            Glyph = entity.Sprite.Animation.CurrentFrame[0].Glyph,
            IsVisible = entity.Sprite.IsVisible,
            Position = entity.Sprite.Position,
            Name = entity.Sprite.Name,
            ID = entity.Sprite.ID,

            EntComponents = entity.EntComponents,
            EntityTime = entity.EntityTime,
            NonBlocking = entity.NonBlocking,
            IsActionable = entity.IsCrouching,
            IsCrouching = entity.IsCrouching,
            MoveCostMod = entity.MoveCostMod

        };

        public static implicit operator Entity(EntitySerialized serializedObject) => new Entity()
        {
            //FIXME: how to set Owner and ID here?
            Sprite = new Sprite(serializedObject.FgColor, serializedObject.BgColor, serializedObject.Glyph, serializedObject.Position, serializedObject.Name, 1, 1),
            EntComponents = serializedObject.EntComponents,
            EntityTime = serializedObject.EntityTime,
            NonBlocking = serializedObject.NonBlocking,
            IsActionable = serializedObject.IsCrouching,
            IsCrouching = serializedObject.IsCrouching,
            MoveCostMod = serializedObject.MoveCostMod
        };
    }
}