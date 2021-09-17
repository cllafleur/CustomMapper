using System;

namespace MapperDslUI.Models.Origin
{
    public class DataWithCustomFieldsBase
    {
        /// <summary>Gets or sets The date 1</summary>
        public DateTime? Date1 { get; set; }

        /// <summary>Gets or sets The date 2</summary>
        public DateTime? Date2 { get; set; }

        /// <summary>Gets or sets The date 3</summary>
        public DateTime? Date3 { get; set; }

        public string LongText1 { get; set; }

        public string LongText2 { get; set; }

        public string LongText3 { get; set; }

        public string ShortText1 { get; set; }

        public string ShortText2 { get; set; }

        public string ShortText3 { get; set; }
    }
}


