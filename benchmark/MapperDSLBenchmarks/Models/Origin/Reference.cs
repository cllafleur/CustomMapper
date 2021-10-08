namespace MapperDslUI.Models.Origin
{
    /// <summary>
    /// Referencing object
    /// </summary>
    public class Reference
    {
        public Reference()
        {
        }

        public Reference(string id, string label)
        {
            this.Id = id;
            this.Label = label;
        }

        /// <summary>
        /// Gets or sets ReferenceIdentifier of the referenced object
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets ReferenceLabel of the referenced object
        /// </summary>
        public string Label { get; set; }
    }
}


