namespace MapperDslUI.Models.Origin
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class VacancyCriteriaRead 
    {
        /// <summary>
        /// The vacancy free criteria 1
        /// </summary>
        public string FreeCriteria1
        {
            get;
            set;
        }

        /// <summary>
        /// The vacancy free criteria 2
        /// </summary>
        public string FreeCriteria2
        {
            get;
            set;
        }

        /// <summary>
        /// The vacancy free criteria 1 formatted
        /// </summary>
        public string FreeCriteria1HTML
        {
            get;
            set;
        }

        /// <summary>
        /// The vacancy free criteria 2 formatted
        /// </summary>
        public string FreeCriteria2HTML
        {
            get;
            set;
        }

        /// <summary>
        /// The vacancy diploma
        /// </summary>
        public Reference Diploma
        {
            get;
            set;
        }

        /// <summary>
        /// The vacancy education level
        /// </summary>
        public Reference JobEducationLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The vacancy experience level
        /// </summary>
        public Reference JobExperienceLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The specialisations object list
        /// </summary>
        public List<Reference> Specialisations
        {
            get;
            set;
        }

        /// <summary>
        /// The skills object list
        /// </summary>
        public List<Reference> Skills
        {
            get;
            set;
        }

        /// <summary>
        /// The candidate criteria custom fields object
        /// </summary>
        public DataWithFormattedCustomFieldsReadSerializableModel CriteriaCustomFields
        {
            get;
            set;
        }
    }
}
