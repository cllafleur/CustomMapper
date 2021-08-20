using System;

namespace MapperDslUI.Models.Origin
{
    public class VacancyOriginRead
    {
        /// <summary>
        /// The vacancy recruiting reason
        /// </summary>
        public Reference RecruitingReason
        {
            get;
            set;
        }
        /// <summary>
        /// The number of vacancies
        /// </summary>
        public int? NumberOfVacancies
        {
            get;
            set;
        }
        /// <summary>
        /// The vacancy beginning date
        /// </summary>
        public DateTime? BeginningDate
        {
            get;
            set;
        }
        /// <summary>
        /// The eeo object
        /// </summary>
        public Reference EEO
        {
            get;
            set;
        }
        /// <summary>
        /// The line manager object
        /// </summary>
        public ContactReference LineManager
        {
            get;
            set;
        }
        /// <summary>
        /// The agency comment object
        /// </summary>
        public string CommentForAgency
        {
            get;
            set;
        }
        /// <summary>
        /// The requesting party custom fields object
        /// </summary>
        public DataWithFormattedCustomFieldsReadSerializableModel RequestingPartyCustomFields
        {
            get;
            set;
        }
    }
}
