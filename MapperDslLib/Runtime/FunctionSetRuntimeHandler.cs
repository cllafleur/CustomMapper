using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal class FunctionSetRuntimeHandler<T> : ISetRuntimeHandler<T>
    {
        private IInsertFunctionHandler<T> insertFunctionHandler;
        private List<IGetRuntimeHandler<T>> arguments;

        public FunctionSetRuntimeHandler(IInsertFunctionHandler<T> insertFunctionHandler, List<IGetRuntimeHandler<T>> arguments)
        {
            this.insertFunctionHandler = insertFunctionHandler;
            this.arguments = arguments;
        }

        public void SetValue(T obj, object value)
        {
            List<object> parameters = new List<object>();
            foreach (var arg in arguments)
            {
                parameters.Add(arg.Get(obj));
            }
            insertFunctionHandler.SetObject(obj, value, parameters.ToArray());
        }
    }
}