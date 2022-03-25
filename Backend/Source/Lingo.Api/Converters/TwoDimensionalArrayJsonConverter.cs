using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lingo.Api.Converters;

public class TwoDimensionalArrayJsonConverter : JsonConverter<object[,]>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsArray && typeToConvert.GetArrayRank() == 2;
    }

    public override object[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, object[,] value, JsonSerializerOptions options)
    {
        int numberOfRows = value.GetLength(0);
        int numberOfColumns = value.GetLength(1);

        writer.WriteStartArray();
        for (int i = 0; i < numberOfRows; i++)
        {
            writer.WriteStartArray();
            for (int j = 0; j < numberOfColumns; j++)
            {
                string valueAsJson = JsonSerializer.Serialize(value.GetValue(i, j));
                writer.WriteRawValue(valueAsJson);
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}