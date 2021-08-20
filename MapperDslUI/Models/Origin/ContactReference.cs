namespace MapperDslUI.Models.Origin
{

    public class ContactReference : Reference
    {
        public ContactReference()
        {
        }

        public ContactReference(string email, string uri)
            : base()
        {
            this.Email = Email;
            this.Uri = uri;
        }

        /// <summary>
        /// Uri of the referenced object
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Uri of the referenced object
        /// </summary>
        public string Uri { get; set; }
    }
}
