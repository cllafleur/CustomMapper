namespace MapperDslLib;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;

public static class MapperExtensions
{
    private static readonly IGetAccessorFactoryHandler[] extractGetAccessorFactoryHandlers = new IGetAccessorFactoryHandler[] {
        new ObjectGetAccessorFactoryHandler(),
    };

    private static readonly IGetAccessorFactoryHandler[] insertGetAccessorFactoryHandlers = new IGetAccessorFactoryHandler[] {
        new ArrayGetAccessorFactoryHandler(),
        new ObjectGetAccessorFactoryHandler(true),
    };

    private static readonly ISetAccessorFactoryHandler[] insertSetAccessorFactoryHandlers = new ISetAccessorFactoryHandler[] {
        new ArraySetAccessorFactoryHandler(),
        new ObjectSetAccessorFactoryHandler(),
    };

    private static readonly IGetAccessor deconstructorAccessor = new DeconstructJsonNodeAccessor();

    public static IMapperHandler<JsonNode, T> GetMapperFromJson<T>(this Mapper mapper)
    {
        var options = new CompilerOptions()
        {
            ExtractGetAccessorFactoryHandlers = extractGetAccessorFactoryHandlers,
            DeconstructorAccessor = deconstructorAccessor,
        };

        return mapper.GetMapper<JsonNode, T>(options);
    }

    public static IMapperHandler<JsonNode, JsonNode> GetMapperJsonToJson(this Mapper mapper)
    {
        var options = new CompilerOptions()
        {
            ExtractGetAccessorFactoryHandlers = extractGetAccessorFactoryHandlers,
            InsertGetAccessorFactoryHandlers = insertGetAccessorFactoryHandlers,
            InsertSetAccessorFactoryHandlers = insertSetAccessorFactoryHandlers,
            DeconstructorAccessor = deconstructorAccessor,
        };

        return mapper.GetMapper<JsonNode, JsonNode>(options);
    }
}
