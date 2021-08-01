using System.Collections.Generic;

namespace MapperDslLib
{
    internal class FunctionGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private IExtractFunctionHandler functionHandler;
        private IEnumerable<IGetRuntimeHandler<TOrigin>> arguments;

        public FunctionGetRuntimeHandler(IExtractFunctionHandler functionHandler, IEnumerable<IGetRuntimeHandler<TOrigin>> arguments)
        {
            this.functionHandler = functionHandler;
            this.arguments = arguments;
        }

        public object Get(TOrigin obj)
        {
            List<object> values = new List<object>();
            foreach (var arg in arguments)
            {
                values.Add(arg.Get(obj));
            }
            return functionHandler.GetObject(obj, values.ToArray());
        }
    }
}