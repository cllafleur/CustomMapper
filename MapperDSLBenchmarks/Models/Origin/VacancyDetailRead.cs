namespace MapperDslUI.Models.Origin
{
    using System;
    using System.Collections.Generic;

    public class VacancyDetailRead
    {
        /// <summary>
        /// The vacancy reference
        /// </summary>
        public string Reference
        {
            get;
            set;
        }
        /// <summary>The vacancy status</summary>
        public Reference Status
        {
            get;
            set;
        }

        /// <summary>The translation language of the object</summary>
        public int? LanguageId
        {
            get;
            set;
        }
        /// <summary>The vacancy category</summary>
        public Reference VacancyCategory
        {
            get;
            set;
        }
        /// <summary>The vacancy view</summary>
        public Reference VacancyView
        {
            get;
            set;
        }
        /// <summary>The candidate view</summary>


        public Reference CandidateView
        {
            get;
            set;
        }
        /// <summary>The vacancy organization</summary>



        public Reference Organisation
        {
            get;
            set;
        }
        /// <summary>The candidate redirection url</summary>



        public string CandidateRedirectionUrl
        {
            get;
            set;
        }
        /// <summary>The vacancy creation date</summary>



        public DateTime? CreationDate
        {
            get;
            set;
        }
        /// <summary>The vacancy modification date</summary>



        public DateTime? ModificationDate
        {
            get;
            set;
        }
        /// <summary>The vacancy requesting party object</summary>



        public VacancyOriginRead Origin
        {
            get;
            set;
        }
        /// <summary>The vacancy location object</summary>



        public VacancyLocationRead Location
        {
            get;
            set;
        }
        /// <summary>The candidate criteria object</summary>



        public VacancyCriteriaRead Criteria
        {
            get;
            set;
        }
        /// <summary>The job description object</summary>




        public VacancyJobDescriptionRead JobDescription
        {
            get;
            set;
        }
        /// <summary>The custom fields object</summary>


        public DataWithFormattedCustomFieldsReadSerializableModel CustomFields
        {
            get;
            set;
        }
        /// <summary>The custom block1 custom fields object</summary>


        public DataWithFormattedCustomFieldsReadSerializableModel CustomBlock1CustomFields
        {
            get;
            set;
        }
        /// <summary>The custom block2 custom fields object</summary>


        public DataWithFormattedCustomFieldsReadSerializableModel CustomBlock2CustomFields
        {
            get;
            set;
        }
        /// <summary>The custom block3 custom fields object</summary>


        public DataWithFormattedCustomFieldsReadSerializableModel CustomBlock3CustomFields
        {
            get;
            set;
        }
        /// <summary>The custom block4 custom fields object</summary>


        public DataWithFormattedCustomFieldsReadSerializableModel CustomBlock4CustomFields
        {
            get;
            set;
        }
        /// <summary>The languages</summary>




        public List<VacancyLanguageRead> Languages
        {
            get;
            set;
        }
        /// <summary>The vacancy referral information</summary>


        public VacancyReferralRead Referral
        {
            get;
            set;
        }
        /// <summary>Is published on intranet</summary>


        public bool PublishedOnIntranet
        {
            get;
            set;
        }
        /// <summary>Is published on internet</summary>


        public bool PublishedOnInternet
        {
            get;
            set;
        }
        /// <summary>Customer authorization informations</summary>


        public AuthorizationInfoRead Authorizations
        {
            get;
            set;
        }
        /// <summary>The candidate redirection url</summary>


        public string VacancyCoverUrl
        {
            get;
            set;
        }
    }
}




