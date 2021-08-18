using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperDslUI.Models.Target
{
    public class DescriptionObject
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string Description2 { get; set; }
    }

    public class TargetObject
    {
        public DescriptionObject Description { get; set; }

        public DateTime ModificationDate { get; set; }

        public Dictionary<string, string> Properties { get; set; }
    }
}
