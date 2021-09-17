namespace MapperDslUI.Models.Origin
{
    using System.Collections.Generic;

    public class VacancyJobDescriptionRead
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
        /// <summary>
        /// The vacancy job time
        /// </summary>
        public Reference JobTime
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy salary range
        /// </summary>
        public Reference SalaryRange
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy contract type
        /// </summary>
        public Reference ContractType
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy contract length
        /// </summary>
        public string ContractLength
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy primary profile
        /// </summary>
        public Reference PrimaryProfile
        {
            get;
            set;
        }
        /// <summary>
        /// The secondary profiles object list
        /// </summary>
        public List<Reference> SecondaryProfiles
        {
            get;
            set;
        }
        /// <summary>
        /// The country 
        /// </summary>
        public Reference Country
        {
            get;
            set;
        }
        /// <summary>
        /// The profesional category 
        /// </summary>
        public Reference ProfessionalCategory
        {
            get;
            set;
        }
        /// <summary>
        /// The job description custom fields object
        /// </summary>
        public DataWithFormattedCustomFieldsReadSerializableModel JobDescriptionCustomFields
        {
            get;
            set;
        }
    }
}
