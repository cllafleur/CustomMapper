using System.Collections.Generic;

namespace MapperDslLib
{
    internal class FunctionSetRuntimeHandler<T> : ISetRuntimeHandler<T>
    {
        private IInsertFunctionHandler insertFunctionHandler;
        private List<IGetRuntimeHandler<T>> arguments;

        public FunctionSetRuntimeHandler(IInsertFunctionHandler insertFunctionHandler, List<IGetRuntimeHandler<T>> arguments)
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