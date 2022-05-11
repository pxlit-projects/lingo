using System.Text.Json;
using System.Text.Json.Serialization;
using Lingo.Domain.Pit.Contracts;

namespace Lingo.Api.Converters;

public class BallPitJsonConverter : JsonConverter<IBallPit>
{
    public override IBallPit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IBallPit value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, (IBallPit)null, options);
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