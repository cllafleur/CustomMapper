using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib
{
    public class MapperListener : MapperBaseListener
    {
        private List<string> _currentRow;
        private readonly MapperVisitor _visitor;

        public MapperListener(MapperVisitor visitor)
        {
            _visitor = visitor;
            Result = new List<StatementMapper>();
        }


        public List<StatementMapper> Result { get; private set; }

        public override void EnterStatement([NotNull] MapperParser.StatementContext context)
        {
            base.EnterStatement(context);
        }

        public override void ExitStatement([NotNull] MapperParser.StatementContext context)
        {
            base.ExitStatement(context);

            Result.Add((StatementMapper)_visitor.VisitStatement(context));
        }
    }
}
