using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal class FunctionGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private IExtractFunctionHandler<TOrigin> functionHandler;
        private IEnumerable<IGetRuntimeHandler<TOrigin>> arguments;

        public FunctionGetRuntimeHandler(IExtractFunctionHandler<TOrigin> functionHandler, IEnumerable<IGetRuntimeHandler<TOrigin>> arguments)
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