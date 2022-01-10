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
        private TextReader _mappingDefinition;
        private IFunctionHandlerProvider _functionHandlerProvider;

        public Mapper(IFunctionHandlerProvider functionProvider, TextReader mappingDefinition)
        {
            _mappingDefinition = mappingDefinition;
            _functionHandlerProvider = functionProvider;
        }

        public IEnumerable<StatementMapper> Actions { get; private set; }

        public void Dispose()
        {

        }

        public (bool success, string[] errors) Load()
        {
            AntlrInputStream stream = new AntlrInputStream(_mappingDefinition);
            var lexer = new MapperLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MapperParser(tokenStream);
            var walker = new ParseTreeWalker();
            var visitor = new MapperVisitor();
            var listener = new MapperListener(visitor);

            var syntaxErrorListener = new SyntaxErrorListener();
            parser.AddErrorListener(syntaxErrorListener);

            walker.Walk(listener, parser.file());
            var data = listener.Result;
            Actions = data;
            if (parser.NumberOfSyntaxErrors > 0)
            {
                throw new ParsingDefinitionException($"Syntax errors, count : {parser.NumberOfSyntaxErrors}\n\n{string.Join("\n", syntaxErrorListener.GetErrors())}");
            }
            return (parser.NumberOfSyntaxErrors == 0, syntaxErrorListener.GetErrors());
        }

        public IMapperHandler<TOrigin, TTarget> GetMapper<TOrigin, TTarget>(CompileOption option = CompileOption.v1)
        {
            var compiler = new MapperCompiler<TOrigin, TTarget>(_functionHandlerProvider, option);
            var handler = compiler.Compile(Actions);
            return handler;
        }
    }
}
