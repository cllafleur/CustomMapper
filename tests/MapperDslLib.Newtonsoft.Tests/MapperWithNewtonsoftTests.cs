namespace MapperDslLib.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

public class MapperWithNewtonsoftTests
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

        public DateTime? ModificationDate { get; set; }

    }

    private const string CASE1 = @"Title -> Title
Description -> Description
ModificationDate -> ModificationDate
";

    [Test]
    public void Map_OriginAsJobjectTargetAsStaticType_SetRootLevel_Success()
    {
        OriginObject originObj = new OriginObject();
        originObj.Title = "supertitle";
        originObj.Description = "superdescription";
        originObj.ModificationDate = new DateTime(2021, 07, 31);
        var target = new TargetObject();
        var oSerialized = JsonConvert.SerializeObject(originObj);
        JObject origin = (JObject)JsonConvert.DeserializeObject(oSerialized);

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
        mapper.Load();
        var handler = mapper.GetMapper<JObject, TargetObject>();

        handler.Map(origin, target);

        Assert.AreEqual(originObj.Title, target.Title);
        Assert.AreEqual(originObj.ModificationDate, target.ModificationDate);
        Assert.AreEqual(originObj.Description, target.Description);
    }

    [Test]
    public void Map_OriginAsJobjectTargetAsJobjectType_SetRootLevel_Success()
    {
        OriginObject originObj = new OriginObject();
        originObj.Title = "supertitle";
        originObj.Description = "superdescription";
        originObj.ModificationDate = new DateTime(2021, 07, 31);
        var target = new JObject();
        var oSerialized = JsonConvert.SerializeObject(originObj);
        JObject origin = (JObject)JsonConvert.DeserializeObject(oSerialized);

        var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(CASE1));
        mapper.Load();
        var handler = mapper.GetMapper<JObject, JObject>();

        handler.Map(origin, target);

        Assert.AreEqual(originObj.Title, target["Title"].ToString());
        Assert.AreEqual(originObj.ModificationDate, (DateTime)target["ModificationDate"]);
        Assert.AreEqual(originObj.Description, target["Description"].ToString());
    }
}
