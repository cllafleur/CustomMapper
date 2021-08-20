namespace MapperDslUI.Models.Origin
{
    public class VacancyLocationRead
    {
        /// <summary>
        /// The vacancy geographical area
        /// </summary>
        public Reference GeographicalArea
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy country
        /// </summary>
        public Reference Country
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy region
        /// </summary>
        public Reference Region
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy department
        /// </summary>
        public Reference Department
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy address
        /// </summary>
        public string Address
        {
            get;
            set;
        }
        /// <summary>
        /// The location  custom fields object
        /// </summary>
        public DataWithFormattedCustomFieldsReadSerializableModel LocationCustomFields
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy latitude
        /// </summary>
        public double? Latitude
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy longitude
        /// </summary>
        public double? Longitude
        {
            get;
            set;
        }
    }
}
