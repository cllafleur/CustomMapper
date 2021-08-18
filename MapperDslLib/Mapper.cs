using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapperDslLib
{
    public class Mapper : IDisposable
    {
        private Stream _mappingDefinition;
        private IFunctionHandlerProvider _functionHandlerProvider;

        public Mapper(IFunctionHandlerProvider functionProvider, Stream mappingDefinition)
        {
            _mappingDefinition = mappingDefinition;
            _functionHandlerProvider = functionProvider;
        }

        public IEnumerable<StatementMapper> Actions { get; private set; }

        public void Dispose()
        {

        }

        public void Load()
        {
            AntlrInputStream stream = new AntlrInputStream(_mappingDefinition);
            var lexer = new MapperLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MapperParser(tokenStream);
            var walker = new ParseTreeWalker();
            var visitor = new MapperVisitor();
            var listener = new MapperListener(visitor);

            walker.Walk(listener, parser.file());
            var data = listener.Result;
            Actions = data;
        }

        public IMapperHandler<TOrigin, TTarget> GetMapper<TOrigin, TTarget>()
        {
            var compiler = new MapperCompiler<TOrigin, TTarget>(_functionHandlerProvider);
            var handler = compiler.Compile(Actions);
            return handler;
        }
    }
}
