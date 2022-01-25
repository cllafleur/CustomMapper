namespace MapperDslLib.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NUnit.Framework;

public class MapperWithSystemJsonTests
{
    class OriginObject
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime ModificationDate { get; set; }
    }
    class TargetObject
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ModificationDate { get; set; }

    }

    private const string CASE1 = @"Title -> Title
Description -> Description
ModificationDate -> ModificationDate
";

    [Test]
    public void Map_OriginAsJsonobjectTargetAsStaticType_SetRootLevel_Success()
    {
        OriginObject originObj = new OriginObject();
        originObj.Title = "supertitle";
        originObj.Description = "superdescription";
        originObj.ModificationDate = new DateTime(2021, 07, 31);
        var target = new TargetObject();
        var oSerialized = JsonSerializer.Serialize(originObj);
        var origin = JsonNode.Parse(oSerialized).AsObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
        mapper.Load();
        var handler = mapper.GetMapperFromJson<TargetObject>();

        handler.Map(origin, target);

        Assert.AreEqual(originObj.Title, target.Title);
        Assert.AreEqual(originObj.ModificationDate.ToString("s"), target.ModificationDate);
        Assert.AreEqual(originObj.Description, target.Description);
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetRootLevel_Success()
    {
        JsonObject originObj = new JsonObject();
        originObj.Add("Title", JsonValue.Create("supertitle"));
        originObj.Add("Description", JsonValue.Create("superdescription"));
        originObj.Add("ModificationDate", JsonValue.Create(new DateTime(2021, 07, 31)));
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(originObj["Title"].ToString(), target["Title"].ToString());
        Assert.AreEqual(originObj["ModificationDate"].ToString(), target["ModificationDate"].ToString());
        Assert.AreEqual(originObj["Description"].ToString(), target["Description"].ToString());
    }

    private const string CASE2 = @"Sub.Title -> Title
Sub.Description -> Description
Sub.ModificationDate -> ModificationDate
";

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetFromLevel2To1Level_Success()
    {
        JsonObject originObj = new JsonObject();
        JsonObject sub = new JsonObject();
        originObj.Add("Sub", sub);
        sub.Add("Title", JsonValue.Create("supertitle"));
        sub.Add("Description", JsonValue.Create("superdescription"));
        sub.Add("ModificationDate", JsonValue.Create(new DateTime(2021, 07, 31)));
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE2));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(sub["Title"].ToString(), target["Title"].ToString());
        Assert.AreEqual(sub["ModificationDate"].ToString(), target["ModificationDate"].ToString());
        Assert.AreEqual(sub["Description"].ToString(), target["Description"].ToString());
    }

    private const string CASE3 = @"Title -> Sub.Title
Description -> Sub.Description
ModificationDate -> Sub.ModificationDate
";

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetFromLevel1To2Level_Success()
    {
        JsonObject originObj = new JsonObject();
        originObj.Add("Title", JsonValue.Create("supertitle"));
        originObj.Add("Description", JsonValue.Create("superdescription"));
        originObj.Add("ModificationDate", JsonValue.Create(new DateTime(2021, 07, 31)));
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE3));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(originObj["Title"].ToString(), target["Sub"]["Title"].ToString());
        Assert.AreEqual(originObj["ModificationDate"].ToString(), target["Sub"]["ModificationDate"].ToString());
        Assert.AreEqual(originObj["Description"].ToString(), target["Sub"]["Description"].ToString());
    }

    private const string Json1 = @"{
       ""reference"": ""BU18I065-1915"",
       ""creationDate"": ""2018-06-20T10:44:35.807"",
       ""modificationDate"": ""2021-08-19T00:00:31.787"",
       ""extras"": {
        ""longField"": [ 12345 ]
       }
}";

    private const string CASE4 = @"reference -> Sub.Title
creationDate -> Sub.CreationDate
modificationDate -> Sub.ModificationDate
extras.longField -> Ex.long
";

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetFromJsonLevel1To2Level_Success()
    {
        var originObj = JsonNode.Parse(Json1);
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE4));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(originObj["reference"].ToString(), target["Sub"]["Title"].ToString());
        Assert.AreEqual(originObj["modificationDate"].ToString(), target["Sub"]["ModificationDate"].ToString());
        Assert.AreEqual(originObj["creationDate"].ToString(), target["Sub"]["CreationDate"].ToString());
        Assert.AreEqual(originObj["extras"]["longField"][0].GetValue<int>(), target["Ex"]["long"].GetValue<int>());
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetFromArrayToScalarField_Success()
    {
        var json = @"{
            ""array"": [ 1, 2 ],
            ""array2"": [
                { ""text"": ""textvalue"" }
            ]
}";
        var mappingDef = @"
array -> valueArray
array2.text -> valueArray2
";

        var originObj = JsonNode.Parse(json);
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(originObj["array"][0].ToString(), target["valueArray"].ToString());
        Assert.AreEqual(originObj["array2"][0]["text"].ToString(), target["valueArray2"].ToString());
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetFromArrayToArrayField_Success()
    {
        var json = @"{
            ""array"": [ 1, 2 ],
            ""array2"": [
                { ""text"": ""textvalue"" },
                { ""text"": ""textvalue2"" }
            ]
}";
        var mappingDef = @"
array -> valueArray*
array2.text -> valueArray2*
";

        var originObj = JsonNode.Parse(json);
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(originObj["array"].AsArray().ToString(), target["valueArray"].AsArray().ToString());
        Assert.AreEqual(originObj["array2"].AsArray().Select(i => i["text"].ToString()).ToList(), target["valueArray2"].AsArray().Select(i => i.ToString()).ToList());
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetFromArrayToUnamedArrayField_Success()
    {
        var json = @"{
            ""array"": [ 1, 2 ],
            ""array2"": [
                { ""text"": ""textvalue"" },
                { ""text"": ""textvalue2"" }
            ]
}";
        var mappingDef = @"
array -> *
array2.text -> *
";

        var originObj = JsonNode.Parse(json);
        var target = new JsonArray();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(
            originObj["array"].AsArray().Concat(originObj["array2"].AsArray().Select(i => i["text"])).Select(i => i.ToString()),
            target.AsArray().Select(i => i.ToString()));
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetIterativeOnMiddleArrayField_Success()
    {
        var json = @"{
            ""array"": [ 1, 2 ],
            ""array2"": [
                { ""text"": ""textvalue"" },
                { ""text"": ""textvalue2"" }
            ]
}";
        var mappingDef = @"
array -> o.middle*.value
";

        var originObj = JsonNode.Parse(json);
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(
            originObj["array"].AsArray().Select(i => i.ToString()),
            target["o"]["middle"].AsArray().Select(i => i["value"].ToString()));
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetIterativeOnMiddleArrayPlus2LevelField_Success()
    {
        var json = @"{
            ""array"": [ 1, 2 ],
            ""array2"": [
                { ""text"": ""textvalue"" },
                { ""text"": ""textvalue2"" }
            ]
}";
        var mappingDef = @"
array -> o.middle*.int.value
";

        var originObj = JsonNode.Parse(json);
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(
            originObj["array"].AsArray().Select(i => i.ToString()),
            target["o"]["middle"].AsArray().Select(i => i["int"]["value"].ToString()));
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetIterativeOnMiddleArrayPlus3LevelField_Success()
    {
        var json = @"{
            ""array"": [ 1, 2 ],
            ""array2"": [
                { ""text"": ""textvalue"" },
                { ""text"": ""textvalue2"" }
            ]
}";
        var mappingDef = @"
array -> o.middle*.int.i.value
";

        var originObj = JsonNode.Parse(json);
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        Assert.AreEqual(
            originObj["array"].AsArray().Select(i => i.ToString()),
            target["o"]["middle"].AsArray().Select(i => i["int"]["i"]["value"].ToString()));
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsJsonobjectType_SetIterativeOnMiddleArrayMultipleArrayField_Success()
    {
        var json = @"{
            ""array"": [ 1, 2 ],
            ""array2"": [
                { ""text"": ""textvalue"" },
                { ""text"": ""textvalue2"" }
            ]
}";
        var mappingDef = @"
array -> o.middle*.int.value*
";

        var originObj = JsonNode.Parse(json);
        var target = new JsonObject();

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperJsonToJson();

        handler.Map(originObj, target);

        var result = new List<string>();
        foreach (var e in target["o"]["middle"].AsArray())
        {
            foreach (var i in e["int"]["value"].AsArray())
            {
                result.Add(i.ToString());
            }
        }
        Assert.AreEqual(
            originObj["array"].AsArray().Select(i => i.ToString()),
            result);
    }

}
