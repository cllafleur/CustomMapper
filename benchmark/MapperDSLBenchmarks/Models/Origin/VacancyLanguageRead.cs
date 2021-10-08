namespace MapperDslUI.Models.Origin
{
    public class VacancyLanguageRead
    {
        /// <summary>The language</summary>
        public Reference Language
        {
            get;
            set;
        }
        /// <summary>The language level</summary>
        public Reference LanguageLevel
        {
            get;
            set;
        }
        public VacancyLanguageRead()
        {
        }
        public VacancyLanguageRead(Reference language, Reference languageLevel)
        {
            this.Language = language;
            this.LanguageLevel = languageLevel;
        }
    }
}
