using System.Text.Json;
using System.Text.Json.Serialization;
using Lingo.Domain.Contracts;

namespace Lingo.Api.Converters;

public class PlayerJsonConverter : JsonConverter<IPlayer>
{
    public override IPlayer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IPlayer value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, (IPlayer)null, options);
                break;
            default:
            {
                var type = value.GetType();
                JsonSerializer.Serialize(writer, value, type, options);
                break;
            }
        }
    }
}