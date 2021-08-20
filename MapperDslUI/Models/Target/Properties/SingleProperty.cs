namespace MapperDslUI.Models.Target.Properties
{
    using System.Collections.Generic;
    public class SingleProperty : Property
    {
        public List<SinglePropertyItem> Items { get; set; } = new List<SinglePropertyItem>();
    }
}
