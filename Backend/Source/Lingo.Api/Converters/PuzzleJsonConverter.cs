using System.Text.Json;
using System.Text.Json.Serialization;
using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.Api.Converters;

public class PuzzleJsonConverter : JsonConverter<IPuzzle>
{
    public override IPuzzle? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IPuzzle value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, (IPuzzle)null, options);
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