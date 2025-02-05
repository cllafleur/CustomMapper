﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapperDslLib.Runtime
{
    internal class MapperHandler<TOrigin, TTarget> : IMapperHandler<TOrigin, TTarget>
    {
        private IEnumerable<IStatementRuntimeHandler<TOrigin, TTarget>> operations;

        public MapperHandler(List<IStatementRuntimeHandler<TOrigin, TTarget>> operations)
        {
            this.operations = operations;
        }

        public void Map(TOrigin origin, TTarget target)
        {
            foreach (var operation in operations)
            {
                operation.Evaluate(origin, target);
            }
        }
    }
}