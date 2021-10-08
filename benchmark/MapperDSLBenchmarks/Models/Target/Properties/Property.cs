namespace MapperDslUI.Models.Target.Properties
{
    using System.Xml.Serialization;

    [XmlInclude(typeof(SingleProperty))]
    [XmlInclude(typeof(CompositeProperty))]
    public class Property
    {
        public PropertyKindEnum Kind { get; protected set; }
    }
}
