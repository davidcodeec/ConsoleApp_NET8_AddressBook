

using ClassLibrary.Shared.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WPF_App_NET8.Interfaces;

public class IContactConverter : JsonConverter<IContact>
{
    public override IContact ReadJson(JsonReader reader, Type objectType, IContact existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);

        // Extract the concrete type property from JSON
        var typeName = jObject["$type"]?.ToString();

        // Determine the concrete type based on the typeName
        Type concreteType = Type.GetType(typeName);

        // Deserialize the JSON into the concrete type
        return (IContact)jObject.ToObject(concreteType, serializer);
    }

    public override void WriteJson(JsonWriter writer, IContact value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}