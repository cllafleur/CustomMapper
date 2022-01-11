namespace MapperDslLib.Parser
{
    internal class InsertInstanceRefMapper: IExpressionMapper
    {
        public InstanceRefMapper InstanceRef { get; }
        public FieldInstanceRefMapper SetFieldRef { get; }
        public ParsingInfo ParsingInfo { get; }


        public InsertInstanceRefMapper(InstanceRefMapper instanceRef, FieldInstanceRefMapper setFieldRef, ParsingInfo parsingInfo)
        {
            this.InstanceRef = instanceRef;
            this.SetFieldRef = setFieldRef;
            this.ParsingInfo = parsingInfo;
        }

        public string GetLitteral()
        {
            if (InstanceRef != null)
            {
                return string.Join(".", new[] { InstanceRef.GetLitteral(), SetFieldRef.Value });
            }
            return SetFieldRef.Value;
        }
    }
}