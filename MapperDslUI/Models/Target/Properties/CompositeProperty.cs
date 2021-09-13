namespace MapperDslUI.Models.Target.Properties
{
    using System.Collections.Generic;

    public class CompositeProperty : Property
    {
        public CompositeProperty()
        {
            this.Kind = PropertyKindEnum.Composite;
        }

        public List<CompositePropertyItem> Items { get; set; } = new List<CompositePropertyItem>();
    }
}
