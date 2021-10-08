namespace MapperDslUI.Models.Origin
{
    /// <summary>
    /// VacancyReferralRead
    /// </summary>
    [System.Serializable]
    public class VacancyReferralRead
    {
        /// <summary>
        /// The bonus amount
        /// </summary>
        public double? BonusAmount
        {
            get;
            set;
        }

        /// <summary>
        /// The currency
        /// </summary>
        public Reference Currency
        {
            get;
            set;
        }

        /// <summary>
        /// The bonus type
        /// </summary>
        public Reference BonusType
        {
            get;
            set;
        }
    }
}
