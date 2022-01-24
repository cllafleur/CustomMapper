using System;
using System.Collections.Generic;
using System.Text;
using MapperDslLib.Runtime.Accessor;

namespace MapperDslLib
{
    public class CompilerOptions
    {
        public CompilerVersions Version { get; set; } = CompilerVersions.v2;
        public IGetAccessorFactoryHandler[] ExtractGetAccessorFactoryHandlers { get; set; }
        public IGetAccessorFactoryHandler[] InsertGetAccessorFactoryHandlers { get; set; }
        public ISetAccessorFactoryHandler[] InsertSetAccessorFactoryHandlers { get; set; }
        public IGetAccessor DeconstructorAccessor { get; set; }
    }
}
