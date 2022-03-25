using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Lingo.Api.Filters;

public class ArrayJsonOutputFormatter : SystemTextJsonOutputFormatter
{
    public ArrayJsonOutputFormatter(JsonSerializerOptions jsonSerializerOptions) : base(jsonSerializerOptions)
    {
    }

    protected override bool CanWriteType(Type? type)
    {
        return type.IsArray && type.GetElementType().IsArray;
    }

    public override Task WriteAsync(OutputFormatterWriteContext context)
    {
        var array = context.Object as Array;

        if (array.Rank == 2)
        {
            int numberOfRows = array.GetLength(0);
            int numberOfColumns = array.GetLength(1);

            var jaggedArray = new object[numberOfRows][];
            for (int i = 0; i < numberOfRows; i++)
            {
                jaggedArray[i] = new object[numberOfColumns];
                for (int j = 0; j < numberOfColumns; j++)
                {
                    jaggedArray[i][j] = array.GetValue(i, j);
                }
            }
            context = new OutputFormatterWriteContext(context.HttpContext, context.WriterFactory, jaggedArray.GetType(), jaggedArray);
        }

        return base.WriteAsync(context);
    }
}