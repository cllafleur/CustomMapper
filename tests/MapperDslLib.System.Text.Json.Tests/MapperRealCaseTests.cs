namespace MapperDslLib.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using MapperDslLib.Runtime;
using NUnit.Framework;

public class MapperRealCaseTests
{
    class Item
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public override bool Equals(object obj)
        {
            var input = obj as Item;
            return input?.Id == Id && input?.Label == Label;
        }
    }
    class ItemFunction : IInsertFunctionHandler<List<Item>>, IInsertTupleFunctionHandler<List<Item>>
    {
        public void SetObject(List<Item> instanceObject, TupleSourceResult source, Parameters parameters)
        {
            var idIndex = GetIndexOf("id", source.TupleDataInfo);
            var labelIndex = GetIndexOf("label", source.TupleDataInfo);

            foreach (var tuple in source.Result)
            {
                var item = new Item
                {
                    Id = (string)tuple[idIndex],
                    Label = (string)tuple[labelIndex],
                };
                instanceObject.Add(item);
            }
        }

        private int GetIndexOf(string name, SourceInfo[] infos)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetObject(List<Item> instanceObject, SourceResult source, Parameters parameters)
        {
            throw new NotSupportedException();
        }
    }

    [Test]
    public void Map_OriginAsJsonobjectTargetAsItemType_SetWithFunction_Success()
    {
        var json = @"{
            ""data"": [
                { ""id"": ""id1"", ""label"": ""label1"" },
                { ""id"": ""id2"", ""label"": ""label2"" }
            ]
}";
        var mappingDef = @"
(id: data.id, label: data.label) -> item()
";

        var originObj = JsonNode.Parse(json);
        var target = new List<Item>();

        var funcProvider = new FunctionHandlerProvider();
        funcProvider.Register<List<Item>, ItemFunction>("item");
        var mapper = new Mapper(funcProvider, new StringReader(mappingDef));
        mapper.Load();
        var handler = mapper.GetMapperFromJson<List<Item>>();

        handler.Map(originObj, target);

        CollectionAssert.AreEqual(
            target,
            new List<Item>() { new Item { Id = "id1", Label = "label1" }, new Item { Id = "id2", Label = "label2" } });
    }

    class AttachmentFunction : IExtractFunctionHandler<JsonNode>
    {
        public SourceResult GetObject(JsonNode instanceObj, Parameters parameters)
        {
            return null;
        }
    }

//    [Test]
//    public void Map_OriginAsJsonobjectTargetAsItemType_SetWithTupleInsert_Success()
//    {
//        var json = @"{
//        ""name"": ""John"",
//        ""email"": ""jDoe@nd.xxx"",
//        ""civility"": ""Mr""
//}";
//        var mappingDef = @"
//name -> informations.name
//email -> informations.email
//civility -> informations.civility
//attachments() -> attachments*.(key: key, type: fileType, filename: description)
//";

//        var originObj = JsonNode.Parse(json);
//        var target = new JsonObject();

//        var funcProvider = new FunctionHandlerProvider();
//        funcProvider.Register<JsonNode, AttachmentFunction>("attachments");
//        var mapper = new Mapper(funcProvider, new StringReader(mappingDef));
//        mapper.Load();
//        var handler = mapper.GetMapperJsonToJson();

//        handler.Map(originObj, target);

//        Assert.That(target["informations"]["name"].GetValue<string>(), Is.EqualTo("John"));
//        Assert.That(target["informations"]["email"].GetValue<string>(), Is.EqualTo("jDoe@nd.xxx"));
//        Assert.That(target["informations"]["civility"].GetValue<string>(), Is.EqualTo("Mr"));
//    }
}