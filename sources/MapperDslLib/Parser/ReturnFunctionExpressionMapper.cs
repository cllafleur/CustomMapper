using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Parser
{
    public class ReturnFunctionExpressionMapper : IExpressionMapper, INamedExpressionMapper
    {
        public ParsingInfo ParsingInfo { get; }

        public string ExpressionName { get; set; }
        public FunctionMapper Function { get; }
        public InstanceRefMapper Value { get; }

        public ReturnFunctionExpressionMapper(FunctionMapper function, InstanceRefMapper value, ParsingInfo info)
        {
            Function = function;
            Value = value;
            ParsingInfo = info;
        }
    }
}
