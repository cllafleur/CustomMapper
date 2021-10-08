namespace MapperDslUI.Models.Origin
{
    using System;

    public class DataWithFormattedCustomFieldsReadSerializableModel : DataWithFormattedCustomFieldsBaseSerializableModel
    {
        /// <summary>Gets or sets The custom code table 1</summary>
        public Reference CustomCodeTable1
        {
            get;
            set;
        }

        /// <summary>Gets or sets The custom code table 2</summary>
        public Reference CustomCodeTable2
        {
            get;
            set;
        }

        /// <summary>Gets or sets The custom code table 3</summary>
        public Reference CustomCodeTable3
        {
            get;
            set;
        }

        public DateTime? Date1
        {
            get;
            set;
        }

        public DateTime? Date2
        {
            get;
            set;
        }

        public DateTime? Date3
        {
            get;
            set;
        }
    }
}


