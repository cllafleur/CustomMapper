using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

namespace MapperDslLib
{
    internal class SyntaxErrorListener : BaseErrorListener
    {
        private List<string> errors = new List<string>();

        public SyntaxErrorListener()
        {
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
            errors.Add($"Line: {line} Pos: {charPositionInLine} {msg}");
        }

        public string[] GetErrors()
        {
            return errors.ToArray();
        }
    }
}