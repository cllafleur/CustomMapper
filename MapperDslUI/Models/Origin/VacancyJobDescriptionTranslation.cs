namespace MapperDslUI.Models.Origin
{
    public class VacancyJobDescriptionTranslation
    {
        /// <summary>
        /// The vacancy title
        /// </summary>
        public string JobTitle
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy description 1
        /// </summary>
        public string Description1
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy description 1 formatted
        /// </summary>
        public string Description1HTML
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy description 2
        /// </summary>
        public string Description2
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy description 2 formatted
        /// </summary>
        public string Description2HTML
        {
            get;
            set;
        }
        public DataWithFormattedCustomFieldsBaseSerializableModel CustomFields
        {
            get;
            set;
        }
    }
}
