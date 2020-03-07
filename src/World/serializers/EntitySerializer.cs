using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        [DataMember] public Sprite Sprite;
        [DataMember] public List<Component> EntComponents;
        [DataMember] public uint ID;
        [DataMember] public uint EntityTime;
        [DataMember] public bool NonBlocking;
        [DataMember] public bool IsActionable;
        [DataMember] public bool IsCrouching;
        [DataMember] public uint MoveCostMod;

        public static implicit operator EntitySerialized(Entity entity) => new EntitySerialized()
        {
            Sprite = entity.Sprite,
            EntComponents = entity.EntComponents,
            ID = entity.ID,
            EntityTime = entity.EntityTime,
            NonBlocking = entity.NonBlocking,
            IsActionable = entity.IsCrouching,
            IsCrouching = entity.IsCrouching,
            MoveCostMod = entity.MoveCostMod

        };

        public static implicit operator Entity(EntitySerialized serializedObject) => new Entity()
        {

            Sprite = serializedObject.Sprite,
            EntComponents = serializedObject.EntComponents,
            ID = serializedObject.ID,
            EntityTime = serializedObject.EntityTime,
            NonBlocking = serializedObject.NonBlocking,
            IsActionable = serializedObject.IsCrouching,
            IsCrouching = serializedObject.IsCrouching,
            MoveCostMod = serializedObject.MoveCostMod
        };
    }
}