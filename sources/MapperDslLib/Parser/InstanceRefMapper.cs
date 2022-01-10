using System.Text;

namespace MapperDslLib.Parser
{
    public class InstanceRefMapper : IExpressionMapper, INamedExpressionMapper
    {

        public InstanceRefMapper(IEnumerable<FieldInstanceRefMapper> children, ParsingInfo infos)
        {
            ParsingInfo = infos;
            Children = children;
        }

        public string GetLitteral()
        {
            return string.Join(".", Children.Select(c => c.Value));
        }

        public ParsingInfo ParsingInfo { get; }
        public IEnumerable<FieldInstanceRefMapper> Children { get; }
        public string ExpressionName { get; set; }
    }
}